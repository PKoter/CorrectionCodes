using System.Windows.Controls;
using System.Windows.Input;

namespace CorrectionCodes.Components
{
	public sealed class EditableBitTable : ListBox
	{
		public ICommand FlipBitCommand { get; private set; } = new FlipBitCommand();

	}
}
