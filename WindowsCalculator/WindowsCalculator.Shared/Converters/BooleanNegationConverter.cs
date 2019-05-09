using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;

namespace WindowsCalculator.Shared.Converters
{
	class BooleanNegationConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			bool? boxedBool = value as bool?;

			if (boxedBool != null)
			{
				return !boxedBool;
			}
			else
			{
				return value;
			}
		}

		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			return value;
		}
	}
}
