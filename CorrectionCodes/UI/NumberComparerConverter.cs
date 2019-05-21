using System;
using System.Globalization;
using System.Windows.Data;

namespace CorrectionCodes.UI
{
	public class NumberComparerConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int n)
				return n > 0;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
