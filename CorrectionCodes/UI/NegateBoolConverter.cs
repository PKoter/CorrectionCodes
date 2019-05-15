using System;
using System.Globalization;
using System.Windows.Data;

namespace CorrectionCodes.UI
{
	public sealed class NegateBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool b ? !b : value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
				return !b;

			return value;
		}
	}
}
