using System;
using System.Linq;

namespace CorrectionCodes.Core
{
	public sealed class Crc32 : IByteBasedCode
	{
		private static readonly uint[] _table;
		
		static Crc32()
		{
			const uint poly = 0x04C11DB7;
			_table = new uint[256];
			for (uint i = 0; i < _table.Length; i++)
			{
				uint temp = i;
				for (int j = 8; j > 0; --j)
				{
					if ((temp & 1) == 1)
					{
						temp = ((temp >> 1) ^ poly);
					}
					else
					{
						temp >>= 1;
					}
				}
				_table[i] = temp;
			}
		}

		public static uint ComputeChecksum(byte[] bytes)
		{
			uint crc = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				byte index = (byte)((crc & 0xff) ^ bytes[i]);
				crc = ((crc >> 8) ^ _table[index]);
			}
			return crc ;
		}

		public byte[] ComputeCode(byte[] rawBytes)
		{
			var crc = ComputeChecksum(rawBytes);
			return BitConverter.GetBytes(crc);
		}

		public void DetectBitErrors(BitData transmittedData)
		{
			var inputBytes = transmittedData.TramsmittedBytes;
			var crcBytes   = transmittedData.CorrectionBytes;
			var payload    = inputBytes.Concat(crcBytes).ToArray();
			var crc        = ComputeChecksum(payload);

			transmittedData.IncorrectTransmission = crc != 0;
		}
	}
}
