using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WindowsCalculator.Shared.Controls
{
	class RadixButton : RadioButton
	{
		public RadixButton()
		{

		}

		internal string GetRawDisplayValue()
		{
			string rawValue;
			string radixContent = Content.ToString();
			LocalizationSettings.GetInstance().RemoveGroupSeparators(radixContent.Data, radixContent.Length, rawValue);

			return rawValue;
		}
	}
}
