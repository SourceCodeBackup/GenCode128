namespace GenCode128
{
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Represent the set of code values to be output into barcode form
    /// </summary>
    public class Code128Content
    {
        /// <summary>
        /// Create content based on a string of ASCII data
        /// </summary>
        /// <param name="asciiData">the string that should be represented</param>
        public Code128Content(string asciiData)
        {
            this.Codes = this.StringToCode128(asciiData);
        }

        /// <summary>
        /// Provides the Code128 code values representing the object's string
        /// </summary>
        public int[] Codes { get; }

        /// <summary>
        /// Transform the string into integers representing the Code128 codes
        /// necessary to represent it
        /// </summary>
        /// <param name="asciiData">String to be encoded</param>
        /// <returns>Code128 representation</returns>
        private int[] StringToCode128(string asciiData)
        {
            // turn the string into ascii byte data
            var asciiBytes = Encoding.ASCII.GetBytes(asciiData);
            
            // a support array, for our lookahead 
            int[] nextchar = new int[6];

            // a support array for us to choose which is the best starting code
            Code128Code.CodeSetAllowed[] csa = new Code128Code.CodeSetAllowed[4];

            // decide which codeset to start with
            csa[0] = asciiBytes.Length > 0
                                        ? Code128Code.CodesetAllowedForChar(asciiBytes[0])
                                        : Code128Code.CodeSetAllowed.CodeAorBorC;
            csa[1] = asciiBytes.Length > 1
                                        ? Code128Code.CodesetAllowedForChar(asciiBytes[1])
                                        : Code128Code.CodeSetAllowed.CodeAorBorC;
            csa[2] = asciiBytes.Length > 2
                                        ? Code128Code.CodesetAllowedForChar(asciiBytes[2])
                                        : Code128Code.CodeSetAllowed.CodeAorBorC;
            csa[3] = asciiBytes.Length > 3
                                        ? Code128Code.CodesetAllowedForChar(asciiBytes[3])
                                        : Code128Code.CodeSetAllowed.CodeAorBorC;
            var currentCodeSet = this.GetBestStartSet(csa);

            // set up the beginning of the barcode
            // assume no codeset changes, account for start, checksum, and stop
            var codes = new ArrayList(asciiBytes.Length + 3) { Code128Code.StartCodeForCodeSet(currentCodeSet) };

            // add the codes for each character in the string, checking the 6 char lookahead
            for (var i = 0; i < asciiBytes.Length; i++)
            {
                nextchar[0] = asciiBytes[i];
                nextchar[1] = asciiBytes.Length > (i + 1) ? asciiBytes[i + 1] : -1;
                nextchar[2] = asciiBytes.Length > (i + 2) ? asciiBytes[i + 2] : -1;
                nextchar[3] = asciiBytes.Length > (i + 3) ? asciiBytes[i + 3] : -1;
                nextchar[4] = asciiBytes.Length > (i + 4) ? asciiBytes[i + 4] : -1;
                nextchar[5] = asciiBytes.Length > (i + 5) ? asciiBytes[i + 5] : -1;

                codes.AddRange(Code128Code.CodesForChar(ref nextchar, ref currentCodeSet, ref i));
            }

            // calculate the check digit
            var checksum = (int)codes[0];
            for (var i = 1; i < codes.Count; i++)
            {
                checksum += i * (int)codes[i];
            }

            codes.Add(checksum % 103);

            codes.Add(Code128Code.StopCode());

            var result = codes.ToArray(typeof(int)) as int[];
            return result;
        }

        /// <summary>
        /// Determines the best starting code set based on the the
        /// characters of the string to be encoded
        /// </summary>
        /// <param name="initChars">First 4 characters of input string</param>
        /// <returns>The codeset determined to be best to start with</returns>
        private CodeSet GetBestStartSet(Code128Code.CodeSetAllowed[] initChars)
        {
            int vote = 0;
            int votec = 0;

            for (int i = 0; i < 4; i++)
            {
                switch (initChars[i])
                {
                    case Code128Code.CodeSetAllowed.CodeA:
                        vote += 1;
                        break;
                    case Code128Code.CodeSetAllowed.CodeB:
                        vote -= 1;
                        break;
                    case Code128Code.CodeSetAllowed.CodeC:
                        votec += 2;
                        break;
                        /*default:
                            break;*/
                }
            }

            //Optimizing: 4+ or only 2 characters codeC. Otherwise, vote for codeA or codeB. Ties go to codeB due to my own prejudices
            return (votec == 8 || (votec == 4 && vote == 0)) ? CodeSet.CodeC : ((vote > 0) ? CodeSet.CodeA : CodeSet.CodeB);
        }
    }
}
