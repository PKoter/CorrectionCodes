using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CorrectionCodes.UI;

namespace CorrectionCodes
{
	/// <summary>
	/// Interaction logic for AlgorithmContextControl.xaml
	/// </summary>
	public partial class AlgorithmContextControl : UserControl
	{
		private MainController _controller;

		public AlgorithmContextControl()
		{
			InitializeComponent();
		
		}

		public MainController Controller
		{
			get => _controller;
			set
			{
				_controller = value;
				this.errBitTable.SetBitModifiedHandler(_controller.OnBitModified);
				this.DataContext = value;
			}
		}

		private void GenerateTextData(object sender, RoutedEventArgs e)
		{
			_controller.GenerateText();
		}

		private void OnMainGridMouseDown(object sender, MouseButtonEventArgs e)
		{
			//Keyboard.ClearFocus();
			//this.Focus();
			//var focusRequest = new TraversalRequest(FocusNavigationDirection.First);
			//(Keyboard.FocusedElement as UIElement)?.MoveFocus(focusRequest);
		}

		private void OnTextInputLostFocus(TextDelayedInput input, string newText)
		{
			_controller.ConvertTextToBits();
		}

		private void GenerateErrors(object sender, RoutedEventArgs e)
		{
			_controller.GenerateErrors();

		}
	}
}
