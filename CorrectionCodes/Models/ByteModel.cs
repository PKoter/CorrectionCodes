using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorrectionCodes.Models
{
	public class ByteModel
	{
		private BitModel[] _bits;

		public ICollection<BitModel> Bits => _bits;
	}
}
