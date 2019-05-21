using System.Linq;
using JetBrains.Annotations;

namespace CorrectionCodes.Core
{
	[UsedImplicitly]
	public sealed class ParityBit : IBitBasedCode
	{
		private int CountOnes(byte[] bits) => bits.Count(b => b == 1); 


		public void DetectBitErrors(BitData transmittedData)
		{
			//var parityBit = transmittedData.TransmittedBits.Last();
			var onesCount = CountOnes(transmittedData.TransmittedBits);
			transmittedData.IncorrectTransmission = onesCount % 2 == 1;
		}

		public byte[] ComputeCode(byte[] rawBits)
		{
			var onesCount = CountOnes(rawBits);
			return new byte[] { (byte)(onesCount % 2) };
		}

		public bool IsContagiousData => true;
	}
}
