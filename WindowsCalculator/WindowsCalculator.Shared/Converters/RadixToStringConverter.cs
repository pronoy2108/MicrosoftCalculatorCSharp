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
	class RadixToStringConverter : IValueConverter
	{
		public virtual Object Convert(Object value, TypeName targetType, Object parameter, String language)
		{
			var boxedInt = (int)value;
			string convertedValue;
			var resourceLoader = AppResourceProvider.GetInstance();

			switch (boxedInt)
			{
				case RADIX_TYPE.BIN_RADIX:
					convertedValue = resourceLoader.GetResourceString("Bin");
					break;
				case RADIX_TYPE.OCT_RADIX:
					convertedValue = resourceLoader.GetResourceString("Oct");
					break;
				case RADIX_TYPE.DEC_RADIX:
					convertedValue = resourceLoader.GetResourceString("Dec");
					break;
				case RADIX_TYPE.HEX_RADIX:
					convertedValue = resourceLoader.GetResourceString("Hex");
					break;
				default:
					break;

			};

			return convertedValue;
		}

		public virtual Object ConvertBack(Object value, TypeName targetType, Object parameter, String language)
		{
			throw new NotImplementedException();
		}
	}
}