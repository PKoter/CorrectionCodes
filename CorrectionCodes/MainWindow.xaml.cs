using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorrectionCodes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainController _controller;

		public MainWindow()
		{
			InitializeComponent();
			_controller = new MainController();
			this.DataContext = _controller;
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
