using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CorrectionCodes.Models
{
	public sealed class BitModel : INotifyPropertyChanged
	{
		private int _numericValue;

		public int NumericValue
		{
			get => _numericValue;
			set
			{
				if (value != _numericValue)
					Modified = !Modified;

				_numericValue = value;
			}
		}

		public bool Bool
		{
			get => NumericValue == 1; 
			//set => NumericValue = value ? 1 : 0;
		}

		public bool Modified { get; set; }

		public BitModel(int numericValue) 
		{
			_numericValue = numericValue;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void FlipBit()
		{
			NumericValue = Bool ? 0 : 1;
		}
	}
}
