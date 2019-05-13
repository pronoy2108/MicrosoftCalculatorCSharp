using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;

namespace CalculatorApp.Converters
{
	class ExpressionItemContainerStyle : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			return value;
		}

		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			return value;
		}
	}
}
