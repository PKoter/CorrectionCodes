using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public sealed class BitData
	{
		[NotNull] public byte[] CorrectionBits { get; set; }
		[NotNull] public byte[] TransmittedBits { get; private set; }

		public bool[] DetectedBitErrors { get; set; }
		public byte[] FixedTransmittedBits { get; set; }


		public BitData(byte[] correctionBits, byte[] transmittedBits)
		{
			CorrectionBits = correctionBits;
			TransmittedBits = transmittedBits;
		}
	}
}
