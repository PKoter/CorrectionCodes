using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PropertyChanged;

namespace CorrectionCodes.Models
{
	[UsedImplicitly]
	public sealed class BitModel : INotifyPropertyChanged
	{
		private byte[] _source;
		private bool[] _changes;
		private int _index;

		//[DoNotCheckEquality]
		public int NumericValue
		{
			get => _source[_index];
			private set
			{
				Modified = !Modified;
				_changes[_index] = Modified;
				_source[_index] = (byte)value;
			}
		}

		public bool Bool
		{
			get => NumericValue == 1; 
			//set => NumericValue = value ? 1 : 0;
		}

		public bool Modified { get; set; }
		public bool DetectedError { get; set; }

		public BitModel([NotNull] byte[] source, int index)
		{
			_index = index;
			_source = source;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void FlipBit()
		{
			NumericValue ^= 1;
			_changes[_index] = Modified;
		}

		public void SetBitChanges(bool[] changes)
		{
			_changes = changes;
			Modified = _changes[_index];
		}
	}
}
