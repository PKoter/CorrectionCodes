using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CorrectionCodes.UI
{
	public sealed class NumericTextBox : TextBox
	{
		private static readonly Regex _onlyNumbers = new Regex(@"^(0|([1-9]\d*))$");

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			var input = e.Text;
			e.Handled = !_onlyNumbers.IsMatch(input);
			base.OnPreviewTextInput(e);
		}
	}
}
