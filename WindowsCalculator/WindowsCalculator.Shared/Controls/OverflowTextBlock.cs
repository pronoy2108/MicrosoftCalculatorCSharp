using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WindowsCalculator.Shared.Controls
{
	class OverflowTextBlock : Control
	{
		const double scrollRatio = 0.7;

		bool m_scrollingLeft;
		bool m_scrollingRight;

		ListView m_listView;
		ScrollViewer m_expressionContainer;
		Button m_scrollLeft;
		Button m_scrollRight;

		EventRegistrationToken m_scrollLeftClickEventToken;
		EventRegistrationToken m_scrollRightClickEventToken;
		EventRegistrationToken m_pointerEnteredEventToken;
		EventRegistrationToken m_pointerExitedEventToken;

		// PROCTETED

		override protected void OnApplyTemplate()
		{
			/* MAX?
			assert(((m_scrollLeft == nullptr) && (m_scrollRight == nullptr)) || ((m_scrollLeft != nullptr) && (m_scrollRight != nullptr)));
			*/

			m_expressionContainer = expressionContainer as ScrollViewer;
			m_expressionContainer.ChangeView(m_expressionContainer.ExtentWidth - m_expressionContainer.ViewportWidth, null, null);

			m_scrollLeft = scrollLeft as Button;
			m_scrollRight = scrollRight as Button;

			m_scrollLeftClickEventToken = m_scrollLeft.Click += OnScrollClick;
			m_scrollRightClickEventToken = m_scrollRight.Click += OnScrollClick;

			m_scrollingLeft = false;
			m_scrollingRight = false;

			auto borderContainer = expressionborder as Border;
			m_pointerEnteredEventToken = borderContainer.PointerEntered += this.OnPointerEntered;
			m_pointerExitedEventToken = borderContainer.PointerExited += this.OnPointerExited;

			m_listView = TokenList as ListView;

			UpdateAllState();
		}

		override protected AutomationPeer OnCreateAutomationPeer()
		{
			return new OverflowTextBlockAutomationPeer(this);
		}

		// PRIVATE 

		private void OnTokensUpdatedPropertyChanged(bool oldValue, bool newValue)
		{
			if ((m_listView != null) && (newValue))
			{
				uint tokenCount = m_listView.Items.Size;
				if (tokenCount > 0)
				{
					m_listView.UpdateLayout();
					m_listView.ScrollIntoView(m_listView.Items.GetAt(tokenCount - 1));
					m_expressionContainer.ChangeView(m_expressionContainer.ExtentWidth - m_expressionContainer.ViewportWidth, null, null);
				}
			}
			AutomationProperties.SetAccessibilityView(this, m_listView != null && m_listView.Items.Size > 0 ? AccessibilityView.Control : AccessibilityView.Raw);
		}

		private void UpdateAllState()
		{
			UpdateVisualState();
		}

		private void UpdateVisualState()
		{
			if (IsActive)
			{
				VisualStateManager.GoToState(this, "Active", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", true);
			}
		}

		private void ScrollLeft()
		{
			if (m_expressionContainer.HorizontalOffset > 0)
			{
				m_scrollingLeft = true;
				double offset = m_expressionContainer.HorizontalOffset - (scrollRatio * m_expressionContainer.ViewportWidth);
				m_expressionContainer.ChangeView(offset, null, null);
				m_expressionContainer.UpdateLayout();
				UpdateScrollButtons();
			}
		}

		private void ScrollRight()
		{
			if (m_expressionContainer.HorizontalOffset < m_expressionContainer.ExtentWidth - m_expressionContainer.ViewportWidth)
			{
				m_scrollingRight = true;
				double offset = m_expressionContainer.HorizontalOffset + (scrollRatio * m_expressionContainer.ViewportWidth);
				m_expressionContainer.ChangeView(offset, null, null);
				m_expressionContainer.UpdateLayout();
				UpdateScrollButtons();
			}
		}

		private void OnScrollClick(Object sender, RoutedEventArgs)
		{
			var clicked = sender as Button;

			if (clicked == m_scrollLeft)
			{
				ScrollLeft();
			}
			else
			{
				ScrollRight();
			}
		}

		private void OnPointerEntered(Object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
			{
				UpdateScrollButtons();
			}
		}

		private void OnPointerExited(Object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
			{
				UpdateScrollButtons();
			}
		}

		// PUBLIC 

		public void UpdateScrollButtons()
		{
			// When the width is smaller than the container, don't show any
			if (m_listView.ActualWidth <= m_expressionContainer.ActualWidth)
			{
				ShowHideScrollButtons(Visibility.Collapsed, Visibility.Collapsed);
			}
			// We have more number on both side. Show both arrows
			else if ((m_expressionContainer.HorizontalOffset > 0) && (m_expressionContainer.HorizontalOffset < (m_expressionContainer.ExtentWidth - m_expressionContainer.ViewportWidth)))
			{
				ShowHideScrollButtons(Visibility.Visible, Visibility.Visible);
			}
			// Width is larger than the container and left most part of the number is shown. Should be able to scroll left.
			else if (m_expressionContainer.HorizontalOffset == 0)
			{
				ShowHideScrollButtons(Visibility.Collapsed, Visibility.Visible);
				if (m_scrollingLeft)
				{
					m_scrollingLeft = false;
					m_scrollRight.Focus(FocusState.Programmatic);
				}
			}
			else // Width is larger than the container and right most part of the number is shown. Should be able to scroll left.
			{
				ShowHideScrollButtons(Visibility.Visible, Visibility.Collapsed);
				if (m_scrollingRight)
				{
					m_scrollingRight = false;
					m_scrollLeft.Focus(FocusState.Programmatic);
				}
			}
		}

		public void UnregisterEventHandlers()
		{
			// Unregister the event handlers
			if (m_scrollLeft != null)
			{
				m_scrollLeft.Click -= m_scrollLeftClickEventToken;
			}

			if (m_scrollRight != null)
			{
				m_scrollRight.Click -= m_scrollRightClickEventToken;
			}

			var borderContainer = expressionborder Border;

			// Adding an extra check, in case the returned template is null
			if (borderContainer != null)
			{
				borderContainer.PointerEntered -= m_pointerEnteredEventToken;
				borderContainer.PointerExited -= m_pointerExitedEventToken;
			}
		}
	}
}
