using System;
using System.Text;

namespace GenCode128
{
   public enum CodeSet
   {
      CodeA
      ,CodeB
      // ,CodeC   // not supported
   }

	/// <summary>
	/// Represent the set of code values to be output into barcode form
	/// </summary>
	public class Code128Content
	{
      private int[] mCodeList;

      /// <summary>
      /// Create content based on a string of ASCII data
      /// </summary>
      /// <param name="AsciiData">the string that should be represented</param>
		public Code128Content( string AsciiData )
		{
         mCodeList = StringToCode128(AsciiData);
		}

      /// <summary>
      /// Provides the Code128 code values representing the object's string
      /// </summary>
      public int[] Codes
      {
         get
         {
            return mCodeList;
         }
      }

      /// <summary>
      /// Transform the string into integers representing the Code128 codes
      /// necessary to represent it
      /// </summary>
      /// <param name="AsciiData">String to be encoded</param>
      /// <returns>Code128 representation</returns>
      private int[] StringToCode128( string AsciiData )
      {
         // turn the string into ascii byte data
         byte[] asciiBytes = Encoding.ASCII.GetBytes( AsciiData );

         // decide which codeset to start with
         Code128Code.CodeSetAllowed csa1 = asciiBytes.Length>0 ? Code128Code.CodesetAllowedForChar( asciiBytes[0] ) : Code128Code.CodeSetAllowed.CodeAorB;
         Code128Code.CodeSetAllowed csa2 = asciiBytes.Length>0 ? Code128Code.CodesetAllowedForChar( asciiBytes[1] ) : Code128Code.CodeSetAllowed.CodeAorB;
         CodeSet currcs = GetBestStartSet(csa1,csa2);

         // set up the beginning of the barcode
         System.Collections.ArrayList codes = new System.Collections.ArrayList(asciiBytes.Length + 3); // assume no codeset changes, account for start, checksum, and stop
         codes.Add(Code128Code.StartCodeForCodeSet(currcs));

         // add the codes for each character in the string
         for (int i = 0; i < asciiBytes.Length; i++)
         {
            int thischar = asciiBytes[i];
            int nextchar = asciiBytes.Length>(i+1) ? asciiBytes[i+1] : -1;

            codes.AddRange( Code128Code.CodesForChar(thischar, nextchar, ref currcs) );
         }

         // calculate the check digit
         int checksum = (int)(codes[0]);
         for (int i = 1; i < codes.Count; i++)
         {
            checksum += i * (int)(codes[i]);
         }
         codes.Add( checksum % 103 );

         codes.Add( Code128Code.StopCode() );

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
      private CodeSet GetBestStartSet( Code128Code.CodeSetAllowed csa1, Code128Code.CodeSetAllowed csa2 )
      {
         int vote = 0;

         vote += (csa1==Code128Code.CodeSetAllowed.CodeA) ?  1 : 0;
         vote += (csa1==Code128Code.CodeSetAllowed.CodeB) ? -1 : 0;
         vote += (csa2==Code128Code.CodeSetAllowed.CodeA) ?  1 : 0;
         vote += (csa2==Code128Code.CodeSetAllowed.CodeB) ? -1 : 0;

         return (vote>0) ? CodeSet.CodeA : CodeSet.CodeB;   // ties go to codeB due to my own prejudices
      }
	}

   /// <summary>
   /// Static tools for determining codes for individual characters in the content
   /// </summary>
   public static class Code128Code
   {
      #region Constants

      private const int cSHIFT = 98;
      private const int cCODEA = 101;
      private const int cCODEB = 100;

      private const int cSTARTA = 103;
      private const int cSTARTB = 104;
      private const int cSTOP   = 106;

      #endregion

      /// <summary>
      /// Get the Code128 code value(s) to represent an ASCII character, with 
      /// optional look-ahead for length optimization
      /// </summary>
      /// <param name="CharAscii">The ASCII value of the character to translate</param>
      /// <param name="LookAheadAscii">The next character in sequence (or -1 if none)</param>
      /// <param name="CurrCodeSet">The current codeset, that the returned codes need to follow;
      /// if the returned codes change that, then this value will be changed to reflect it</param>
      /// <returns>An array of integers representing the codes that need to be output to produce the 
      /// given character</returns>
      public static int[] CodesForChar(int CharAscii, int LookAheadAscii, ref CodeSet CurrCodeSet)
      {
         int[] result;
         int shifter = -1;

         if (!CharCompatibleWithCodeset(CharAscii,CurrCodeSet))
         {
            // if we have a lookahead character AND if the next character is ALSO not compatible
            if ( (LookAheadAscii != -1)  && !CharCompatibleWithCodeset(LookAheadAscii,CurrCodeSet)  ) 
            {
               // we need to switch code sets
               switch (CurrCodeSet)
               {
                  case CodeSet.CodeA:
                     shifter = cCODEB;
                     CurrCodeSet = CodeSet.CodeB;
                     break;
                  case CodeSet.CodeB:
                     shifter = cCODEA;
                     CurrCodeSet = CodeSet.CodeA;
                     break;
               }
            }
            else
            {
               // no need to switch code sets, a temporary SHIFT will suffice
               shifter = cSHIFT;
            }
         }

         if (shifter!=-1)
         {
            result = new int[2];
            result[0] = shifter;
            result[1] = CodeValueForChar(CharAscii);
         }
         else
         {
            result = new int[1];
            result[0] = CodeValueForChar(CharAscii);
         }

         return result;
      }

      /// <summary>
      /// Tells us which codesets a given character value is allowed in
      /// </summary>
      /// <param name="CharAscii">ASCII value of character to look at</param>
      /// <returns>Which codeset(s) can be used to represent this character</returns>
      public static CodeSetAllowed CodesetAllowedForChar(int CharAscii)
      {
         if (CharAscii>=32 && CharAscii<=95)
         {
            return CodeSetAllowed.CodeAorB;
         }
         else 
         {
            return (CharAscii<32) ? CodeSetAllowed.CodeA : CodeSetAllowed.CodeB;
         }
      }

      /// <summary>
      /// Determine if a character can be represented in a given codeset
      /// </summary>
      /// <param name="CharAscii">character to check for</param>
      /// <param name="currcs">codeset context to test</param>
      /// <returns>true if the codeset contains a representation for the ASCII character</returns>
      public static bool CharCompatibleWithCodeset(int CharAscii, CodeSet currcs)
      {
         CodeSetAllowed csa = CodesetAllowedForChar(CharAscii);
         return csa==CodeSetAllowed.CodeAorB 
                  || (csa==CodeSetAllowed.CodeA && currcs==CodeSet.CodeA)
                  || (csa==CodeSetAllowed.CodeB && currcs==CodeSet.CodeB);
      }

      /// <summary>
      /// Gets the integer code128 code value for a character (assuming the appropriate code set)
      /// </summary>
      /// <param name="CharAscii">character to convert</param>
      /// <returns>code128 symbol value for the character</returns>
      public static int CodeValueForChar(int CharAscii)
      {
         return (CharAscii>=32) ? CharAscii-32 :  CharAscii+64;
      }

      /// <summary>
      /// Return the appropriate START code depending on the codeset we want to be in
      /// </summary>
      /// <param name="cs">The codeset you want to start in</param>
      /// <returns>The code128 code to start a barcode in that codeset</returns>
      public static int StartCodeForCodeSet(CodeSet cs)
      {
         return cs==CodeSet.CodeA ? cSTARTA : cSTARTB;
      }

      /// <summary>
      /// Return the Code128 stop code
      /// </summary>
      /// <returns>the stop code</returns>
      public static int StopCode()
      {
         return cSTOP;
      }

      /// <summary>
      /// Indicates which code sets can represent a character -- CodeA, CodeB, or either
      /// </summary>
      public enum CodeSetAllowed
      {
         CodeA,
         CodeB,
         CodeAorB
      }

   }
}
