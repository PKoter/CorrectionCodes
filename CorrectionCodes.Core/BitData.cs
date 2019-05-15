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

		[NotNull] public byte[] CorrectionBytes  => CorrectionBits.ConvertToBytes();
		[NotNull] public byte[] DataBytes        => DataBits.ConvertToBytes();
		[NotNull] public byte[] TramsmittedBytes => TransmittedBits.ConvertToBytes();

		public bool[] DetectedBitErrors     { get; set; }
		public byte[] FixedTransmittedBits  { get; set; }
		public byte[] FixedTransmittedBytes { get; set; }
		public bool   IncorrectTransmission { get; set; }

		public int DataBitCount       => DataBits.Length;
		public int CorrectionBitCount => CorrectionBits.Length;
		public int PayloadBitCount    => TransmittedBits.Length;


		public BitData([NotNull] byte[] correctionBits, [NotNull] byte[] transmittedBits, byte[] dataBits)
		{
			CorrectionBits = correctionBits;
			TransmittedBits = transmittedBits;
			DataBits = dataBits;
		}
	}
}
