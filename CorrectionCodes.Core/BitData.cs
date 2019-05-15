﻿using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public sealed class BitData
	{
		[NotNull] public byte[] CorrectionBits  { get; set; }
		[NotNull] public byte[] TransmittedBits { get; private set; }

		[NotNull] public byte[] CorrectionBytes => CorrectionBits.ConvertToBytes();
		[NotNull] public byte[] TramsmittedBytes => TransmittedBits.ConvertToBytes();

		public bool[] DetectedBitErrors     { get; set; }
		public byte[] FixedTransmittedBits  { get; set; }
		public byte[] FixedTransmittedBytes { get; set; }
		public bool   IncorrectTransmission { get; set; }

		public int DataBitCount => TransmittedBits.Length;
		public int CorrectionBitCount => CorrectionBits.Length;
		public int PayloadBitCount => CorrectionBitCount + DataBitCount;


		public BitData([NotNull] byte[] correctionBits, [NotNull] byte[] transmittedBits)
		{
			CorrectionBits = correctionBits;
			TransmittedBits = transmittedBits;
		}
	}
}
