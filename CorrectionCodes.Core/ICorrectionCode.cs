using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public interface ICorrectionCode
	{
		/// <summary>
		/// Sets DetectedBitErrors and FixedTransmittedBits at indexes coresponding to transmitted bit indexes.
		/// </summary>
		/// <param name="transmittedData"></param>
		/// <returns></returns>
		void DetectBitErrors([NotNull] BitData transmittedData);
	}

	public interface IBitBasedCode : ICorrectionCode
	{
		/// <summary>
		/// Returns redundant correction bits produced from given data bits
		/// </summary>
		/// <param name="rawBits"></param>
		/// <returns></returns>
		[NotNull] byte[] ComputeCode([NotNull] byte[] rawBits);

		bool IsContagiousData { get; }
	}

	public interface IByteBasedCode : ICorrectionCode
	{
		/// <summary>
		/// Returns redundant correction bytes produced from given data bytes
		/// </summary>
		/// <param name="rawBytes"></param>
		/// <returns></returns>
		[NotNull] byte[] ComputeCode([NotNull] byte[] rawBytes);
	}
}
