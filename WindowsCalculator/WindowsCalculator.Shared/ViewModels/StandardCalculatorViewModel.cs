using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsCalculator.Shared.ViewModels
{
	class StandardCalculatorViewModel
	{
		string m_DisplayValue = "0";
		string m_DecimalDisplayValue = "0";
		string m_HexDisplayValue = "0";
		string m_BinaryDisplayValue = "0";
		string m_OctalDisplayValue = "0";
		m_standardCalculatorManager = (m_calculatorDisplay, m_resourceProvider);
		m_MemorizedNumbers = new Vector<MemoryItemViewModel>());
		bool m_IsMemoryEmpty = true;
		bool m_IsFToEChecked = false;
		bool m_isShiftChecked = false;
		bool m_IsShiftProgrammerChecked = false;
		bool m_IsQwordEnabled = true;
		bool m_IsDwordEnabled = true;
		bool m_IsWordEnabled = true;
		bool m_IsByteEnabled = true;
		bool m_isBitFlipChecked = false;
		bool m_isBinaryBitFlippingEnabled = false;
		RADIX_TYPE m_CurrentRadixType = RADIX_TYPE.DEC_RADIX;
		NumbersAndOperatorsEnum m_CurrentAngleType = NumbersAndOperatorsEnum.Degree;
		string m_OpenParenthesisCount = "";
		Object m_Announcement = null;
		Object m_feedbackForButtonPress = null;
		bool m_isRtlLanguage = false;
		Object m_localizedMaxDigitsReachedAutomationFormat = null;
		Object m_localizedButtonPressFeedbackAutomationFormat = null;
		Object m_localizedMemorySavedAutomationFormat = null;
		Object m_localizedMemoryItemChangedAutomationFormat = null;
		Object m_localizedMemoryItemClearedAutomationFormat = null;
		Object m_localizedMemoryCleared = null;
		string m_decimalSeparator;
		CalculatorDisplay m_calculatorDisplay;

		string m_expressionAutomationNameFormat;
		string m_localizedCalculationResultAutomationFormat;
		string m_localizedCalculationResultDecimalAutomationFormat;
		string m_localizedHexaDecimalAutomationFormat;
		string m_localizedDecimalAutomationFormat;
		string m_localizedOctalAutomationFormat;
		string m_localizedBinaryAutomationFormat;

		CalculatorApp.EngineResourceProvider m_resourceProvider;
		CalculationManager.CalculatorManager m_standardCalculatorManager;

		public StandardCalculatorViewModel()
		{
			/* MAX?
			WeakReference calculatorViewModel(this);
			*/

			m_decimalSeparator = LocalizationSettings.GetInstance().GetDecimalSeparator();
			m_calculatorDisplay.SetCallback(calculatorViewModel);

			m_expressionAutomationNameFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.CalculatorExpression);
			m_localizedCalculationResultAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.CalculatorResults);
			m_localizedCalculationResultDecimalAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.CalculatorResults_DecimalSeparator_Announced);
			m_localizedHexaDecimalAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.HexButton);
			m_localizedDecimalAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.DecButton);
			m_localizedOctalAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.OctButton);
			m_localizedBinaryAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.BinButton);
			m_leftParenthesisAutomationFormat = AppResourceProvider.GetInstance().GetResourceString(CalculatorResourceKeys.LeftParenthesisAutomationFormat);

			// Initialize the Automation Name
			CalculationResultAutomationName = GetLocalizedStringFormat(m_localizedCalculationResultAutomationFormat, m_DisplayValue);
			CalculationExpressionAutomationName = GetLocalizedStringFormat(m_expressionAutomationNameFormat, "");

			// Initialize history view model
			m_HistoryVM = new HistoryViewModel(m_standardCalculatorManager);
			m_HistoryVM.SetCalculatorDisplay(m_calculatorDisplay);

			m_decimalSeparator = LocalizationSettings.GetInstance().GetDecimalSeparator();

			if (CoreWindow.GetForCurrentThread() != null)
			{
				// Must have a CoreWindow to access the resource context.
				m_isRtlLanguage = LocalizationService.GetInstance().IsRtlLayout();
			}
			
			IsEditingEnabled = false;
			IsUnaryOperatorEnabled = true;
			IsBinaryOperatorEnabled = true;
			IsOperandEnabled = true;
			IsNegateEnabled = true;
			IsDecimalEnabled = true;
			AreHistoryShortcutsEnabled = true;
			AreProgrammerRadixOperatorsEnabled = false;
			
			m_tokenPosition = -1;
			m_isLastOperationHistoryLoad = false;
			
		}


		// PRIVATE
		string LocalizeDisplayValue(string displayValue, bool isError)
		{
			string result(displayValue);

			LocalizationSettings.GetInstance().LocalizeDisplayValue(result);

			// WINBLUE: 440747 - In BiDi languages, error messages need to be wrapped in LRE/PDF
			if (isError && m_isRtlLanguage)
			{
				result.insert(result.begin(), Utils.LRE);
				result.push_back(Utils.PDF);
			}

			return result;
		}

		private string CalculateNarratorDisplayValue(string displayValue, string localizedDisplayValue, bool isError)
		{

			string localizedValue = localizedDisplayValue;
			string automationFormat = m_localizedCalculationResultAutomationFormat;

			// The narrator doesn't read the decimalSeparator if it's the last character
			if (Utils.IsLastCharacterTarget(displayValue, m_decimalSeparator))
			{
				// remove the decimal separator, to avoid a long pause between words
				localizedValue = LocalizeDisplayValue(displayValue.Substring(0, displayValue.Length - 1), isError);

				// Use a format which has a word in the decimal separator's place
				// "The Display is 10 point"
				automationFormat = m_localizedCalculationResultDecimalAutomationFormat;
			}

			// In Programmer modes using non-base10, we want the strings to be read as literal digits.
			if (IsProgrammer && CurrentRadixType != RADIX_TYPE.DEC_RADIX)
			{
				localizedValue = GetNarratorStringReadRawNumbers(localizedValue);
			}

			return GetLocalizedStringFormat(automationFormat, localizedValue);
		}

		private string GetNarratorStringReadRawNumbers(string localizedDisplayValue)
		{
			string wss;
			RADIX_TYPE radix = CurrentRadixType as RADIX_TYPE;
			var locSettings = LocalizationSettings.GetInstance();

			// Insert a space after each digit in the string, to force Narrator to read them as separate numbers.
			/* MAX?
			wstring wstrValue(localizedDisplayValue->Data());
			*/
			/* MAX? For each maybe
			for (wchar_t& c : wstrValue)
			{
				wss << c;
				if (locSettings.IsLocalizedHexDigit(c))
				{
					wss << L' ';
				}
			}
			*/

			return wss;
		}

		private void SetMemorizedNumbers(Vector<string> memorizedNumbers)
		{
			var localizer = LocalizationSettings.GetInstance();
			if (newMemorizedNumbers.size() == 0) // Memory has been cleared
			{
				MemorizedNumbers.Clear();
				IsMemoryEmpty = true;
			}
			// A new value is added to the memory
			else if (newMemorizedNumbers.size() > MemorizedNumbers.Length)
			{
				while (newMemorizedNumbers.size() > MemorizedNumbers.Length)
				{
					size_t newValuePosition = newMemorizedNumbers.size() - MemorizedNumbers.Length - 1;
					var stringValue = newMemorizedNumbers.at(newValuePosition);

					MemoryItemViewModel memorySlot = new MemoryItemViewModel(this);
					memorySlot.Position = 0;
					localizer.LocalizeDisplayValue(stringValue);

					memorySlot.Value = Utils.LRO + stringValue + Utils.PDF;

					MemorizedNumbers.InsertAt(0, memorySlot);
					IsMemoryEmpty = false;

					// Update the slot position for the rest of the slots
					for (unsigned int i = 1; i < MemorizedNumbers.Length; i++)
					{
						MemorizedNumbers.GetAt(i).Position++;
					}
				}
			}
			else if (newMemorizedNumbers.size() == MemorizedNumbers.Length) // Either M+ or M-
			{
				for (uint i = 0; i < MemorizedNumbers.Length; i++)
				{
					var newStringValue = newMemorizedNumbers.at(i);
					localizer.LocalizeDisplayValue(newStringValue);

					// If the value is different, update the value
					if (MemorizedNumbers.GetAt(i).Value != StringReference(newStringValue))
					{
						MemorizedNumbers.GetAt(i).Value = Utils.LRO + ref new String(newStringValue.c_str()) + Utils::PDF;
					}

				}
			}
		}

		private void UpdateProgrammerPanelDisplay()
		{
			string hexDisplayString;
			string decimalDisplayString;
			string octalDisplayString;
			string binaryDisplayString;

			if (!IsInError)
			{
				// we want the precision to be set to maximum value so that the autoconversions result as desired
				int32_t precision = 64;

				if (m_standardCalculatorManager.GetResultForRadix(16, precision) == "")
				{
					hexDisplayString = DisplayValue->Data();
					decimalDisplayString = DisplayValue->Data();
					octalDisplayString = DisplayValue->Data();
					binaryDisplayString = DisplayValue->Data();
				}
				else
				{
					hexDisplayString = m_standardCalculatorManager.GetResultForRadix(16, precision);
					decimalDisplayString = m_standardCalculatorManager.GetResultForRadix(10, precision);
					octalDisplayString = m_standardCalculatorManager.GetResultForRadix(8, precision);
					binaryDisplayString = m_standardCalculatorManager.GetResultForRadix(2, precision);
				}
			}

			var localizer = LocalizationSettings.GetInstance();
			binaryDisplayString = AddPadding(binaryDisplayString);

			localizer.LocalizeDisplayValue(hexDisplayString);
			localizer.LocalizeDisplayValue(decimalDisplayString);
			localizer.LocalizeDisplayValue(octalDisplayString);
			localizer.LocalizeDisplayValue(binaryDisplayString);

			HexDisplayValue = Utils.LRO + hexDisplayString + Utils.PDF;
			DecimalDisplayValue = Utils.LRO + decimalDisplayString + Utils.PDF;
			OctalDisplayValue = Utils.LRO + octalDisplayString + Utils.PDF;
			BinaryDisplayValue = Utils.LRO + binaryDisplayString + Utils.PDF;

			HexDisplayValue_AutomationName = GetLocalizedStringFormat(m_localizedHexaDecimalAutomationFormat, GetNarratorStringReadRawNumbers(HexDisplayValue));
			DecDisplayValue_AutomationName = GetLocalizedStringFormat(m_localizedDecimalAutomationFormat, DecimalDisplayValue);
			OctDisplayValue_AutomationName = GetLocalizedStringFormat(m_localizedOctalAutomationFormat, GetNarratorStringReadRawNumbers(OctalDisplayValue));
			BinDisplayValue_AutomationName = GetLocalizedStringFormat(m_localizedBinaryAutomationFormat, GetNarratorStringReadRawNumbers(BinaryDisplayValue));
		}

		private void HandleUpdatedOperandData(CalculationManager.Command cmdenum)
		{
			DisplayExpressionToken displayExpressionToken = ExpressionTokens.GetAt(m_tokenPosition);

			if (displayExpressionToken == null)
			{
				return;
			}

			if ((displayExpressionToken.Token == null) || (displayExpressionToken.Token.Length() == 0))
			{
				displayExpressionToken->CommandIndex = 0;
			}

			char ch;

			if ((cmdenum >= Command.Command0) && (cmdenum <= Command.Command9))
			{
				switch (cmdenum)
				{
					case Command.Command0:
						ch = '0';
						break;
					case Command.Command1:
						ch = '1';
						break;
					case Command.Command2:
						ch = '2';
						break;
					case Command.Command3:
						ch = '3';
						break;
					case Command.Command4:
						ch = '4';
						break;
					case Command.Command5:
						ch = '5';
						break;
					case Command.Command6:
						ch = '6';
						break;
					case Command.Command7:
						ch = '7';
						break;
					case Command.Command8:
						ch = '8';
						break;
					case Command.Command9:
						ch = '9';
						break;
				}
			}
			else if (cmdenum == Command.CommandPNT)
			{
				ch = '.';
			}
			else if (cmdenum == Command.CommandBACK)
			{
				ch = 'x';
			}
			else
			{
				return;
			}

			int length = 0;
			string temp = "100";
			string data = m_selectedExpressionLastData.Data();
			int i = 0, j = 0;
			int commandIndex = displayExpressionToken.CommandIndex;

			if (IsOperandTextCompletelySelected)
			{
				//Clear older text;
				m_selectedExpressionLastData = "";
				if (ch == 'x')
				{
					temp = '\0';
					commandIndex = 0;
				}
				else
				{
					temp[0] = ch;
					temp[1] = '\0';
					commandIndex = 1;
				}
				IsOperandTextCompletelySelected = false;
			}
			else
			{
				if (ch == 'x')
				{
					if (commandIndex == 0)
					{
						delete temp;
						return;
					}

					length = m_selectedExpressionLastData.Length();
					for (; j < length; ++j)
					{
						if (j == commandIndex - 1)
						{
							continue;
						}
						temp[i++] = data[j];
					}
					temp[i] = '\0';
					commandIndex -= 1;
				}
				else
				{
					length = m_selectedExpressionLastData.Length() + 1;
					if (length > 50)
					{
						return;
					}
					for (; i < length; ++i)
					{
						if (i == commandIndex)
						{
							temp[i] = ch;
							continue;
						}
						temp[i] = data[j++];
					}
					temp[i] = '\0';
					commandIndex += 1;
				}
			}

			string updatedData = temp;
			UpdateOperand(m_tokenPosition, updatedData);
			displayExpressionToken.Token = updatedData;
			IsOperandUpdatedUsingViewModel = true;
			displayExpressionToken.CommandIndex = commandIndex;
		}
		
		private NumbersAndOperatorsEnum ConvertIntegerToNumbersAndOperatorsEnum(uint parameter)
		{
			NumbersAndOperatorsEnum angletype;
			switch (parameter)
			{
				case 321:
					angletype = NumbersAndOperatorsEnum.Degree;
					break;
				case 322:
					angletype = NumbersAndOperatorsEnum.Radians;
					break;
				case 323:
					angletype = NumbersAndOperatorsEnum.Grads;
					break;
				default:
					angletype = NumbersAndOperatorsEnum.Degree;
			};
			return angletype;
		}
		


		bool m_pinned;
		bool m_isOperandEnabled;
		bool m_isEditingEnabled;
		bool m_isStandard;
		bool m_isScientific;
		bool m_isProgrammer;
		bool m_isBinaryBitFlippingEnabled;
		bool m_isBitFlipChecked;
		bool m_isShiftChecked;
		bool m_isRtlLanguage;
		int m_tokenPosition;
		bool m_keyPressed;
		bool m_operandUpdated;
		bool m_completeTextSelection;
		bool m_isLastOperationHistoryLoad;
		Platform::String^ m_selectedExpressionLastData;
        Common::DisplayExpressionToken^ m_selectedExpressionToken;
        Platform::String^ m_leftParenthesisAutomationFormat;

        Platform::String^ LocalizeDisplayValue(_In_ std::wstring const &displayValue, _In_ bool isError);
		CalculatorApp::Common::Automation::NarratorAnnouncement^ GetDisplayUpdatedNarratorAnnouncement();
		Platform::String^ GetCalculatorExpressionAutomationName();
		Platform::String^ GetNarratorStringReadRawNumbers(_In_ Platform::String^ localizedDisplayValue);

		CalculationManager::Command ConvertToOperatorsEnum(NumbersAndOperatorsEnum operation);
		void DisableButtons(CalculationManager::CommandType selectedExpressionCommandType);
		Platform::String^ GetLeftParenthesisAutomationName();

		Platform::String^ m_feedbackForButtonPress;
        void OnButtonPressed(Platform::Object^ parameter);
		void OnClearMemoryCommand(Platform::Object^ parameter);
		std::wstring AddPadding(std::wstring);
		size_t LengthWithoutPadding(std::wstring);

		std::shared_ptr<CalculatorVector<std::pair<std::wstring, int>>> m_tokens;
		std::shared_ptr<CalculatorVector<std::shared_ptr<IExpressionCommand>>> m_commands;

		// Token types
		bool IsUnaryOp(int nOpCode);
		bool IsBinOp(int nOpcode);
		bool IsTrigOp(int nOpCode);
		bool IsOpnd(int nOpCode);
		bool IsRecoverableCommand(int nOpCode);

		CalculationManager::CommandType GetSelectedTokenType(_In_ unsigned int);
		void SaveEditedCommand(_In_ unsigned int index, _In_ CalculationManager::Command command);

		bool IsViewPinned();
		void SetViewPinnedState(bool pinned);

		friend class CalculatorDisplay;
		friend class CalculatorFunctionalTests::HistoryTests;
        friend class CalculatorUnitTests::MultiWindowUnitTests;
        friend class CalculatorUnitTests::TimerTests;
	}
}
/*
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma once
#include "Common\Automation\NarratorAnnouncement.h"
#include "Common\DisplayExpressionToken.h"
#include "Common\CalculatorDisplay.h"
#include "Common\EngineResourceProvider.h"
#include "Common\CalculatorButtonUser.h"
#include "HistoryViewModel.h"
#include "MemoryItemViewModel.h"

namespace CalculatorFunctionalTests
{
    class HistoryTests;
}

namespace CalculatorUnitTests
{
    class MultiWindowUnitTests;
    class TimerTests;
}

namespace CalculatorApp
{
    namespace WS = Windows::System;
    namespace CM = CalculationManager;

    namespace ViewModel
    {
#define ASCII_0 48
        public delegate void HideMemoryClickedHandler();
        public delegate void ProgModeRadixChangeHandler();
        namespace CalculatorViewModelProperties
        {
            extern Platform::StringReference IsMemoryEmpty;
            extern Platform::StringReference IsInError;
            extern Platform::StringReference BinaryDisplayValue;
        }

        [Windows::UI::Xaml::Data::Bindable]
        public ref class StandardCalculatorViewModel sealed : public Windows::UI::Xaml::Data::INotifyPropertyChanged
        {
        public:
            StandardCalculatorViewModel();
            void UpdateOperand(int pos, Platform::String^ text);
            void UpdatecommandsInRecordingMode();
            int GetBitLengthType();
            int GetNumberBase();

            OBSERVABLE_OBJECT_CALLBACK(OnPropertyChanged);
            OBSERVABLE_PROPERTY_RW(Platform::String^, DisplayValue);
            OBSERVABLE_PROPERTY_RW(HistoryViewModel^, HistoryVM);
            OBSERVABLE_PROPERTY_RW(bool, IsInError);
            OBSERVABLE_PROPERTY_RW(bool, IsOperatorCommand);
            OBSERVABLE_PROPERTY_RW(Platform::String^, DisplayStringExpression);
            OBSERVABLE_PROPERTY_RW(Windows::Foundation::Collections::IVector<Common::DisplayExpressionToken^>^, ExpressionTokens);
            OBSERVABLE_PROPERTY_RW(Platform::String^, DecimalDisplayValue);
            OBSERVABLE_PROPERTY_RW(Platform::String^, HexDisplayValue);
            OBSERVABLE_PROPERTY_RW(Platform::String^, OctalDisplayValue);
            OBSERVABLE_PROPERTY_RW(Platform::String^, BinaryDisplayValue);
            OBSERVABLE_PROPERTY_RW(Platform::String^, HexDisplayValue_AutomationName);
            OBSERVABLE_PROPERTY_RW(Platform::String^, DecDisplayValue_AutomationName);
            OBSERVABLE_PROPERTY_RW(Platform::String^, OctDisplayValue_AutomationName);
            OBSERVABLE_PROPERTY_RW(Platform::String^, BinDisplayValue_AutomationName);
            OBSERVABLE_PROPERTY_RW(bool, IsBinaryOperatorEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsUnaryOperatorEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsNegateEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsDecimalEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsCurrentViewPinned);
            OBSERVABLE_PROPERTY_RW(Windows::Foundation::Collections::IVector<MemoryItemViewModel^>^, MemorizedNumbers);
            OBSERVABLE_PROPERTY_RW(bool, IsMemoryEmpty);
            OBSERVABLE_PROPERTY_RW(bool, IsFToEChecked);
            OBSERVABLE_PROPERTY_RW(bool, IsFToEEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsHyperbolicChecked);
            OBSERVABLE_PROPERTY_RW(bool, AreHEXButtonsEnabled);
            NAMED_OBSERVABLE_PROPERTY_RW(Platform::String^, CalculationResultAutomationName);
            NAMED_OBSERVABLE_PROPERTY_RW(Platform::String^, CalculationExpressionAutomationName);
            OBSERVABLE_PROPERTY_RW(bool, IsShiftProgrammerChecked);
            OBSERVABLE_PROPERTY_RW(bool, IsQwordEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsDwordEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsWordEnabled);
            OBSERVABLE_PROPERTY_RW(bool, IsByteEnabled);
            OBSERVABLE_PROPERTY_RW(Platform::String^, OpenParenthesisCount);
            OBSERVABLE_PROPERTY_RW(int, CurrentRadixType);
            OBSERVABLE_PROPERTY_RW(bool, AreTokensUpdated);
            OBSERVABLE_PROPERTY_RW(bool, AreHistoryShortcutsEnabled);
            OBSERVABLE_PROPERTY_RW(bool, AreProgrammerRadixOperatorsEnabled);
            OBSERVABLE_PROPERTY_RW(CalculatorApp::Common::Automation::NarratorAnnouncement^, Announcement);

            COMMAND_FOR_METHOD(CopyCommand, StandardCalculatorViewModel::OnCopyCommand);
            COMMAND_FOR_METHOD(PasteCommand, StandardCalculatorViewModel::OnPasteCommand);
            COMMAND_FOR_METHOD(ButtonPressed, StandardCalculatorViewModel::OnButtonPressed);
            COMMAND_FOR_METHOD(ClearMemoryCommand, StandardCalculatorViewModel::OnClearMemoryCommand);
            COMMAND_FOR_METHOD(PinUnpinAppBarButtonOnClicked, StandardCalculatorViewModel::OnPinUnpinCommand);
            COMMAND_FOR_METHOD(MemoryItemPressed, StandardCalculatorViewModel::OnMemoryItemPressed);
            COMMAND_FOR_METHOD(MemoryAdd, StandardCalculatorViewModel::OnMemoryAdd);
            COMMAND_FOR_METHOD(MemorySubtract, StandardCalculatorViewModel::OnMemorySubtract);

            event HideMemoryClickedHandler^ HideMemoryClicked;
            event ProgModeRadixChangeHandler^ ProgModeRadixChange;

            property bool IsShiftChecked {
                bool get() { return m_isShiftChecked; }
                void set(bool value)
                {
                    if (m_isShiftChecked != value)
                    {
                        m_isShiftChecked = value;
                        RaisePropertyChanged(L"IsShiftChecked");
                    }
                }
            }

            property bool IsBitFlipChecked {
                bool get() { return m_isBitFlipChecked; }
                void set(bool value)
                {
                    if (m_isBitFlipChecked != value)
                    {
                        m_isBitFlipChecked = value;
                        IsBinaryBitFlippingEnabled = IsProgrammer && m_isBitFlipChecked;
                        AreProgrammerRadixOperatorsEnabled = IsProgrammer && !m_isBitFlipChecked;
                        RaisePropertyChanged(L"IsBitFlipChecked");
                    }
                }
            }

            property bool IsBinaryBitFlippingEnabled {
                bool get() { return m_isBinaryBitFlippingEnabled; }
                void set(bool value)
                {
                    if (m_isBinaryBitFlippingEnabled != value)
                    {
                        m_isBinaryBitFlippingEnabled = value;
                        RaisePropertyChanged(L"IsBinaryBitFlippingEnabled");
                    }
                }
            }

            property bool IsStandard {
                bool get() { return m_isStandard; }
                void set(bool value)
                {
                    if (m_isStandard != value)
                    {
                        m_isStandard = value;
                        if (value)
                        {
                            IsScientific = false;
                            IsProgrammer = false;
                        }
                        RaisePropertyChanged(L"IsStandard");
                    }
                }
            }

            property bool IsScientific {
                bool get() { return m_isScientific; }
                void set(bool value)
                {
                    if (m_isScientific != value)
                    {
                        m_isScientific = value;
                        if (value)
                        {
                            IsStandard = false;
                            IsProgrammer = false;
                        }
                        RaisePropertyChanged(L"IsScientific");
                    }
                }
            }

            property bool IsProgrammer {
                bool get() { return m_isProgrammer; }
                void set(bool value)
                {
                    if (m_isProgrammer != value)
                    {
                        m_isProgrammer = value;
                        if (!m_isProgrammer)
                        {
                            IsBitFlipChecked = false;
                        }
                        IsBinaryBitFlippingEnabled = m_isProgrammer && IsBitFlipChecked;
                        AreProgrammerRadixOperatorsEnabled = m_isProgrammer && !IsBitFlipChecked;
                        if (value)
                        {
                            IsStandard = false;
                            IsScientific = false;
                        }
                        RaisePropertyChanged(L"IsProgrammer");
                    }
                }
            }

            property bool  IsEditingEnabled {
                bool get() { return m_isEditingEnabled; }
                void set(bool value) {
                    if (m_isEditingEnabled != value)
                    {
                        //                        Numbers::Common::KeyboardShortcutManager::IsCalculatorInEditingMode = value;
                        m_isEditingEnabled = value;
                        bool currentEditToggleValue = !m_isEditingEnabled;
                        IsBinaryOperatorEnabled = currentEditToggleValue;
                        IsUnaryOperatorEnabled = currentEditToggleValue;
                        IsOperandEnabled = currentEditToggleValue;
                        IsNegateEnabled = currentEditToggleValue;
                        IsDecimalEnabled = currentEditToggleValue;
                        RaisePropertyChanged(L"IsEditingEnabled");
                    }
                }
            }

            property bool IsEngineRecording {
                bool get() { return m_standardCalculatorManager.IsEngineRecording(); }
            }

            property bool  IsOperandEnabled {
                bool get() { return m_isOperandEnabled; }
                void set(bool value) {
                    if (m_isOperandEnabled != value)
                    {
                        m_isOperandEnabled = value;
                        IsDecimalEnabled = value;
                        AreHEXButtonsEnabled = IsProgrammer;
                        IsFToEEnabled = value;
                        RaisePropertyChanged(L"IsOperandEnabled");
                    }
                }
            }

            property int TokenPosition
            {
                int get() { return m_tokenPosition; }
                void set(int value) { m_tokenPosition = value; }
            }

            property Platform::String^ SelectedExpressionLastData
            {
                Platform::String^ get() { return m_selectedExpressionLastData; }
                void set(Platform::String^ value) { m_selectedExpressionLastData = value; }
            }

            property bool KeyPressed
            {
                bool get() { return m_keyPressed; }
                void set(bool value) { m_keyPressed = value; }
            }

            property bool IsOperandUpdatedUsingViewModel
            {
                bool get() { return m_operandUpdated; }
                void set(bool value) { m_operandUpdated = value; }
            }

            property bool IsOperandTextCompletelySelected
            {
                bool get() { return m_completeTextSelection; }
                void set(bool value) { m_completeTextSelection = value; }
            }

            property Platform::String^ LeftParenthesisAutomationName
            {
                Platform::String^ get() 
                {
                    return GetLeftParenthesisAutomationName();
                }
            }

        internal:
            void OnPaste(Platform::String^ pastedString, CalculatorApp::Common::ViewMode mode);
            void OnCopyCommand(Platform::Object^ parameter);
            void OnPasteCommand(Platform::Object^ parameter);

            NumbersAndOperatorsEnum MapCharacterToButtonId(const wchar_t ch, bool& canSendNegate);

            //Memory feature related methods. They are internal because they need to called from the MainPage code-behind
            void OnMemoryButtonPressed();
            void OnMemoryItemPressed(Platform::Object^ memoryItemPosition);
            void OnMemoryAdd(Platform::Object^ memoryItemPosition);
            void OnMemorySubtract(Platform::Object^ memoryItemPosition);
            void OnMemoryClear(_In_ Platform::Object^ memoryItemPosition);
            void OnPinUnpinCommand(Platform::Object^ parameter);

            void SetPrimaryDisplay(_In_ std::wstring const&displayString, _In_ bool isError);
            void DisplayPasteError();
            void SetTokens(_Inout_ std::shared_ptr<CalculatorVector<std::pair<std::wstring, int>>> const &tokens);
            void SetExpressionDisplay(_Inout_ std::shared_ptr<CalculatorVector<std::pair<std::wstring, int>>> const &tokens, _Inout_ std::shared_ptr<CalculatorVector<std::shared_ptr<IExpressionCommand>>> const &commands);
            void SetHistoryExpressionDisplay(_Inout_ std::shared_ptr<CalculatorVector<std::pair<std::wstring, int>>> const &tokens, _Inout_ std::shared_ptr<CalculatorVector <std::shared_ptr<IExpressionCommand>>> const &commands);
            void SetParenthesisCount(_In_ const std::wstring& parenthesisCount);
            void OnMaxDigitsReached();
            void OnBinaryOperatorReceived();
            void OnMemoryItemChanged(unsigned int indexOfMemory);

            Platform::Array<unsigned char>^ Serialize();
            void Deserialize(Platform::Array<unsigned char>^ state);

            Platform::String^ GetLocalizedStringFormat(Platform::String^ format, Platform::String^ displayValue);
            void OnPropertyChanged(Platform::String^ propertyname);
            void SetCalculatorType(CalculatorApp::Common::ViewMode targetState);

            Platform::String^ GetRawDisplayValue();
            void Recalculate(bool fromHistory = false);
            bool IsOperator(CalculationManager::Command cmdenum);
            void FtoEButtonToggled();
            void SwitchProgrammerModeBase(RADIX_TYPE calculatorBase);
            void SetMemorizedNumbersString();
            void SwitchAngleType(NumbersAndOperatorsEnum num);
            void ResetDisplay();
            RADIX_TYPE GetCurrentRadixType() { return (RADIX_TYPE)m_CurrentRadixType; }
            void SetPrecision(int32_t precision);
            void UpdateMaxIntDigits() { m_standardCalculatorManager.UpdateMaxIntDigits(); }
            NumbersAndOperatorsEnum GetCurrentAngleType() { return m_CurrentAngleType; }

        private:
            void SetMemorizedNumbers(const std::vector<std::wstring>& memorizedNumbers);
            void UpdateProgrammerPanelDisplay();
            void HandleUpdatedOperandData(CalculationManager::Command cmdenum);
            NumbersAndOperatorsEnum ConvertIntegerToNumbersAndOperatorsEnum(unsigned int parameter);
            NumbersAndOperatorsEnum m_CurrentAngleType;
            wchar_t m_decimalSeparator;
            CalculatorDisplay m_calculatorDisplay;
            CalculatorApp::EngineResourceProvider m_resourceProvider;
            CalculationManager::CalculatorManager m_standardCalculatorManager;
            Platform::String^ m_expressionAutomationNameFormat;
            Platform::String^ m_localizedCalculationResultAutomationFormat;
            Platform::String^ m_localizedCalculationResultDecimalAutomationFormat;
            Platform::String^ m_localizedHexaDecimalAutomationFormat;
            Platform::String^ m_localizedDecimalAutomationFormat;
            Platform::String^ m_localizedOctalAutomationFormat;
            Platform::String^ m_localizedBinaryAutomationFormat;
            Platform::String^ m_localizedMaxDigitsReachedAutomationFormat;
            Platform::String^ m_localizedButtonPressFeedbackAutomationFormat;
            Platform::String^ m_localizedMemorySavedAutomationFormat;
            Platform::String^ m_localizedMemoryItemChangedAutomationFormat;
            Platform::String^ m_localizedMemoryItemClearedAutomationFormat;
            Platform::String^ m_localizedMemoryCleared;

            bool m_pinned;
            bool m_isOperandEnabled;
            bool m_isEditingEnabled;
            bool m_isStandard;
            bool m_isScientific;
            bool m_isProgrammer;
            bool m_isBinaryBitFlippingEnabled;
            bool m_isBitFlipChecked;
            bool m_isShiftChecked;
            bool m_isRtlLanguage;
            int m_tokenPosition;
            bool m_keyPressed;
            bool m_operandUpdated;
            bool m_completeTextSelection;
            bool m_isLastOperationHistoryLoad;
            Platform::String^ m_selectedExpressionLastData;
            Common::DisplayExpressionToken^ m_selectedExpressionToken;
            Platform::String^ m_leftParenthesisAutomationFormat;

            Platform::String^ LocalizeDisplayValue(_In_ std::wstring const &displayValue, _In_ bool isError);
            Platform::String^ CalculateNarratorDisplayValue(_In_ std::wstring const &displayValue, _In_ Platform::String^ localizedDisplayValue, _In_ bool isError);
            CalculatorApp::Common::Automation::NarratorAnnouncement^ GetDisplayUpdatedNarratorAnnouncement();
            Platform::String^ GetCalculatorExpressionAutomationName();
            Platform::String^ GetNarratorStringReadRawNumbers(_In_ Platform::String^ localizedDisplayValue);

            CalculationManager::Command ConvertToOperatorsEnum(NumbersAndOperatorsEnum operation);
            void DisableButtons(CalculationManager::CommandType selectedExpressionCommandType);
            Platform::String^ GetLeftParenthesisAutomationName();

            Platform::String^ m_feedbackForButtonPress;
            void OnButtonPressed(Platform::Object^ parameter);
            void OnClearMemoryCommand(Platform::Object^ parameter);
            std::wstring AddPadding(std::wstring);
            size_t LengthWithoutPadding(std::wstring);

            std::shared_ptr<CalculatorVector<std::pair<std::wstring, int>>> m_tokens;
            std::shared_ptr<CalculatorVector <std::shared_ptr<IExpressionCommand>>> m_commands;

            // Token types
            bool IsUnaryOp(int nOpCode);
            bool IsBinOp(int nOpcode);
            bool IsTrigOp(int nOpCode);
            bool IsOpnd(int nOpCode);
            bool IsRecoverableCommand(int nOpCode);

            CalculationManager::CommandType GetSelectedTokenType(_In_ unsigned int);
            void SaveEditedCommand(_In_ unsigned int index, _In_ CalculationManager::Command command);

            bool IsViewPinned();
            void SetViewPinnedState(bool pinned);

            friend class CalculatorDisplay;
            friend class CalculatorFunctionalTests::HistoryTests;
            friend class CalculatorUnitTests::MultiWindowUnitTests;
            friend class CalculatorUnitTests::TimerTests;
        };
    }
}


*/
