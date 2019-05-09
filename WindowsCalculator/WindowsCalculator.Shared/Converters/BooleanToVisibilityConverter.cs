using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;
using Windows.UI.Xaml;

namespace WindowsCalculator.Shared.Converters
{
	class BooleanToVisibilityConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			bool? boxedBool = value as bool?;

			if (boxedBool != null)
			{
				return boxedBool.Value ? Visibility.Visible : Visibility.Collapsed;
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

	class BooleanToVisibilityNegationConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			bool? boxedBool = value as bool?;

			if (boxedBool != null)
			{
				return boxedBool.Value ? Visibility.Collapsed : Visibility.Visible;
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
