namespace GenCode128
{
    /// <summary>
    /// Static tools for determining codes for individual characters in the content
    /// </summary>
    public static class Code128Code
    {
        private const int CShift = 98;

        private const int CCodeA = 101;

        private const int CCodeB = 100;

        private const int CStartA = 103;

        private const int CStartB = 104;

        private const int CStop = 106;
        
        /// <summary>
        /// Indicates which code sets can represent a character -- CodeA, CodeB, or either
        /// </summary>
        public enum CodeSetAllowed
        {
            CodeA,

            CodeB,

            CodeAorB
        }

        /// <summary>
        /// Get the Code128 code value(s) to represent an ASCII character, with 
        /// optional look-ahead for length optimization
        /// </summary>
        /// <param name="charAscii">The ASCII value of the character to translate</param>
        /// <param name="lookAheadAscii">The next character in sequence (or -1 if none)</param>
        /// <param name="currCodeSet">The current codeset, that the returned codes need to follow;
        /// if the returned codes change that, then this value will be changed to reflect it</param>
        /// <returns>An array of integers representing the codes that need to be output to produce the 
        /// given character</returns>
        public static int[] CodesForChar(int charAscii, int lookAheadAscii, ref CodeSet currCodeSet)
        {
            int[] result;
            int shifter = -1;

            if (!CharCompatibleWithCodeset(charAscii, currCodeSet))
            {
                // if we have a lookahead character AND if the next character is ALSO not compatible
                if ((lookAheadAscii != -1) && !CharCompatibleWithCodeset(lookAheadAscii, currCodeSet))
                {
                    // we need to switch code sets
                    switch (currCodeSet)
                    {
                        case CodeSet.CodeA:
                            shifter = CCodeB;
                            currCodeSet = CodeSet.CodeB;
                            break;
                        case CodeSet.CodeB:
                            shifter = CCodeA;
                            currCodeSet = CodeSet.CodeA;
                            break;
                    }
                }
                else
                {
                    // no need to switch code sets, a temporary SHIFT will suffice
                    shifter = CShift;
                }
            }

            if (shifter != -1)
            {
                result = new int[2];
                result[0] = shifter;
                result[1] = CodeValueForChar(charAscii);
            }
            else
            {
                result = new int[1];
                result[0] = CodeValueForChar(charAscii);
            }

            return result;
        }

        /// <summary>
        /// Tells us which codesets a given character value is allowed in
        /// </summary>
        /// <param name="charAscii">ASCII value of character to look at</param>
        /// <returns>Which codeset(s) can be used to represent this character</returns>
        public static CodeSetAllowed CodesetAllowedForChar(int charAscii)
        {
            if (charAscii >= 32 && charAscii <= 95)
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
        /// <param name="currcs">codeset context to test</param>
        /// <returns>true if the codeset contains a representation for the ASCII character</returns>
        public static bool CharCompatibleWithCodeset(int charAscii, CodeSet currcs)
        {
            CodeSetAllowed csa = CodesetAllowedForChar(charAscii);
            return csa == CodeSetAllowed.CodeAorB || (csa == CodeSetAllowed.CodeA && currcs == CodeSet.CodeA)
                   || (csa == CodeSetAllowed.CodeB && currcs == CodeSet.CodeB);
        }

        /// <summary>
        /// Gets the integer code128 code value for a character (assuming the appropriate code set)
        /// </summary>
        /// <param name="charAscii">character to convert</param>
        /// <returns>code128 symbol value for the character</returns>
        public static int CodeValueForChar(int charAscii)
        {
            return (charAscii >= 32) ? charAscii - 32 : charAscii + 64;
        }

        /// <summary>
        /// Return the appropriate START code depending on the codeset we want to be in
        /// </summary>
        /// <param name="cs">The codeset you want to start in</param>
        /// <returns>The code128 code to start a barcode in that codeset</returns>
        public static int StartCodeForCodeSet(CodeSet cs)
        {
            return cs == CodeSet.CodeA ? CStartA : CStartB;
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