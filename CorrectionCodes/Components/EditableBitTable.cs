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

		public static DependencyProperty DataProperty =
			DependencyProperty.Register(nameof(Data), typeof(BitData), typeof(EditableBitTable),
										new PropertyMetadata(OnDataChanged));

		private Action<BitModel> _bitModified;

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

		private BitData _lastData;

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

			var byteModels = self.ItemsSource as List<ByteModel>;
			var bitModels  = byteModels.SelectMany(bm => bm.Bits);
			foreach (var bitModel in bitModels)
			{
				bitModel.SetBitChanges(bitChanges);
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
			foreach (var bit in bits.Concat(new byte[1]))
			{
				if (i == 8)
				{
					var byteModel = new ByteModel(bitModels)
					{
						Index      = byteModels.Count,
						IsChecksum = dataBytesCount <= byteModels.Count
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
