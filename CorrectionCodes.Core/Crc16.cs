using System;

namespace CorrectionCodes.Core
{
	public sealed class Crc16 : IByteBasedCode
	{
		private const  ushort   polynomial = 0xA001;
		private static readonly ushort[] _table = new ushort[256];

		static Crc16()
		{
			for (ushort i = 0; i < _table.Length; i++)
			{
				ushort value = 0;
				ushort temp = i;
				for (byte j = 0; j < 8; ++j)
				{
					if (((value ^ temp) & 0x0001) != 0)
					{
						value = (ushort)((value >> 1) ^ polynomial);
					}
					else
					{
						value >>= 1;
					}
					temp >>= 1;
				}
				_table[i] = value;
			}
		}

		public static ushort ComputeChecksum(byte[] bytes)
		{
			ushort crc = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				byte index = (byte)(crc ^ bytes[i]);
				crc = (ushort)((crc >> 8) ^ _table[index]);
			}

			return crc;
		}

		public byte[] ComputeCode(byte[] rawBytes)
		{
			var crc = ComputeChecksum(rawBytes);
			return BitConverter.GetBytes(crc);
		}

		public void DetectBitErrors(BitData transmittedData)
		{
			var payloadBytes = transmittedData.TramsmittedBytes;
			var crc = ComputeChecksum(payloadBytes);

			transmittedData.IncorrectTransmission = crc != 0;
		}
	}
}
