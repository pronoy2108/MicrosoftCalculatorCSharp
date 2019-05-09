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
	class CalculatorButton : Button
	{
		public CalculatorButton()
		{
			// Set the default bindings for this button, these can be overwritten by Xaml if needed
			// These are a replacement for binding in styles
			Binding commandBinding = new Binding();
			commandBinding.Path = new PropertyPath("ButtonPressed");
			this.SetBinding(Button.CommandProperty, commandBinding);
		}

		// ButtonId
		public int ButtonId
		{
			get { return (int)GetValue(ButtonIdProperty); }
			set { SetValue(ButtonIdProperty, value); }
		}

		public static readonly DependencyProperty ButtonIdProperty =
			DependencyProperty.Register("ButtonId", typeof(int), typeof(CalculatorButton), new PropertyMetadata(null));

		// AuditoryFeedback
		public int AuditoryFeedback
		{
			get { return (int)GetValue(AuditoryFeedbackProperty); }
			set { SetValue(AuditoryFeedbackProperty, value); }
		}

		public static readonly DependencyProperty AuditoryFeedbackProperty =
			DependencyProperty.Register("AuditoryFeedback", typeof(int), typeof(CalculatorButton), new PropertyMetadata(null));

		// HoverBackground
		public Brush HoverBackground
		{
			get { return (Brush)GetValue(HoverBackgroundProperty); }
			set { SetValue(HoverBackgroundProperty, value); }
		}

		public static readonly DependencyProperty HoverBackgroundProperty =
			DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(CalculatorButton), new PropertyMetadata(null));

		// HoverForeground
		public Brush HoverForeground
		{
			get { return (Brush)GetValue(HoverForegroundProperty); }
			set { SetValue(HoverForegroundProperty, value); }
		}

		public static readonly DependencyProperty HoverForegroundProperty =
			DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(CalculatorButton), new PropertyMetadata(null));

		// PressBackground
		public Brush PressBackground
		{
			get { return (Brush)GetValue(PressBackgroundProperty); }
			set { SetValue(PressBackgroundProperty, value); }
		}

		public static readonly DependencyProperty PressBackgroundProperty =
			DependencyProperty.Register("PressBackground", typeof(Brush), typeof(CalculatorButton), new PropertyMetadata(null));

		// PressForeground
		public Brush PressForeground
		{
			get { return (Brush)GetValue(PressForegroundProperty); }
			set { SetValue(PressBackgroundProperty, value); }
		}

		public static readonly DependencyProperty PressForegroundProperty =
			DependencyProperty.Register("PressForeground", typeof(Brush), typeof(CalculatorButton), new PropertyMetadata(null));

		// PROTECTED 

		override virtual void OnKeyDown(KeyRoutedEventArgs e)
		{
			// Ignore the Enter key
			if (e.Key == VirtualKey.Enter)
			{
				return;
			}

			Button.OnKeyDown(e);
		}

		override virtual void OnKeyUp(KeyRoutedEventArgs e)
		{
			// Ignore the Enter key
			if (e.Key == VirtualKey.Enter)
			{
				return;
			}

			Button.OnKeyUp(e);
		}

		// PRIVATE

		private void OnButtonIdPropertyChanged(int oldValue, int newValue)
		{
			this.CommandParameter = new CalculatorButtonPressedEventArgs(AuditoryFeedback, newValue);
		}

		private void OnAuditoryFeedbackPropertyChanged(string oldValue, string newValue)
		{
			this.CommandParameter = new CalculatorButtonPressedEventArgs(newValue, ButtonId);
		}
	}
}
