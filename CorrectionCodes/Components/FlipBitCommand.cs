using System;
using System.Windows.Input;
using CorrectionCodes.Models;
using JetBrains.Annotations;

namespace CorrectionCodes.Components
{
	public sealed class FlipBitCommand : ICommand
	{
		private EditableBitTable _bitTable;
		private bool _canExecute = true;

		public FlipBitCommand([NotNull] EditableBitTable bitTable) 
		{
			_bitTable = bitTable;
		}

		public void SetCanExecute(bool canExecute)
		{
			if (_canExecute != canExecute)
			{
				_canExecute = canExecute;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public void Execute(object parameter)
		{
			if (!(parameter is BitModel bitModel))
				return;

			bitModel.FlipBit();
			_bitTable.RaiseBitModified(bitModel);
		}

		public event EventHandler CanExecuteChanged;
	}
}
