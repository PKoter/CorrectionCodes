using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace CorrectionCodes.UI
{
	public sealed class TextDelayedInput : TextBox
	{
		private DispatcherTimer _filterInvoke;
		private string _lastValue;

		public event Action<TextDelayedInput, string> DelayedTextChanged;

		public TextDelayedInput()
		{
			_filterInvoke = new DispatcherTimer()
			{
				Interval = TimeSpan.FromMilliseconds(1000),
				IsEnabled = false
			};
			_filterInvoke.Tick += (s, a) =>
				{
					var text = Text;
					if (! string.Equals(_lastValue, text))
					{
						_filterInvoke.Stop();
						DelayedTextChanged?.Invoke(this, text);
						_lastValue = text;
					}
				};
		}

		[NotNull] public string LastValue => _lastValue ?? "";

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (!_filterInvoke.IsEnabled)
				_filterInvoke.Start();

			base.OnTextChanged(e);
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			if (!_filterInvoke.IsEnabled)
				_filterInvoke.Start();

			base.OnPreviewTextInput(e);
		}
	}
}
