using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CorrectionCodes.UI
{
	public sealed class SuccinctGrid : Grid
	{
		public GridLengthCollection Rows
		{
			get { return null; }
			set
			{
				var rows = value;
				if (rows == null)
					return;
				RowDefinitions.Clear();
				foreach (var length in rows.Values)
				{
					RowDefinitions.Add(new RowDefinition { Height = length });
				}
			}
		}
		
		public GridLengthCollection Columns
		{
			get { return null; }
			set
			{
				var columns = value;
				if (columns == null)
					return;
				ColumnDefinitions.Clear();
				foreach (var length in columns.Values)
				{
					ColumnDefinitions.Add(new ColumnDefinition { Width = length });
				}
			}
		}
	}

	[TypeConverter(typeof(GridLengthCollectionConverter))]
	public sealed class GridLengthCollection 
	{	
		private IEnumerable<GridLength> _values;

		public GridLengthCollection(IEnumerable<GridLength> values)
		{
			_values = values;
		}

		public IEnumerable<GridLength> Values
		{
			get { return _values; }
			set { _values = value; }
		}
	}

	public sealed class GridLengthCollectionConverter: TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
 
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
 
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string s)
				return ParseString(s);
			return base.ConvertFrom(context, culture, value);
		}
 
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is GridLengthCollection collection)
				return ToString(collection, culture);
			return base.ConvertTo(context, culture, value, destinationType);
		}
 
		private string ToString(GridLengthCollection value, CultureInfo culture)
		{
			var converter = new GridLengthConverter();
			return string.Join(",", value.Values.Select(v => converter.ConvertToString(v)));
		}
 
		private static GridLengthCollection ParseString(string s)
		{
			var converter = new GridLengthConverter();
			var lengths   = s.Split(',').Select(p => (GridLength)converter.ConvertFromString(p.Trim()));
			return new GridLengthCollection(lengths);
		}
	}
}
