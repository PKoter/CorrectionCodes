using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CorrectionCodes.Core;
using CorrectionCodes.Models;
using JetBrains.Annotations;

namespace CorrectionCodes
{
	public sealed class MainController : INotifyPropertyChanged
	{
		private Random _random;
		private byte[] _originalBits;
		private ICorrectionCode _codeAlgo;

		public string TextData { get; set; }
		public int?   TextLength => TextData?.Length;
		public byte[] TransmittedBits { get; private set; }
		public bool[] ModifiedBits { get; private set; }
		public BitData DataModel { get; private set; }


		public int GeneratedTextLength { get; set; } = 1;
		public int GeneratedErrorCount { get; set; } = 0;
		public bool GenerateTextData { get; set; } = true;
		public bool GenerateTransmissionErrors { get; set; } = true;

		public bool? IncorrectTransmission{ get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public MainController([NotNull] ICorrectionCode codeAlgorithm, [NotNull] Random random)
		{
			_random = random;
			_codeAlgo = codeAlgorithm ?? throw new ArgumentNullException();
		}

		public void GenerateText()
		{
			var chars = Enumerable.Repeat(1, GeneratedTextLength).Select(i => (char)_random.Next('a', 'z' + 1));
			TextData = string.Concat(chars);

			ConvertTextToBits();
		}

		public void ConvertTextToBits()
		{
			if (string.IsNullOrEmpty(TextData))
				return;

			var textBytes = Encoding.ASCII.GetBytes(TextData);
			_originalBits = textBytes.SelectMany(b => b.ToBits()).ToArray();
			ComputeCode();
			IncorrectTransmission = false;
		}

		private void ComputeCode()
		{
			byte[] crcBits = null;
			if (_codeAlgo is IByteBasedCode byteBasedCode)
			{
				var bytes = byteBasedCode.ComputeCode(_originalBits.ConvertToBytes());
				crcBits = bytes.ConvertToBits();
			}
			else if(_codeAlgo is IBitBasedCode bitBasedCode)
			{
				crcBits = bitBasedCode.ComputeCode(_originalBits);
			}
			TransmittedBits = _originalBits.ConcatArray(crcBits);
			DataModel = new BitData(crcBits, TransmittedBits, (byte[])_originalBits.Clone());
			ModifiedBits = new bool[TransmittedBits.Length];
		}

		public void GenerateErrors()
		{
			if (_originalBits == null)
				return;

			var crcBits = DataModel.CorrectionBits;
			TransmittedBits = _originalBits.ConcatArray(crcBits);

			var bitErrors = new bool[TransmittedBits.Length];
			for (int i = 0; i < GeneratedErrorCount; i++)
			{
				int index;
				do
					index = _random.Next(TransmittedBits.Length);
				while (bitErrors[index]);

				bitErrors[index ] = true;
				TransmittedBits[index] ^= 1;
			}
			DataModel = new BitData(crcBits, TransmittedBits, DataModel.DataBits);
			ComputeErrorStatistics(DataModel);
			ModifiedBits = bitErrors;
		}

		private void ComputeErrorStatistics(BitData dataModel)
		{
			if (dataModel == null)
				return;

			_codeAlgo.DetectBitErrors(dataModel);
			IncorrectTransmission = DataModel.IncorrectTransmission;
		}

		public void OnBitModified([NotNull] BitModel obj)
		{
			ComputeErrorStatistics(DataModel);

		}
	}
}
