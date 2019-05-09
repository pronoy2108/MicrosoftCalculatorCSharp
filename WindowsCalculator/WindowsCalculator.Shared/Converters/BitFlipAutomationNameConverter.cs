using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.UI.Converters;

namespace WindowsCalculator.Shared.Converters
{
	class BitFlipAutomationNameConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			var resourceLoader = AppResourceProvider.GetInstance();

			// initialising the updated display with 64 bits of zeros
			string updatedBinaryDisplay(64, L'0');

			const localizationSettings = LocalizationSettings.GetInstance();
			char ch0 = localizationSettings.GetDigitSymbolFromEnUsDigit('0');
			char ch1 = localizationSettings.GetDigitSymbolFromEnUsDigit('1');

			string indexName = resourceLoader.GetResourceString(parameter.ToString());
			string bitName = resourceLoader.GetResourceString("BitAutomationName");
			string valueName = resourceLoader.GetResourceString("ValueAutomationName");
			string zero = resourceLoader.GetResourceString("BinaryZeroValueAutomationName");
			string one = resourceLoader.GetResourceString("BinaryOneValueAutomationName");

			if ((value != null) && (parameter != null))
			{
				string binaryDisplay = ((string)value).Data();
				string indexString = ((string)parameter).Data();
				Stream converter;
				converter << indexString; // MAX?
				uint index;
				converter >> index;
				uint binaryLength = 0;

				// remove all the characters except 0 and 1 from the array.
				for each(bit in binaryDisplay)
				{
					if ((bit == ch1) || (bit == ch0))
					{
						updatedBinaryDisplay[binaryLength++] = bit;
					}
					if (binaryLength == 63)
					{
						break;
					}
				}

				// return if binaryDisplay is empty
				if (binaryLength == 0)
				{
					return (indexName + bitName + valueName + zero);
				}

				// if index is more than the length of binary display return automation name with zero
				if (index >= binaryLength)
				{
					return (indexName + bitName + valueName + zero);
				}

				// if bit is set return automation name with one else return zero
				if (updatedBinaryDisplay[binaryLength - index - 1] == ch1)
				{
					return (indexName + bitName + valueName + one);
				}
			}

			return (indexName + bitName + valueName + zero);
		}

		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			return value;
		}
	}
}