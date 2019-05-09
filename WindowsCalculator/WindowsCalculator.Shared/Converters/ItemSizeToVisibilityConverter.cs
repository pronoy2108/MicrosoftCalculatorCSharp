using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;
using Windows.UI.Xaml.Interop;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Data;

namespace WindowsCalculator.Shared.Converters
{
	[WebHostHidden]
	sealed public class ItemSizeToVisibilityConverter : IValueConverter
	{
		public virtual Object Convert(Object value, TypeName targetType, Object parameter, String language)
		{
			var items = (int)value;
			var boolValue = items == 0;

			return BooleanToVisibilityConverter.Convert(boolValue);
		}

		public virtual Object ConvertBack(Object value, TypeName targetType, Object parameter, String language)
		{
			throw new NotImplementedException();
		}

	}

	public class ItemSizeToVisibilityNegationConverter : IValueConverter
	{
		public virtual Object Convert(Object value, TypeName targetType, Object parameter, String language)
		{
			var items = (int)value;
			var boolValue = items > 0;

			return BooleanToVisibilityConverter.Convert(boolValue);
		}
		public virtual Object ConvertBack(Object value, TypeName targetType, Object parameter, String language)
		{
			throw new NotImplementedException();
		}
	}
}