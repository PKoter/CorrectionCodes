using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CorrectionCodes.Models;
using JetBrains.Annotations;

namespace CorrectionCodes.Components
{
	public sealed class EditableBitTable : ListBox
	{
		public ICommand FlipBitCommand { get; private set; } = new FlipBitCommand();

		public static DependencyProperty ReadOnlyProperty = 
			DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(EditableBitTable), new PropertyMetadata(OnReadOnlyChanged));

		public static DependencyProperty BitsProperty = 
			DependencyProperty.Register(nameof(Bits), typeof(byte[]), typeof(EditableBitTable), new PropertyMetadata(OnBitSourceChanged));

		public static DependencyProperty BitChangesProperty = 
			DependencyProperty.Register(nameof(BitChanges), typeof(bool[]), typeof(EditableBitTable), new PropertyMetadata(OnBitsModified));

		public bool ReadOnly
		{
			get => (bool)GetValue(ReadOnlyProperty);
			set => SetValue(ReadOnlyProperty, value);
		}

		public byte[] Bits
		{
			get => (byte[])GetValue(BitsProperty);
			set => SetValue(BitsProperty, value);
		}

		public bool[] BitChanges
		{
			get => (bool[])GetValue(BitChangesProperty);
			set => SetValue(BitChangesProperty, value);
		}

		private static void OnReadOnlyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self = dependencyObject as EditableBitTable;
			var command = self.FlipBitCommand as FlipBitCommand;
			command.SetCanExecute(!(bool)dpea.NewValue);
		}

		private static void OnBitSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dpea)
		{
			var self = dependencyObject as EditableBitTable;
			if (!(dpea.NewValue is byte[] bits))
			{
				self.ItemsSource = null;
				return;
			}

			var byteModels = CreateModels(bits);
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
			var bitModels = byteModels.SelectMany(bm => bm.Bits);
			foreach (var bitModel in bitModels)
			{
				bitModel.SetBitChanges(bitChanges);
			}
		}

		[NotNull]
		private static List<ByteModel> CreateModels(byte[] bits)
		{
			var byteModels = new List<ByteModel>(bits.Length / 8);

			int       i         = 0;
			int       index     = 0;
			var       bitModels = new BitModel[8];
			ByteModel byteModel;
			foreach (var bit in bits)
			{
				if (i == 8)
				{
					byteModel = new ByteModel(bitModels) { Index = byteModels.Count };
					byteModels.Add(byteModel);

					bitModels = new BitModel[8];
					i         = 0;
				}
				bitModels[i] = new BitModel(bits, index);
				i++;
				index ++;
			}
			byteModel = new ByteModel(bitModels){ Index = byteModels.Count };
			byteModels.Add(byteModel);
			return byteModels;
		}
	}
}
