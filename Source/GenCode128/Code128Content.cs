namespace GenCode128
{
    using System.Text;

    /// <summary>
    /// Represent the set of code values to be output into barcode form
    /// </summary>
    public class Code128Content
    {
        private readonly int[] codeList;

        /// <summary>
        /// Create content based on a string of ASCII data
        /// </summary>
        /// <param name="asciiData">the string that should be represented</param>
        public Code128Content(string asciiData)
        {
            this.codeList = this.StringToCode128(asciiData);
        }

        /// <summary>
        /// Provides the Code128 code values representing the object's string
        /// </summary>
        public int[] Codes
        {
            get
            {
                return this.codeList;
            }
        }

        /// <summary>
        /// Transform the string into integers representing the Code128 codes
        /// necessary to represent it
        /// </summary>
        /// <param name="asciiData">String to be encoded</param>
        /// <returns>Code128 representation</returns>
        private int[] StringToCode128(string asciiData)
        {
            // turn the string into ascii byte data
            byte[] asciiBytes = Encoding.ASCII.GetBytes(asciiData);

            // decide which codeset to start with
            Code128Code.CodeSetAllowed csa1 = asciiBytes.Length > 0
                                                  ? Code128Code.CodesetAllowedForChar(asciiBytes[0])
                                                  : Code128Code.CodeSetAllowed.CodeAorB;
            Code128Code.CodeSetAllowed csa2 = asciiBytes.Length > 0
                                                  ? Code128Code.CodesetAllowedForChar(asciiBytes[1])
                                                  : Code128Code.CodeSetAllowed.CodeAorB;
            CodeSet currcs = this.GetBestStartSet(csa1, csa2);

            // set up the beginning of the barcode
            System.Collections.ArrayList codes = new System.Collections.ArrayList(asciiBytes.Length + 3);
            
            // assume no codeset changes, account for start, checksum, and stop
            codes.Add(Code128Code.StartCodeForCodeSet(currcs));

            // add the codes for each character in the string
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                int thischar = asciiBytes[i];
                int nextchar = asciiBytes.Length > (i + 1) ? asciiBytes[i + 1] : -1;

                codes.AddRange(Code128Code.CodesForChar(thischar, nextchar, ref currcs));
            }

            // calculate the check digit
            int checksum = (int)codes[0];
            for (int i = 1; i < codes.Count; i++)
            {
                checksum += i * (int)codes[i];
            }

            codes.Add(checksum % 103);

            codes.Add(Code128Code.StopCode());

            int[] result = codes.ToArray(typeof(int)) as int[];
            return result;
        }

        /// <summary>
        /// Determines the best starting code set based on the the first two 
        /// characters of the string to be encoded
        /// </summary>
        /// <param name="csa1">First character of input string</param>
        /// <param name="csa2">Second character of input string</param>
        /// <returns>The codeset determined to be best to start with</returns>
        private CodeSet GetBestStartSet(Code128Code.CodeSetAllowed csa1, Code128Code.CodeSetAllowed csa2)
        {
            int vote = 0;

            vote += (csa1 == Code128Code.CodeSetAllowed.CodeA) ? 1 : 0;
            vote += (csa1 == Code128Code.CodeSetAllowed.CodeB) ? -1 : 0;
            vote += (csa2 == Code128Code.CodeSetAllowed.CodeA) ? 1 : 0;
            vote += (csa2 == Code128Code.CodeSetAllowed.CodeB) ? -1 : 0;

            return (vote > 0) ? CodeSet.CodeA : CodeSet.CodeB; // ties go to codeB due to my own prejudices
        }
    }
}
