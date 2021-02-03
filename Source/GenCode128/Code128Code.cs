namespace GenCode128
{
    using System;

    /// <summary>
    /// Static tools for determining codes for individual characters in the content
    /// </summary>
    public static class Code128Code
    {
        private const int CShift = 98;
        private const int CCodeA = 101;
        private const int CCodeB = 100;
        private const int CCodeC = 99;
        private const int CStartA = 103;
        private const int CStartB = 104;
        private const int CStartC = 105;
        private const int CStop = 106;
        
        /// <summary>
        /// Indicates which code sets can represent a character -- CodeA, CodeB, or either
        /// </summary>
        public enum CodeSetAllowed
        {
            CodeA,
            CodeB,
            CodeC,
            CodeAorB,
            CodeAorBorC
        }

        /// <summary>
        /// Get the Code128 code value(s) to represent an ASCII character
        /// </summary>
        /// <param name="charAscii">The ASCII value of the character to translate</param>
        /// <param name="lookAheadAscii">The next character in sequence (or -1 if none)</param>
        /// <param name="currentCodeSet">The current codeset, that the returned codes need to follow;
        /// if the returned codes change that, then this value will be changed to reflect it</param>
        /// <returns>An array of integers representing the codes that need to be output to produce the 
        /// given character</returns>
        public static int[] CodesForChar(ref int[] charAscii, ref CodeSet currentCodeSet, ref int outerIndex)
        {
            int[] result;
            var shifter = -1;
            var whatToConvert = -1;
            CodeSet bestCodeSet;

            // Let's find out which is the best set to use
            bestCodeSet = BestFitSet(ref charAscii, currentCodeSet, ref shifter);

            // We`re not in the right CodeSet, so we should change it
            if (currentCodeSet != bestCodeSet)
            {
                switch (bestCodeSet)
                {
                    case CodeSet.CodeA:
                        shifter = CCodeA;
                        currentCodeSet = CodeSet.CodeA;
                        break;
                    case CodeSet.CodeB:
                        shifter = CCodeB;
                        currentCodeSet = CodeSet.CodeB;
                        break;
                    case CodeSet.CodeC:
                        shifter = CCodeC;
                        currentCodeSet = CodeSet.CodeC;
                        break;
                }
            }

            // Check if is A/B or C and fill convert, but if it`s CodeSetC, we should code 2 at a time, instead of just one
            switch (currentCodeSet)
            {
                case CodeSet.CodeA:
                case CodeSet.CodeB:
                    whatToConvert = charAscii[0];
                    break;
                case CodeSet.CodeC:
                    whatToConvert = (Convert.ToInt32(Char.ConvertFromUtf32(charAscii[0])) * 10) + (Convert.ToInt32(Char.ConvertFromUtf32(charAscii[1])));
                    outerIndex++;
                    break;
            }

            if (shifter != -1)
            {
                result = new int[2];
                result[0] = shifter;
                result[1] = CodeValueForChar(whatToConvert, currentCodeSet);
            }
            else
            {
                result = new int[1];
                result[0] = CodeValueForChar(whatToConvert, currentCodeSet);
            }

            return result;
        }

        /// <summary>
        /// Determines the best codeset to use, based on Code128 heuristics and optimizations
        /// </summary>
        /// <param name="charAscii">characters to check for</param>
        /// <param name="currentCodeSet">codeset context to test</param>
        /// <param name="shifter">a reference for the shifter</param>
        /// <returns>the best codeset to be used. Shift is also modified if necessary.</returns>
        public static CodeSet BestFitSet(ref int[] charAscii, CodeSet currentCodeSet, ref int shifter)
        {
            bool bestA = false;
            bool bestC = false;
            bool commingFromC = false;
            //bool bestB = false;

            // Optimizing to use CodeSetC. If 6+ are numbers OR we're in the last 4 of the set, and all are numbers
            // Care that we run from 0 to 5 -- Running through 6 and also checking >=3 which means the 4th ahead
            if (currentCodeSet != CodeSet.CodeC)
            {
                bestC = true;
                for (int i = 0; i < 6; i++)
                {
                    bestC = bestC && (CharCompatibleWithCodeset(charAscii[i], CodeSet.CodeC) ? ((charAscii[4] != -1 && charAscii[5] == -1) ? false : true) : ((charAscii[i] == -1 && i > 3) ? true : false));
                    if (!bestC)
                        break;
                }
            }
            else
            {
                // if we're already in C, let's check if we can code at least 2 ascii
                if (CharCompatibleWithCodeset(charAscii[0], CodeSet.CodeC) && CharCompatibleWithCodeset(charAscii[1], CodeSet.CodeC))
                {
                    bestC = true;
                }
                commingFromC = true;
            }

            if (CharCompatibleWithCodeset(charAscii[0], CodeSet.CodeA))
            {
                // if we have a lookahead character AND if the next character is ALSO not compatible
                if ((charAscii[1] != -1) && (commingFromC || CharCompatibleWithCodeset(charAscii[1], CodeSet.CodeA)))
                {
                    // we need to switch code sets
                    bestA = true;
                }
                else
                {
                    // no need to switch code sets, a temporary SHIFT will suffice
                    shifter = CShift;
                }
            }

            //Since we've already checked C and A, if both are False, we can only use CodeSetB
            // Select the prefered one, putting CodeSetC as the first, since it's optimized
            return bestC ? CodeSet.CodeC : (bestA ? CodeSet.CodeA : CodeSet.CodeB);
        }

        /// <summary>
        /// Tells us which codesets a given character value is allowed in
        /// </summary>
        /// <param name="charAscii">ASCII value of character to look at</param>
        /// <returns>Which codeset(s) can be used to represent this character</returns>
        public static CodeSetAllowed CodesetAllowedForChar(int charAscii)
        {
            if (charAscii >= 48 && charAscii <= 57)
            {
                return CodeSetAllowed.CodeC;
            }
            else if (charAscii >= 32 && charAscii <= 95)
            {
                return CodeSetAllowed.CodeAorB;
            }
            else
            {
                return (charAscii < 32) ? CodeSetAllowed.CodeA : CodeSetAllowed.CodeB;
            }
        }

        /// <summary>
        /// Determine if a character can be represented in a given codeset
        /// </summary>
        /// <param name="charAscii">character to check for</param>
        /// <param name="currentCodeSet">codeset context to test</param>
        /// <returns>true if the codeset contains a representation for the ASCII character</returns>
        public static bool CharCompatibleWithCodeset(int charAscii, CodeSet currentCodeSet)
        {
            var csa = CodesetAllowedForChar(charAscii);
            return (csa == CodeSetAllowed.CodeC && currentCodeSet == CodeSet.CodeC)
                   || (csa == CodeSetAllowed.CodeAorB && (currentCodeSet == CodeSet.CodeA || currentCodeSet == CodeSet.CodeB))
                   || (csa == CodeSetAllowed.CodeA && currentCodeSet == CodeSet.CodeA)
                   || (csa == CodeSetAllowed.CodeB && currentCodeSet == CodeSet.CodeB);
        }

        /// <summary>
        /// Gets the integer code128 code value for a character (assuming the appropriate code set)
        /// </summary>
        /// <param name="charAscii">character to convert</param>
        /// <returns>code128 symbol value for the character</returns>
        public static int CodeValueForChar(int charAscii, CodeSet currentCodeSet)
        {
            return currentCodeSet == CodeSet.CodeC ? charAscii : ((charAscii >= 32) ? charAscii - 32 : charAscii + 64);
        }

        /// <summary>
        /// Return the appropriate START code depending on the codeset we want to be in
        /// </summary>
        /// <param name="cs">The codeset you want to start in</param>
        /// <returns>The code128 code to start a barcode in that codeset</returns>
        public static int StartCodeForCodeSet(CodeSet cs)
        {
            return cs == CodeSet.CodeA ? CStartA : (cs == CodeSet.CodeB ? CStartB : CStartC);
        }

        /// <summary>
        /// Return the Code128 stop code
        /// </summary>
        /// <returns>the stop code</returns>
        public static int StopCode()
        {
            return CStop;
        }
    }
}