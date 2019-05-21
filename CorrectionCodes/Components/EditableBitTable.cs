using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CorrectionCodes.Core;
using CorrectionCodes.Models;
using JetBrains.Annotations;

namespace CorrectionCodes.Components
{
	public sealed class EditableBitTable : ListBox
	{
		public ICommand FlipBitCommand { get; private set; }

		public static DependencyProperty ReadOnlyProperty =
			DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(EditableBitTable),
										new PropertyMetadata(OnReadOnlyChanged));

		public static DependencyProperty BitChangesProperty =
			DependencyProperty.Register(nameof(BitChanges), typeof(bool[]), typeof(EditableBitTable),
										new PropertyMetadata(OnBitsModified));

		public static DependencyProperty BitFixesProperty =
			DependencyProperty.Register(nameof(BitFixes), typeof(int[]), typeof(EditableBitTable),
										new PropertyMetadata(OnBitsFixed));

		public static DependencyProperty DataProperty =
			DependencyProperty.Register(nameof(Data), typeof(BitData), typeof(EditableBitTable),
										new PropertyMetadata(OnDataChanged));

		public bool ReadOnly
		{
			get => (bool)GetValue(ReadOnlyProperty);
			set => SetValue(ReadOnlyProperty, value);
		}

		public BitData Data
		{
			get => (BitData)GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}

		public bool[] BitChanges
		{
			get => (bool[])GetValue(BitChangesProperty);
			set => SetValue(BitChangesProperty, value);
		}

		public int[] BitFixes
		{
			get => (int[])GetValue(BitChangesProperty);
			set => SetValue(BitChangesProperty, value);
		}

		private Action<BitModel> _bitModified;
		private BitData _lastData;

		private IEnumerable<BitModel> BitModels =>
			(this.ItemsSource as List<ByteModel>).SelectMany(bm => bm.Bits).Where(b => b != null);

		public EditableBitTable()
		{
			FlipBitCommand = new FlipBitCommand(this);
		}

		public void RaiseBitModified(BitModel bit)
		{
			_bitModified?.Invoke(bit);
		}

		public void SetBitModifiedHandler([CanBeNull] Action<BitModel> bitModel)
		{
			_bitModified = bitModel;
		}

		private static void OnReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self    = dependencyObject as EditableBitTable;
			var command = self.FlipBitCommand as FlipBitCommand;
			command.SetCanExecute(!(bool)dpea.NewValue);
		}

		private static void OnDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self = dependencyObject as EditableBitTable;
			if (!(dpea.NewValue is BitData data))
			{
				self.ItemsSource = null;
				self._lastData   = null;
				return;
			}
			if (self._lastData != null
				&& (data == self._lastData || self._lastData.TransmittedBits == data.TransmittedBits))
				return;

			self._lastData = data;
			var byteModels = CreateModels(data);
			self.ItemsSource = byteModels;
		}

		private static void OnBitsModified(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self = dependencyObject as EditableBitTable;
			if (!(dpea.NewValue is bool[] bitChanges))
			{
				self.ItemsSource = null;
				return;
			}

			var bitModels = self.BitModels;
			foreach (var bitModel in bitModels)
			{
				bitModel.SetBitChanges(bitChanges);
			}
		}

		private static void OnBitsFixed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self = dependencyObject as EditableBitTable;
			if (!(dpea.NewValue is int[] bitFixes))
			{
				if (self.ItemsSource == null)
					return;

				foreach (var bitModel in self.BitModels)
				{
					bitModel.DetectedError = false;
				}
				return;
			}

			Array.Sort(bitFixes);
			int count = 0;
			int index = 0;
			var bitModels = self.BitModels;
			foreach (var bitModel in bitModels)
			{
				if (index == bitFixes[count])
				{
					bitModel.DetectedError = true;
					count++;
					if (count == bitFixes.Length)
						break;
				}
				else
				{
					bitModel.DetectedError = false;
				}
				index++;
			}
		}

		[NotNull]
		private static List<ByteModel> CreateModels(BitData data)
		{
			byte[] bits           = data.TransmittedBits;
			int    dataBytesCount = data.DataBitCount / 8;
			var    byteModels     = new List<ByteModel>(bits.Length / 8);

			int i         = 0;
			int index     = 0;
			var bitModels = new BitModel[8];
			//var excessLength = 1 + (data.PayloadBitCount % 8 == 0 ? 0 : 8 - data.PayloadBitCount % 8);

			foreach (var bit in bits.Concat(new byte[1]))
			{
				if (i == 8 || index >= data.PayloadBitCount)
				{
					var byteModel = new ByteModel(bitModels)
					{
						Index      = byteModels.Count,
						IsChecksum = dataBytesCount <= byteModels.Count && !data.UncontagiousData
					};
					byteModels.Add(byteModel);

					bitModels = new BitModel[8];
					i         = 0;
				}
				bitModels[i] = new BitModel(bits, index);
				i++;
				index ++;
			}
			return byteModels;
		}

	}
}
