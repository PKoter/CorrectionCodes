using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	public interface ICorrectionCode
	{
		/// <summary>
		/// Returns redundant correction bits produced from given data bits
		/// </summary>
		/// <param name="rawBits"></param>
		/// <returns></returns>
		[NotNull] byte[] ComputeCorrectionCode([NotNull] byte[] rawBits);

		/// <summary>
		/// Sets DetectedBitErrors and FixedTransmittedBits at indexes coresponding to transmitted bit indexes.
		/// </summary>
		/// <param name="transmittedData"></param>
		/// <returns></returns>
		[NotNull] BitData DetectBitErrors([NotNull] BitData transmittedData);
	}
}
