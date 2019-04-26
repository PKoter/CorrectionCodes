using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CorrectionCodes.Models;

namespace CorrectionCodes.Components
{
	public sealed class FlipBitCommand : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (!(parameter is BitModel bitModel))
				return;

			bitModel.FlipBit();
		}

		public event EventHandler CanExecuteChanged;
	}
}
