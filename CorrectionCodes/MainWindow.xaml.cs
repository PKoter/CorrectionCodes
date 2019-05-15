using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CorrectionCodes.Core;

namespace CorrectionCodes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Random _random;
		private AlgorithmContextControl _algorithmControl;
		private MainController                      _controller;
		private IDictionary<string, MainController> _controllers;
		private IDictionary<string, Type> _algorithmNames = new Dictionary<string, Type>()
		{
			{"CRC 16", typeof(Crc16)},
			{"CRC 32", typeof(Crc32)},
		};

		public MainWindow()
		{
			InitializeComponent();
			_random = new Random();
			_controllers = new Dictionary<string, MainController>();
			_algorithmControl = new AlgorithmContextControl();

			this.tabs.ItemsSource = _algorithmNames.Keys.Select(k => new TabItem() {Header = k}).ToList();
		}


		private void OnTabSelected(object sender, RoutedEventArgs e)
		{
			var tab = (sender as TabControl).SelectedItem as TabItem;
			var code = tab.Header as string;
			if (_controllers.TryGetValue(code, out _controller) == false)
			{
				var algoInstance = (ICorrectionCode)Activator.CreateInstance(_algorithmNames[code]);
				_controller = new MainController(algoInstance, _random);
				_controllers.Add(code, _controller);
			}

			_algorithmControl.Controller = _controller;
			tab.Content = _algorithmControl;
		}
	}
}
