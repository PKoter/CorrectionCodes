using System;
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
}
