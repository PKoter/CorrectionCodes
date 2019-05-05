using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public static class Extensions
	{
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<byte[]> GroupToBytes([NotNull] this IEnumerable<byte> bits)
		{
			int i = 0;
			var byteArr = new byte[8];
			foreach (var bit in bits)
			{
				if (i == 8)
				{
					yield return byteArr;

					byteArr = new byte[8];
					i       = 0;
				}
				byteArr[i] = bit;
				i++;
			}
			yield return byteArr;
		}

		public static byte ToByte([NotNull] this byte[] bits)
		{
			int b = 0;
			for (int i = 0; i < bits.Length; i++)
			{
				b <<= 1;
				b |= bits[i];
			}
			return (byte)b;
		}


		public static IEnumerable<byte> ToBits(this byte bajt)
		{
			for (int i = 7; i >= 0; i--)
			{
				byte mask = (byte)(1 << i);
				yield return (byte)((bajt & mask) >> i);
			}
			//return Convert.ToString(@byte, 2).Select(c => c == '1' ? (byte)1 : (byte)0);
		}
	}
}
