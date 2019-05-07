using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorrectionCodes.Core;
using JetBrains.Annotations;

namespace CorrectionCodes.Models
{
	[UsedImplicitly]
	public sealed class ByteModel
	{
		private BitModel[] _bits;

		public ICollection<BitModel> Bits => _bits;
		public int Index { get; set; }

		public char Tooltip => (char)_bits.Select(b => (byte)b.NumericValue).ToArray().ToByte();

		public ByteModel(BitModel[] bits) 
		{
			_bits = bits;
		}
	}
}
