using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace WindowsCalculator.Shared.Controls
{
	sealed class CalculationResultAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
        public CalculationResultAutomationPeer(FrameworkElement owner)
		{

		}

		override virtual AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType::Text;
		}

		override virtual Object GetPatternCore(PatternInterface pattern)
		{
			if (pattern == PatternInterface.Invoke)
			{
				return this;
			}

			return FrameworkElementAutomationPeer.GetPatternCore(pattern);
		}

        virtual void Invoke()
		{
			var owner = Owner as CalculationResult;
			owner.ProgrammaticSelect();
		}
	}
}
