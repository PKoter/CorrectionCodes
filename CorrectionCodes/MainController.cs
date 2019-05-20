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
		private bool _notContagiousData;

		public string TextData { get; set; }
		public int?   TextLength => TextData?.Length;
		public byte[] TransmittedBits { get; private set; }
		public bool[] ModifiedBits { get; private set; }
		public int[] FixedBits { get; private set; }
		public BitData DataModel { get; private set; }


		public int GeneratedTextLength { get; set; } = 1;
		public int GeneratedErrorCount { get; set; } = 0;
		public bool GenerateTextData   { get; set; } = true;
		public bool GenerateTransmissionErrors { get; set; } = true;
		public bool DetectedErrorsAreWrong { get; set; }
		public bool WrongTransmissionEvaluation { get; set; }

		public bool? IncorrectTransmission { get; private set; }
		public int FixedBitErrorsCount     { get; set; }
		public int MissedBitErrorsCount    { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public MainController([NotNull] ICorrectionCode codeAlgorithm, [NotNull] Random random)
		{
			_random = random;
			_codeAlgo = codeAlgorithm ?? throw new ArgumentNullException();
			_notContagiousData = _codeAlgo is IBitBasedCode bitBased && !bitBased.IsContagiousData;
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
			if (_notContagiousData)
			{
				TransmittedBits  = crcBits;
				var emptyCrcBits = new byte[crcBits.Length - _originalBits.Length];
				DataModel        = new BitData(emptyCrcBits, crcBits, _originalBits, _notContagiousData);
				_originalBits    = (byte[])TransmittedBits.Clone();
			}
			else
			{
				TransmittedBits = _originalBits.ConcatArray(crcBits);
				DataModel       = new BitData(crcBits, TransmittedBits, _originalBits);
			}
			ModifiedBits = new bool[TransmittedBits.Length];
		}

		public void GenerateErrors()
		{
			if (_originalBits == null)
				return;

			var crcBits = DataModel.CorrectionBits;
			if(_notContagiousData)
				TransmittedBits = (byte[])_originalBits.Clone();
			else
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
			DataModel = new BitData(crcBits, TransmittedBits, DataModel.DataBits, _notContagiousData);
			ModifiedBits = bitErrors;
			ComputeErrorStatistics(DataModel);
		}

		private void ComputeErrorStatistics(BitData dataModel)
		{
			if (dataModel == null)
				return;

			dataModel.FixedBitErrorIndexes = null;
			_codeAlgo.DetectBitErrors(dataModel);
			IncorrectTransmission = DataModel.IncorrectTransmission;
			FixedBitErrorsCount = DataModel.FixedBitCount;

			var missedErrors = ModifiedBits.Count(b => b)- DataModel.FixedBitCount;
			if (DataModel.FixedBitCount > 0)
			{
				var wronglyDetected = DataModel.FixedBitErrorIndexes.Select(i => ModifiedBits[i]).Count(m => m == false);
				missedErrors += wronglyDetected;
				DetectedErrorsAreWrong = wronglyDetected > 0;
			}
			else
				DetectedErrorsAreWrong = false;

			MissedBitErrorsCount = missedErrors;
			WrongTransmissionEvaluation = MissedBitErrorsCount > 0 && IncorrectTransmission != true;
			FixedBits = dataModel.FixedBitErrorIndexes;
		}

		public void OnBitModified([NotNull] BitModel obj)
		{
			ComputeErrorStatistics(DataModel);

		}
	}
}
