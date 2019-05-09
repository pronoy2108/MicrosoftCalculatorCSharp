using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace WindowsCalculator.Shared.Controls
{
	public class SupplementaryItemsControl : ItemsControl
	{
		// PROTECTED 
		protected override DependencyObject GetContainerForItemOverride()
		{

		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, Object item)
		{

		}
	}

	public sealed class SupplementaryContentPresenter : ContentPresenter
	{

		protected override AutomationPeer OnCreateAutomationPeer()
		{

		}
	}

	public sealed class SupplementaryContentPresenterAP : FrameworkElementAutomationPeer
    {
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Text;
        }

		protected override Vector<AutomationPeer> GetChildrenCore()
        {
            return null;
        }

		internal SupplementaryContentPresenterAP(SupplementaryContentPresenter owner) :
			private FrameworkElementAutomationPeer(owner)
        {
		}
	}
}
