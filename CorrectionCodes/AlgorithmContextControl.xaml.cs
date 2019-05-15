using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
				this.DataContext = value;
			}
		}

		private void GenerateTextData(object sender, RoutedEventArgs e)
		{
			_controller.GenerateText();
		}

		private void OnMainGridMouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
		}

		private void OnTextInputLostFocus(object sender, RoutedEventArgs e)
		{
			_controller.ConvertTextToBits();
		}

		private void GenerateErrors(object sender, RoutedEventArgs e)
		{
			_controller.GenerateErrors();

		}
	}
}
