﻿using System;
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
	class FlipButtons : ToggleButton
	{

		// NumbersAndOperatorsEnum
		public int NumbersAndOperatorsEnum
		{
			get { return (int)GetValue(NumbersAndOperatorsEnumProperty); }
			set { SetValue(NumbersAndOperatorsEnumProperty, value); }
		}

		public static readonly DependencyProperty NumbersAndOperatorsEnumProperty =
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
		override protected virtual void OnKeyDown(KeyRoutedEventArgs e)
		{
			// Ignore the Enter key
			if (e.Key == VirtualKey.Enter)
			{
				return;
			}

			ToggleButton.OnKeyDown(e);
		}

		override protected virtual void OnKeyUp(KeyRoutedEventArgs e)
		{
			// Ignore the Enter key
			if (e.Key == VirtualKey.Enter)
			{
				return;
			}

			ToggleButton.OnKeyUp(e);
		}

		// PRIVATE
		private void OnButtonIdPropertyChanged(NumbersAndOperatorsEnum oldValue, NumbersAndOperatorsEnum newValue)
		{
			this.CommandParameter = newValue;
		}
	}
}