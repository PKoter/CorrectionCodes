using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	[UsedImplicitly]
	public sealed class HammingCodes : IBitBasedCode
	{
		public bool IsContagiousData => false;

		public static byte[] Encode(byte[] rawBits)
		{
			var length = rawBits.Length;
			var parityBit = 1;
			while (length >= parityBit)
			{
				parityBit <<= 1;
				length += 1;
			}

			var transmittedBits = new byte[length];

			int to = 2;
			int from = 0;

			while (from < rawBits.Length)
			{
				if (IsPowerOfTwo(to + 1))
					to ++;

				transmittedBits[to] = rawBits[from];
				to++;
				from++;
			}

			for (int i = 1; i <= transmittedBits.Length; i <<= 1)
			{
				transmittedBits[i - 1] = XorAtAndTakeN(transmittedBits, i);
			}
			return transmittedBits;
		}

		public static byte[] Decode(byte[] rawBits, int dataLength)
		{
			var decoded = new byte[dataLength];

			int i = 2;
			int j = 0;
			while (j < decoded.Length)
			{
				if (IsPowerOfTwo(i + 1)) 
					i++;

				decoded[j] = rawBits[i];
				i++;
				j++;
			}
			return decoded;
		}


		private static bool IsPowerOfTwo(int n)
		{
			return Convert.ToString(n, 2).Count(d => d == '1') == 1;
		}

		/*
		public static List<int> GetPositionsForXoring(int length, int currentHammingPosition)
		{
			var positions = new List<int>();
			for (int i = 1; i <= length; i++)
			{
				if ((i & currentHammingPosition) > 0 && !IsPowerOfTwo(i))
					positions.Add(i);

			}
			return positions;
		}

		public byte DoXoringForPosition(byte[] bits, int currentHammingPosition)
		{
			return GetPositionsForXoring(bits.Length, currentHammingPosition)
				   .Select(x => bits[x - 1])
				   .Aggregate((x, y) => (byte)(x ^ y));
		}*/

		public static byte XorAtAndTakeN(byte[] bits, int n)
		{
			int take = n - 1;
			return bits.Where((b, i) =>
					   {
						   if (i == n - 1)
							   return false;
						   if (i >= take && i < take + n)
							   return true;
						   if (i == take + n)
							   take += n * 2;
						   return false;
					   })
					   .Aggregate((x, y) => (byte)(x ^ y));
		}

		public static int ErrorSyndrome(byte[] bits)
		{
			int syndrome = 0;
			int shifts = 0;
			for (int i = 1; i <= bits.Length; i *= 2)
			{
				syndrome += (XorAtAndTakeN(bits, i) ^ bits[i - 1]) << shifts;
				shifts++;
			}
			return syndrome;
		}

		public void DetectBitErrors(BitData transmittedData)
		{
			var transmitted = transmittedData.TransmittedBits;
			var syndrome = ErrorSyndrome(transmitted);
			if (syndrome == 0)
				transmittedData.IncorrectTransmission = false;
			else if (syndrome <= transmitted.Length)
			{
				transmittedData.FixedBitErrorIndexes = new int[] { syndrome - 1 };
			}
			else
				transmittedData.IncorrectTransmission = true;
		}

		public byte[] ComputeCode(byte[] rawBits)
		{
			return Encode(rawBits);
		}
	}



	public class Program
	{
		public const int startWith = 2;
		static int length;

		static bool[] Encode(bool[] code)
		{
			var encoded = new bool[length];

			int i = startWith, j = 0;
			while (i < length)
			{
				if (i == 3 || i == 7)
					i++;

				encoded[i] = code[j];

				i++;
				j++;
			}

			encoded[0] = Helpers.doXoringForPosition(encoded, length, 1);
			encoded[1] = Helpers.doXoringForPosition(encoded, length, 2);
			encoded[3] = Helpers.doXoringForPosition(encoded, length, 4);
			if (length > 7)
				encoded[7] = Helpers.doXoringForPosition(encoded, length, 8);

			return encoded;
		}

		static bool[] Decode(bool[] encoded, int orgSize)
		{
			var decoded = new bool[orgSize];

			int i = startWith, j = 0;
			while (i < length)
			{
				if (i == 3 || i == 7) 
					i++;

				decoded[j] = encoded[i];

				i++;
				j++;
			}

			return decoded;
		}

		static int ErrorSyndrome(bool[] encoded)
		{
			int syndrome =
				(Convert.ToInt32(Helpers.doXoringForPosition(encoded, length, 1) ^ encoded[0])) +
				(Convert.ToInt32(Helpers.doXoringForPosition(encoded, length, 2) ^ encoded[1]) << 1) +
				(Convert.ToInt32(Helpers.doXoringForPosition(encoded, length, 4) ^ encoded[3]) << 2);
			if (length > 7) syndrome +=
			   (Convert.ToInt32(Helpers.doXoringForPosition(encoded, length, 8) ^ encoded[7]) << 3);

			return syndrome;
		}

		static void MixinSingleError(bool[] encoded, int pos)
		{
			encoded[pos - 1] = !encoded[pos - 1];
		}

		public static void Main(string[] args)
		{
			length = 12;
			int errorPosition = 5;
			string codeString = "10011010";//"01010101111";

			var code = Helpers.prettyStringToBoolArray(codeString);
			var encoded = Encode(code);
			
			//Console.WriteLine(Helpers.boolArrayToPrettyString(code));
			Console.WriteLine(Helpers.boolArrayToPrettyString(encoded));

			MixinSingleError(encoded, errorPosition);
			//Console.WriteLine(Helpers.boolArrayToPrettyString(encoded));

			Console.WriteLine(ErrorSyndrome(encoded));
			encoded[errorPosition-1] = !encoded[errorPosition-1];
			Console.WriteLine(ErrorSyndrome(encoded));

			var decoded = Decode(encoded, code.Length);
			//Console.WriteLine(Helpers.boolArrayToPrettyString(decoded));

			Console.WriteLine(Enumerable.SequenceEqual(code, decoded));
		}
	}

	public class Helpers
	{

		public static String boolArrayToPrettyString(bool[] arr)
		{
			return String.Join("", arr.Select(x => Convert.ToInt32(x)));
		}

		public static bool[] prettyStringToBoolArray(String s)
		{
			return s.ToArray().Select(x => ((Convert.ToInt32(x) - 48) > 0)).ToArray();
		}

		public static bool notPowerOf2(int x)
		{
			return !(x == 1 || x == 2 || x == 4 || x == 8);
		}

		public static int[] getPositionsForXoring(int length, int currentHammingPosition)
		{
			var positions = new List<int>();
			for (int i = 1; i <= length; i++)
			{
				if ((i & currentHammingPosition) > 0 && notPowerOf2(i))
					positions.Add(i);

			}
			return positions.ToArray();
		}

		public static bool doXoringForPosition(bool[] vector, int length, int currentHammingPosition)
		{
			return getPositionsForXoring(length, currentHammingPosition)
				.Select(x => vector[x - 1])
				.Aggregate((x, y) => x ^ y);
		}
	}
}
