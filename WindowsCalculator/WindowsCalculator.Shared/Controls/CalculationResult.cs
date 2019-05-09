using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
//using static Uno.UI.FeatureConfiguration;

namespace WindowsCalculator.Shared.Controls
{
	public partial class CalculationResult : Control
	{
		// Visual states for focused
		static string s_FocusedState;
		static string s_UnfocusedState;

		ScrollViewer m_textContainer;
		TextBlock m_textBlock;
		HyperlinkButton m_scrollLeft;
		HyperlinkButton m_scrollRight;
		double m_startingFontSize;
		double scrollRatio = 0.7;
		bool m_isScalingText;
		bool m_haveCalculatedMax;
		EventRegistrationToken m_textContainerLayoutChangedToken;

		const double SCALEFACTOR = 0.357143;
		const int SMALLHEIGHTSCALEFACTOR = 0;
		const int HEIGHTCUTOFF = 100;
		const int INCREMENTOFFSET = 1;
		const int MAXFONTINCREMENT = 5;
		const double WIDTHTOFONTSCALAR = 0.0556513;
		const int WIDTHTOFONTOFFSET = 3;
		const int WIDTHCUTOFF = 50;
		const double FONTTOLERANCE = 0.001;

		public CalculationResult()
		{
			m_startingFontSize = 0.0;
			m_isScalingText = false;
			m_haveCalculatedMax = false;
		}

		// ExpressionVisibility
		public Visibility ExpressionVisibility
		{
			get { return (Visibility)GetValue(ExpressionVisibilityProperty); }
			set { SetValue(ExpressionVisibilityProperty, value); }
		}

		public static readonly DependencyProperty ExpressionVisibilityProperty =
			DependencyProperty.Register("ExpressionVisibility", typeof(Visibility), typeof(CalculationResult), new PropertyMetadata(null));

		// MinFontSize
		public double MinFontSize
		{
			get { return (double)GetValue(MinFontSizeProperty); }
			set { SetValue(MinFontSizeProperty, value); }
		}

		public static readonly DependencyProperty MinFontSizeProperty =
			DependencyProperty.Register("MinFontSize", typeof(double), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayMargin
		public Thickness DisplayMargin
		{
			get { return (Thickness)GetValue(DisplayMarginProperty); }
			set { SetValue(DisplayMarginProperty, value); }
		}

		public static readonly DependencyProperty DisplayMarginProperty =
			DependencyProperty.Register("DisplayMargin", typeof(Thickness), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayMargin
		public int MaxExpressionHistoryCharacters
		{
			get { return (int)GetValue(MaxExpressionHistoryCharactersProperty); }
			set { SetValue(MaxExpressionHistoryCharactersProperty, value); }
		}

		public static readonly DependencyProperty MaxExpressionHistoryCharactersProperty =
			DependencyProperty.Register("MaxExpressionHistoryCharacters", typeof(int), typeof(CalculationResult), new PropertyMetadata(null));

		// IsActive
		public bool IsActive
		{
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}

		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.Register("IsActive", typeof(bool), typeof(CalculationResult), new PropertyMetadata(null));

		// AccentColor
		public Brush AccentColor
		{
			get { return (Brush)GetValue(AccentColorProperty); }
			set { SetValue(AccentColorProperty, value); }
		}

		public static readonly DependencyProperty AccentColorProperty =
			DependencyProperty.Register("AccentColor", typeof(Brush), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayValue
		public string DisplayValue
		{
			get { return (string)GetValue(DisplayValueProperty); }
			set { SetValue(DisplayValueProperty, value); }
		}

		public static readonly DependencyProperty DisplayValueProperty =
			DependencyProperty.Register("DisplayValue", typeof(string), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayValue
		public string DisplayStringExpression
		{
			get { return (string)GetValue(DisplayStringExpressionProperty); }
			set { SetValue(DisplayStringExpressionProperty, value); }
		}

		public static readonly DependencyProperty DisplayStringExpressionProperty =
			DependencyProperty.Register("DisplayStringExpression", typeof(string), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayValue
		public bool IsInError
		{
			get { return (bool)GetValue(IsInErrorProperty); }
			set { SetValue(IsInErrorProperty, value); }
		}

		public static readonly DependencyProperty IsInErrorProperty =
			DependencyProperty.Register("IsInError", typeof(bool), typeof(CalculationResult), new PropertyMetadata(null));

		// DisplayValue
		public bool IsOperatorCommand
		{
			get { return (bool)GetValue(IsOperatorCommandProperty); }
			set { SetValue(IsOperatorCommandProperty, value); }
		}

		public static readonly DependencyProperty IsOperatorCommandProperty =
			DependencyProperty.Register("IsOperatorCommand", typeof(bool), typeof(CalculationResult), new PropertyMetadata(false));

		// CALLBACKS

		private static void OnIsActiveChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
		{

		}

		private static void OnAccentColorChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
		{

		}

		private static void OnDisplayValueChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
		{

		}

		private static void OnIsInErrorChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
		{

		}

		// INTERNAL 

		internal void UpdateTextState()
		{
			if (m_textContainer == null || m_textBlock == null)
			{
				return;
			}

			var containerSize = m_textContainer.ActualWidth;
			string oldText = m_textBlock.Text;
			string newText = DisplayValue;
			/* MAX?
			string newText = Utils.LRO + DisplayValue + Utils.PDF;
			*/

			//Initiate the scaling operation
			//UpdateLayout will keep calling us until we make it through the below 2 if-statements
			if (!m_isScalingText || oldText != newText)
			{
				m_textBlock.Text = newText;

				m_isScalingText = true;
				m_haveCalculatedMax = false;
				m_textBlock.InvalidateArrange();
				return;
			}

			if (containerSize > 0)
			{
				double widthDiff = Math.Abs(m_textBlock.ActualWidth - containerSize);
				double fontSizeChange = INCREMENTOFFSET;

				if (widthDiff > WIDTHCUTOFF)
				{
					fontSizeChange = Math.Min(Math.Max(Math.Floor(WIDTHTOFONTSCALAR * widthDiff) - WIDTHTOFONTOFFSET, INCREMENTOFFSET), MAXFONTINCREMENT);
				}

				if (m_textBlock.ActualWidth < containerSize && Math.Abs(m_textBlock.FontSize - m_startingFontSize) > FONTTOLERANCE && !m_haveCalculatedMax)
				{
					ModifyFontAndMargin(m_textBlock, fontSizeChange);
					m_textBlock.InvalidateArrange();
					return;
				}

				if (fontSizeChange < 5)
				{
					m_haveCalculatedMax = true;
				}

				if (m_textBlock.ActualWidth >= containerSize && Math.Abs(m_textBlock.FontSize - MinFontSize) > FONTTOLERANCE)
				{
					ModifyFontAndMargin(m_textBlock, -1 * fontSizeChange);
					m_textBlock.InvalidateArrange();
					return;
				}

				/* MAX?
				assert(m_textBlock.FontSize >= MinFontSize && m_textBlock.FontSize <= m_startingFontSize);
				*/

				m_isScalingText = false;

				if (IsOperatorCommand)
				{
					m_textContainer.ChangeView(0.0, null, null);
				}
				else
				{
					m_textContainer.ChangeView(m_textContainer.ExtentWidth - m_textContainer.ViewportWidth, null, null);
				}

				if (m_scrollLeft != null && m_scrollRight != null)
				{
					if (m_textBlock.ActualWidth < containerSize)
					{
						ShowHideScrollButtons(Visibility.Collapsed, Visibility.Collapsed);
					}
					else
					{
						if (IsOperatorCommand)
						{
							ShowHideScrollButtons(Visibility.Collapsed, Visibility.Visible);
						}
						else
						{
							ShowHideScrollButtons(Visibility.Visible, Visibility.Collapsed);
						}
					}
				}
				m_textBlock.InvalidateArrange();
			}
		}

		internal string GetRawDisplayValue()
		{
			string rawValue;

			/* MAX?
			LocalizationSettings.GetInstance().RemoveGroupSeparators(DisplayValue.Data, DisplayValue.Length, rawValue);
			*/

			return rawValue.ToString();
		}

		// PROTECTED 

		override protected void OnKeyDown(KeyRoutedEventArgs e)
		{
			if (m_scrollLeft != null && m_scrollRight != null)
			{
				var key = e.Key;
				if (key == VirtualKey.Left)
				{
					this.ScrollLeft();
				}
				else if (key == VirtualKey.Right)
				{
					this.ScrollRight();
				}
			}
		}

		override protected void OnApplyTemplate()
		{
			/* MAX?
			assert((m_scrollLeft == nullptr && m_scrollRight == nullptr) || (m_scrollLeft != nullptr && m_scrollRight != nullptr));
			*/

			if (m_textContainer != null)
			{
				m_textContainer.LayoutUpdated -= m_textContainerLayoutChangedToken;
			}

			m_textContainer = textContainer as ScrollViewer;

			if (m_textContainer)
			{
				m_textContainer.SizeChanged += new SizeChangedEventHandler(this, CalculationResult.TextContainerSizeChanged);

				// We want to know when the size of the container changes so 
				// we can rescale the textbox
				m_textContainerLayoutChangedToken = m_textContainer.LayoutUpdated += OnTextContainerLayoutUpdated;

				m_textContainer.ChangeView(m_textContainer.ExtentWidth - m_textContainer.ViewportWidth, null, null);

				m_scrollLeft = scrollLeft as HyperlinkButton;
				m_scrollRight = scrollRight as HyperlinkButton;
				borderContainer = border as HyperlinkButton;

				if (m_scrollLeft != null && m_scrollRight != null)
				{
					m_scrollLeft.Click += OnScrollClick;
					m_scrollRight.Click += OnScrollClick;
					borderContainer.PointerEntered += OnPointerEntered;
					borderContainer.PointerExited += OnPointerExited;
				}

				m_textBlock = m_textContainer.normalOutput as TextBlock;

				if (m_textBlock != null)
				{
					m_textBlock.Visibility = Visibility.Visible;
					m_startingFontSize = m_textBlock.FontSize;
				}
			}

			UpdateAllState();
			VisualStateManager.GoToState(this, s_UnfocusedState, false);
		}

		override protected void OnTapped(TappedRoutedEventArgs e)
		{
			this.Focus(FocusState.Programmatic);
			RaiseSelectedEvent();
		}

		override protected void OnPointerPressed(PointerRoutedEventArgs e)
		{
			if (m_scrollLeft != null && m_scrollRight != null && e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
			{
				ShowHideScrollButtons(Visibility.Collapsed, Visibility.Collapsed);
			}
		}

		override protected void OnRightTapped(RightTappedRoutedEventArgs e)
		{
			this.Focus(FocusState.Programmatic);
		}

		override protected void OnGotFocus(RoutedEventArgs e)
		{
			if (this.FocusState == FocusState.Keyboard)
			{
				VisualStateManager.GoToState(this, s_FocusedState, true);
			}
		}

		override protected void OnLostFocus(RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, s_UnfocusedState, true);
		}

		/* MAX?
		override protected virtual AutomationPeer OnCreateAutomationPeer()
		{
			return CalculationResultAutomationPeer(this);
		}
		*/

		// PUBLIC
		/* MAX?
		public event SelectedEventHandler Selected
		{

		}
		public void SelectedEventHandler(Object sender)
		{

		}
		*/

		public void ProgrammaticSelect()
		{
			RaiseSelectedEvent();
		}

		// PRIVATE METHODS
		private void OnIsActivePropertyChanged(bool oldValue, bool newValue)
		{
			UpdateVisualState();
		}

		private void OnAccentColorPropertyChanged(Brush oldValue, Brush newValue)
		{
			// Force the "Active" transition to happen again
			if (IsActive)
			{
				VisualStateManager.GoToState(this, "Normal", true);
				VisualStateManager.GoToState(this, "Active", true);
			}
		}

		private void OnDisplayValuePropertyChanged(string oldValue, string newValue)
		{
			UpdateTextState();
		}

		private void OnIsInErrorPropertyChanged(bool oldValue, bool newValue)
		{
			// We need to have a good template for this to work
			if (m_textBlock == null)
			{
				return;
			}

			if (newValue)
			{
				// If there's an error message we need to override the normal display font
				// with the font appropriate for this language. This is because the error 
				// message is localized and therefore can contain characters that are not
				// available in the normal font. 
				// We use UIText as the font type because this is the most common font type to use
				/*
				m_textBlock.FontFamily = LocalizationService.GetInstance().GetLanguageFontFamilyForType(LanguageFontType.UIText);
				*/
			}
			else
			{
				// The error result is no longer an error so we will restore the 
				// value to FontFamily property to the value provided in the style
				// for the TextBlock in the template.
				m_textBlock.ClearValue(TextBlock.FontFamilyProperty);
			}
		}

		private void TextContainerSizeChanged(Object sender, SizeChangedEventArgs e)
		{
			UpdateTextState();
		}

		private void OnTextContainerLayoutUpdated(Object sender, Object e)
		{
			if (m_isScalingText)
			{
				UpdateTextState();
			}
		}

		private void UpdateVisualState()
		{
			UpdateVisualState();
		}

		private void UpdateAllState()
		{
			UpdateVisualState();
			UpdateTextState();
		}

		private void OnScrollClick(Object sender, RoutedEventArgs e)
		{
			var clicked = sender as HyperlinkButton;

			if (clicked == m_scrollLeft)
			{
				this.ScrollLeft();
			}
			else
			{
				this.ScrollRight();
			}
		}

		private void OnPointerEntered(Object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && m_textBlock.ActualWidth >= m_textContainer.ActualWidth)
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

		private void ModifyFontAndMargin(TextBlock textBlock, double fontChange)
		{
			double cur = textBox.FontSize;
			double newFontSize = 0.0;
			Thickness t = textBox.Margin;
			double scaleFactor = SCALEFACTOR;

			if (m_textContainer.ActualHeight <= HEIGHTCUTOFF)
			{
				scaleFactor = SMALLHEIGHTSCALEFACTOR;
			}
			if (fontChange < 0)
			{
				newFontSize = Math.Max(cur + fontChange, MinFontSize);
				t.Bottom += scaleFactor * Math.Abs(cur - newFontSize);
			}
			else
			{
				newFontSize = Math.Min(cur + fontChange, m_startingFontSize);
				t.Bottom -= scaleFactor * Math.Abs(cur - newFontSize);
			}

			textBox.FontSize = newFontSize;
			textBox.Margin = t;
		}

		private void ShowHideScrollButtons(Visibility vLeft, Visibility vRight)
		{
			m_scrollLeft.Visibility = vLeft;
			m_scrollRight.Visibility = vRight;
		}

		private void UpdateScrollButtons()
		{    
			// When the width is smaller than the container, don't show any
			if (m_textBlock.ActualWidth < m_textContainer.ActualWidth)
			{
				ShowHideScrollButtons(Visibility.Collapsed, Visibility.Collapsed);
			}
			// We have more number on both side. Show both arrows
			else if (m_textContainer.HorizontalOffset > 0 && m_textContainer.HorizontalOffset < (m_textContainer.ExtentWidth - m_textContainer.ViewportWidth))
			{
				ShowHideScrollButtons(Visibility.Visible, Visibility.Visible);
			}
			// Width is larger than the container and left most part of the number is shown. Should be able to scroll left.
			else if (m_textContainer.HorizontalOffset == 0)
			{
				ShowHideScrollButtons(Visibility.Collapsed, Visibility.Visible);
			}
			else // Width is larger than the container and right most part of the number is shown. Should be able to scroll left.
			{
				ShowHideScrollButtons(Visibility.Visible, Visibility.Collapsed);
			}
		}

		private void ScrollLeft()
		{
			if (m_textContainer.HorizontalOffset > 0)
			{
				double offset = m_textContainer.HorizontalOffset - (scrollRatio * m_textContainer.ViewportWidth);

				m_textContainer.ChangeView(offset, null, null);
				m_textContainer.UpdateLayout();
				UpdateScrollButtons();
			}
		}

		private void ScrollRight()
		{
			if (m_textContainer.HorizontalOffset < m_textContainer.ExtentWidth - m_textContainer.ViewportWidth)
			{
				double offset = m_textContainer.HorizontalOffset + (scrollRatio * m_textContainer.ViewportWidth);

				m_textContainer.ChangeView(offset, null, null);
				m_textContainer.UpdateLayout();
				UpdateScrollButtons();
			}
		}

		private void RaiseSelectedEvent()
		{
			Selected(this);
		}
	}
}
