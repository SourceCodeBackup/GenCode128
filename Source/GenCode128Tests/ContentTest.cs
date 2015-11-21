using System;
using NUnit.Framework;

using GenCode128;

namespace GenCode128Tests
{
	/// <summary>
	/// Summary description for Content.
	/// </summary>
	[TestFixture]
	public class ContentTests
	{
      private const int cSHIFT = 98;
      private const int cCODEA = 101;
      private const int cCODEB = 100;

      [Test]
      public void CharRangeTests()
      {
         Assert.AreEqual(Code128Code.CodeSetAllowed.CodeAorB, Code128Code.CodesetAllowedForChar(66), "Incorrect codeset requirement returned");
         Assert.AreEqual(Code128Code.CodeSetAllowed.CodeA, Code128Code.CodesetAllowedForChar(17), "Incorrect codeset requirement returned");
         Assert.AreEqual(Code128Code.CodeSetAllowed.CodeB, Code128Code.CodesetAllowedForChar(110), "Incorrect codeset requirement returned");
      }

      [Test]
      public void CharTranslationTests()
      {
         CodeSet currcs,origcs;
         int thischar,nextchar;
         int[] resultcodes;

         // in CodeA, thischar Either, nextchar Either
         thischar = 66; // "B"
         nextchar = 66;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeA, nextchar Either
         thischar = 1; // "^A"
         nextchar = 66;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(65,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeB, nextchar Either
         thischar = 110; // "n"
         nextchar = 66;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(78,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeA, thischar Either, nextchar -1
         thischar = 66; // "B"
         nextchar = -1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeA, nextchar -1
         thischar = 1; // "^A"
         nextchar = -1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(65,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeB, nextchar -1
         thischar = 110; // "n"
         nextchar = -1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(78,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeA, thischar Either, nextchar CodeA
         thischar = 66; // "B"
         nextchar = 1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeA, nextchar CodeA
         thischar = 1; // "^A"
         nextchar = 1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(65,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeB, nextchar CodeA
         thischar = 110; // "n"
         nextchar = 1;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(78,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeA, thischar Either, nextchar CodeB
         thischar = 66; // "B"
         nextchar = 110;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeA, nextchar CodeB
         thischar = 1; // "^A"
         nextchar = 110;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(65,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeA, thischar CodeB, nextchar CodeB
         thischar = 110; // "n"
         nextchar = 110;
         currcs = CodeSet.CodeA;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cCODEB,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(78,resultcodes[1],"Incorrect code returned");
         Assert.AreNotEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeB, thischar Either, nextchar Either
         thischar = 66; // "B"
         nextchar = 66;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeA, nextchar Either
         thischar = 1; // "^A"
         nextchar = 66;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(65,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeB, nextchar Either
         thischar = 110; // "n"
         nextchar = 66;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(78,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeB, thischar Either, nextchar -1
         thischar = 66; // "B"
         nextchar = -1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeA, nextchar -1
         thischar = 1; // "^A"
         nextchar = -1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(65,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeB, nextchar -1
         thischar = 110; // "n"
         nextchar = -1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(78,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeB, thischar Either, nextchar CodeA
         thischar = 66; // "B"
         nextchar = 1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeA, nextchar CodeA
         thischar = 1; // "^A"
         nextchar = 1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cCODEA,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(65,resultcodes[1],"Incorrect code returned");
         Assert.AreNotEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeB, nextchar CodeA
         thischar = 110; // "n"
         nextchar = 1;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(78,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");



         // in CodeB, thischar Either, nextchar CodeB
         thischar = 66; // "B"
         nextchar = 110;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(34,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeA, nextchar CodeB
         thischar = 1; // "^A"
         nextchar = 110;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(2,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(cSHIFT,resultcodes[0],"Incorrect code returned"); 
         Assert.AreEqual(65,resultcodes[1],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");

         // in CodeB, thischar CodeB, nextchar CodeB
         thischar = 110; // "n"
         nextchar = 110;
         currcs = CodeSet.CodeB;
         origcs = currcs;
         resultcodes = Code128Code.CodesForChar(thischar,nextchar, ref currcs);
         Assert.IsNotNull(resultcodes, "No codes returned");
         Assert.AreEqual(1,resultcodes.Length, "Incorrect number of codes returned");
         Assert.AreEqual(78,resultcodes[0],"Incorrect code returned");
         Assert.AreEqual(origcs,currcs,"Incorrect code set returned");


      }

      [Test]
      public void CharCompatibilityTests()
      {
         CodeSet currcs;
         int thischar;

         thischar = 66; // "B"
         currcs = CodeSet.CodeA;
         Assert.AreEqual(true, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");

         thischar = 66; // "B"
         currcs = CodeSet.CodeB;
         Assert.AreEqual(true, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");

         thischar = 17; // "^Q"
         currcs = CodeSet.CodeA;
         Assert.AreEqual(true, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");

         thischar = 17; // "^Q"
         currcs = CodeSet.CodeB;
         Assert.AreEqual(false, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");

         thischar = 110; // "n"
         currcs = CodeSet.CodeA;
         Assert.AreEqual(false, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");

         thischar = 110; // "n"
         currcs = CodeSet.CodeB;
         Assert.AreEqual(true, Code128Code.CharCompatibleWithCodeset(thischar,currcs), "Compat test failed");
      }

      [Test]
      public void CharValueTranslationTests()
      {
         Assert.AreEqual(0, Code128Code.CodeValueForChar(32),"Code translation wrong");
         Assert.AreEqual(31,Code128Code.CodeValueForChar(63),"Code translation wrong");
         Assert.AreEqual(32,Code128Code.CodeValueForChar(64),"Code translation wrong");
         Assert.AreEqual(63,Code128Code.CodeValueForChar(95),"Code translation wrong");
         Assert.AreEqual(64,Code128Code.CodeValueForChar(96),"Code translation wrong");
         Assert.AreEqual(64,Code128Code.CodeValueForChar(0), "Code translation wrong");
         Assert.AreEqual(95,Code128Code.CodeValueForChar(31),"Code translation wrong");
      }

      [Test]
      public void FullStringTest()
      {
         Code128Content content = new Code128Content("BarCode 1");
         int[] result = content.Codes;
         Assert.AreEqual( 12, result.Length, "Wrong number of code values in result" );
         Assert.AreEqual( 104, result[0], "Start code wrong" );
         Assert.AreEqual( 34, result[1], "Code value #1 wrong" );
         Assert.AreEqual( 65, result[2], "Code value #2 wrong" );
         Assert.AreEqual( 82, result[3], "Code value #3 wrong" );
         Assert.AreEqual( 35, result[4], "Code value #4 wrong" );
         Assert.AreEqual( 79, result[5], "Code value #5 wrong" );
         Assert.AreEqual( 68, result[6], "Code value #6 wrong" );
         Assert.AreEqual( 69, result[7], "Code value #7 wrong" );
         Assert.AreEqual(  0, result[8], "Code value #8 wrong" );
         Assert.AreEqual( 17, result[9], "Code value #9 wrong" );
         Assert.AreEqual( 33, result[10], "Checksum wrong" );
         Assert.AreEqual( 106, result[11], "Stop character wrong" );

         content = new Code128Content("\x11S12345");
         result = content.Codes;
         Assert.AreEqual( 10, result.Length, "Wrong number of code values in result" );
      }
   }
}
