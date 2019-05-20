using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public sealed class BitData
	{
		[NotNull] public byte[] CorrectionBits  { get; set; }
		[NotNull] public byte[] DataBits        { get; private set; }
		/// <summary>
		/// all bits of data and crc bits combined
		/// </summary>
		[NotNull] public byte[] TransmittedBits { get; private set; }

		[NotNull] public byte[] CorrectionBytes
		{
			get => CorrectionBits.ConvertToBytes();
			set => CorrectionBits = value.ConvertToBits();
		}

		[NotNull] public byte[] DataBytes        => DataBits.ConvertToBytes();
		[NotNull] public byte[] TramsmittedBytes => TransmittedBits.ConvertToBytes();

		public bool[] DetectedBitErrors     { get; set; }
		public int[]  FixedBitErrorIndexes  { get; set; }
		public byte[] FixedTransmittedBits  { get; set; }
		public byte[] FixedTransmittedBytes { get; set; }
		public bool   IncorrectTransmission { get; set; }
		public bool   UncontagiousData      { get; set; }

		public int DataBitCount       => DataBits.Length;
		public int CorrectionBitCount => CorrectionBits.Length;
		public int PayloadBitCount    => TransmittedBits.Length;
		public int FixedBitCount      => FixedBitErrorIndexes?.Length ?? 0;


		public BitData([NotNull] byte[] correctionBits, [NotNull] byte[] transmittedBits, byte[] dataBits, 
					   bool notContagiousData = false)
		{
			CorrectionBits = correctionBits;
			TransmittedBits = transmittedBits;
			DataBits = dataBits;
			UncontagiousData = notContagiousData;
		}

		public BitData([NotNull] byte[] correctionBits, [NotNull] byte[] dataBits)
		{
			CorrectionBits  = correctionBits;
			TransmittedBits = dataBits.ConcatArray(correctionBits);
			DataBits        = dataBits;
		}

	}
}
