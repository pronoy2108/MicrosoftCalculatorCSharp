using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using CalculatorApp.Common;
using Windows.Foundation.Collections;
using CalculatorApp.ViewModel;
using ApplicationViewModel;
using CalculatorApp.Common.NavCategory;

using CalculatorApp;
using CalculatorApp.DataLoaders;
using CalculationManager;
using Platform;
using std;
using Windows.System;
using Windows.Storage;
using Utils;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WindowsCalculator.Shared.ViewModels
{
	/* MAX?
	namespace ApplicationViewModelProperties
        {
            extern Platform::StringReference Mode;
            extern Platform::StringReference PreviousMode;
            extern Platform::StringReference ClearMemoryVisibility;
            extern Platform::StringReference AppBarVisibility;
            extern Platform::StringReference CategoryName;
            extern Platform::StringReference Categories;
        }
	*/

	[Bindable]
	class ApplicationViewModel
	{

		private ViewMode m_mode;
		private IObservableVector<NavCategoryGroup> m_categories;

		// PUBLIC
		public ApplicationViewModel();

		// Use for first init, use deserialize for rehydration
		public void Initialize(ViewMode mode);

		/* MAX?
		OBSERVABLE_OBJECT();
		OBSERVABLE_PROPERTY_RW(StandardCalculatorViewModel, CalculatorViewModel);
		OBSERVABLE_PROPERTY_RW(DateCalculatorViewModel, DateCalcViewModel);
		OBSERVABLE_PROPERTY_RW(UnitConverterViewModel, ConverterViewModel);
		OBSERVABLE_PROPERTY_RW(ViewMode, PreviousMode);
		OBSERVABLE_PROPERTY_RW(String, CategoryName);

		COMMAND_FOR_METHOD(CopyCommand, OnCopyCommand);
		COMMAND_FOR_METHOD(PasteCommand, OnPasteCommand);
		*/

		public ApplicationViewModel()
		{
			m_CalculatorViewModel = null;
			m_DateCalcViewModel = null;
			m_ConverterViewModel = null;
			m_PreviousMode = ViewMode.None;
			m_mode = ViewMode.None;
			m_categories = null;

			SetMenuCategories();
		}

		public void Initialize(ViewMode mode)
		{
			if (!NavCategory.IsValidViewMode(mode))
			{
				mode = ViewMode.Standard;
			}

			try
			{
				Mode = mode;
			}
			// MAX?
			//catch (const std::exception&e)
			catch ()
			{
				/* MAX?
				TraceLogger.GetInstance().LogStandardException(__FUNCTIONW__, e);
				*/
				if (!TryRecoverFromNavigationModeFailure())
				{
					// Could not navigate to standard mode either.
					// Throw the original exception so we have a good stack to debug.
					throw;
				}
			}

			catch (Exception e)
			{
				/* MAX?
				TraceLogger.GetInstance().LogPlatformException(__FUNCTIONW__, e);
				*/
				if (!TryRecoverFromNavigationModeFailure())
				{
					// Could not navigate to standard mode either.
					// Throw the original exception so we have a good stack to debug.
					throw;
				}
			}
		}

		public ViewMode Mode
		{
			get { return m_mode; }
			set
			{
				if (m_mode != value)
				{
					PreviousMode = m_mode;
					m_mode = value;
					OnModeChanged();
					RaisePropertyChanged(ApplicationViewModelProperties.Mode);
				}
			}
		}

		public IObservableVector<NavCategoryGroup> Categories
		{
			get { return m_categories; }
			set
			{
				if (m_categories != value)
				{
					m_categories = value;
					RaisePropertyChanged(ApplicationViewModelProperties.Categories);
				}
			}
		}

		public Visibility ClearMemoryVisibility
		{
			get { return NavCategory.IsCalculatorViewMode(Mode) ? Visibility.Visible : Visibility.Collapsed; }
		}

		public Visibility AppBarVisibility
		{
			get { return NavCategory.IsCalculatorViewMode(Mode) ? Visibility.Visible : Visibility.Collapsed; }
		}

		/*
		public object ShowCartActiveCommand
		{
			get { return (object)GetValue(ShowCartActiveCommandProperty); }
			set { SetValue(ShowCartActiveCommandProperty, value); }
		}
		*/

		// PRIVATE 
		private bool TryRecoverFromNavigationModeFailure()
		{
			// Here we are simply trying to recover from being unable to navigate to a mode.
			// Try falling back to standard mode and if there are *any* exceptions, we should
			// fail because something is seriously wrong.
			try
			{
				Mode = ViewMode.Standard;
				return true;
			}
			catch (...)
			{
				return false;
			}
			}

			private void OnModeChanged()
			{
				/* MAX?
				assert(NavCategory::IsValidViewMode(m_mode));
				TraceLogger::GetInstance().LogModeChangeBegin(m_PreviousMode, m_mode, ApplicationView::GetApplicationViewIdForWindow(CoreWindow::GetForCurrentThread()));
				*/

				if (NavCategory.IsCalculatorViewMode(m_mode))
				{
					TraceLogger.GetInstance().LogCalculatorModeViewed(m_mode, ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread()));
					if (!m_CalculatorViewModel)
					{
						m_CalculatorViewModel = ref new StandardCalculatorViewModel();
					}
					m_CalculatorViewModel->SetCalculatorType(m_mode);
				}
				else if (NavCategory.IsDateCalculatorViewMode(m_mode))
				{
					TraceLogger.GetInstance().LogDateCalculatorModeViewed(m_mode, ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread()));
					if (!m_DateCalcViewModel)
					{
						m_DateCalcViewModel = ref new DateCalculatorViewModel();
					}
				}
				else if (NavCategory.IsConverterViewMode(m_mode))
				{
					TraceLogger.GetInstance().LogConverterModeViewed(m_mode, ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread()));
					if (!m_ConverterViewModel)
					{
						var dataLoader = make_shared<UnitConverterDataLoader>(ref new GeographicRegion());
						var currencyDataLoader = make_shared<CurrencyDataLoader>(make_unique<CurrencyHttpClient>());
						m_ConverterViewModel = new UnitConverterViewModel(make_shared<UnitConversionManager.UnitConverter>(dataLoader, currencyDataLoader));
					}

					m_ConverterViewModel->Mode = m_mode;
				}

				auto resProvider = AppResourceProvider::GetInstance();
				CategoryName = resProvider.GetResourceString(NavCategory::GetNameResourceKey(m_mode));

				// This is the only place where a ViewMode enum should be cast to an int.
				//
				// Save the changed mode, so that the new window launches in this mode.
				// Don't save until after we have adjusted to the new mode, so we don't save a mode that fails to load.
				ApplicationData::Current->LocalSettings->Values->Insert(ApplicationViewModelProperties::Mode, NavCategory::Serialize(m_mode));

				TraceLogger::GetInstance().LogModeChangeEnd(m_mode, ApplicationView::GetApplicationViewIdForWindow(CoreWindow::GetForCurrentThread()));
				RaisePropertyChanged(ApplicationViewModelProperties::ClearMemoryVisibility);
				RaisePropertyChanged(ApplicationViewModelProperties::AppBarVisibility);
			}

			private void OnCopyCommand(Object parameter)
			{
				if (NavCategory.IsConverterViewMode(m_mode))
				{
					ConverterViewModel.OnCopyCommand(parameter);
				}
				else if (NavCategory.IsDateCalculatorViewMode(m_mode))
				{
					DateCalcViewModel.OnCopyCommand(parameter);
				}
				else
				{
					CalculatorViewModel.OnCopyCommand(parameter);
				}
			}

			private void OnPasteCommand(Object parameter)
			{
				if (NavCategory.IsConverterViewMode(m_mode))
				{
					ConverterViewModel.OnPasteCommand(parameter);
				}
				else
				{
					CalculatorViewModel.OnPasteCommand(parameter);
				}
			}

			private void SetMenuCategories()
			{
				// Use the Categories property instead of the backing variable
				// because we want to take advantage of binding updates and
				// property setter logic.
				Categories = NavCategoryGroup.CreateMenuOptions();
			}
		}
	}
}