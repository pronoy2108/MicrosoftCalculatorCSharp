using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Interop;
using CalculatorApp.Common.Automation;
using Windows.Storage;
using Platform.Collections;

namespace WindowsCalculator.Shared.ViewModels
{
	// MAX?
	//public delegate void HistoryItemClickedHandler(HistoryItemViewModel e);

	[Bindable]
	sealed public class HistoryViewModel : INotifyPropertyChanged
	{
		// PUBLIC 
		/* MAX?
		PUBLIC OBSERVABLE_OBJECT();
		*/
		public int ItemSize { get; set; }
		public IBindableObservableVector Items { get; set; }
		public bool AreHistoryShortcutsEnabled { get; set; }
		public NarratorAnnouncement HistoryAnnouncement { get; set; }

		private CalculationManager.CalculatorManager m_calculatorManager;
		private CalculatorDisplay m_calculatorDisplay;
		private CalculationManager.CALCULATOR_MODE m_currentMode;
		private string m_localizedHistoryCleared;

		public HistoryViewModel(CalculationManager.CalculatorManager calculatorManager) :
			m_calculatorManager(calculatorManager),
			m_localizedHistoryCleared(null)
		{
			AreHistoryShortcutsEnabled = true;

			Items = new Vector<HistoryItemViewModel> ();
			ItemSize = 0;
		}

		public void RestoreCompleteHistory()
		{
			RestoreHistory(CalculationManager.CALCULATOR_MODE.CM_STD);
			RestoreHistory(CalculationManager.CALCULATOR_MODE.CM_SCI);
		}

		public void ReloadHistory(ViewMode currentMode)
		{
			if (currentMode == ViewMode.Standard)
			{
				m_currentMode = CalculationManager.CALCULATOR_MODE.CM_STD;
			}
			else if (currentMode == ViewMode.Scientific)
			{
				m_currentMode = CalculationManager.CALCULATOR_MODE.CM_SCI;
			}
			else
			{
				return;
			}

			var historyListModel = m_calculatorManager.GetHistoryItems(m_currentMode);
			var historyListVM = new Vector<HistoryItemViewModel> ();

			var localizer = LocalizationSettings.GetInstance();

			if (historyListModel.size() > 0)
			{
				for (var ritr = historyListModel.rbegin(); ritr != historyListModel.rend(); ++ritr)
				{
					string expression = ritr.historyItemVector.expression;
					string result = ritr.historyItemVector.result;

					localizer.LocalizeDisplayValue(expression);
					localizer.LocalizeDisplayValue(result);

					var item = new HistoryItemViewModel(
						expression.ToString(),
						result.ToString(),
						ritr.historyItemVector.spTokens, 
						ritr.historyItemVector.spCommands);
					historyListVM.Append(item);
				}
			}

			Items = historyListVM;
			UpdateItemSize();
		}


		public void OnHistoryItemAdded(uint addedItemIndex)
		{
			var newItem = m_calculatorManager.GetHistoryItem(addedItemIndex);
			var localizer = LocalizationSettings.GetInstance();
			string expression = newItem.historyItemVector.expression;
			string result = newItem.historyItemVector.result;

			localizer.LocalizeDisplayValue(expression);
			localizer.LocalizeDisplayValue(result);

			var item = new HistoryItemViewModel(expression, result, newItem.historyItemVector.spTokens, newItem.historyItemVector.spCommands);

			// check if we have not hit the max items
			if (Items.Length >= m_calculatorManager.MaxHistorySize())
			{
				// this means the item already exists
				Items.RemoveAt(Items.Length - 1);
			}

			assert(addedItemIndex <= m_calculatorManager.MaxHistorySize() && addedItemIndex >= 0);
			Items.InsertAt(0, item);
			UpdateItemSize();
			SaveHistory();
		}

		/* MAX?
		COMMAND_FOR_METHOD(HideCommand, OnHideCommand);
		*/
		public void OnHideCommand(Object e)
		{
			// added at VM layer so that the views do not have to individually raise events
			HideHistoryClicked();
		}

		/* MAX?
		COMMAND_FOR_METHOD(ClearCommand, OnClearCommand);
		*/
		public void OnClearCommand(Object e)
		{
			TraceLogger.GetInstance().LogClearHistory();
			if (AreHistoryShortcutsEnabled == true)
			{
				m_calculatorManager.ClearHistory();

				if (Items.Size > 0)
				{
					CalculationManager.CALCULATOR_MODE currentMode = m_currentMode;
					ClearHistoryContainer(currentMode);
					Items.Clear();
					UpdateItemSize();
				}

				MakeHistoryClearedNarratorAnnouncement(HistoryResourceKeys.HistoryCleared, m_localizedHistoryCleared);
			}
		}

		// events that are created
		public event HideHistoryClickedHandler HideHistoryClicked;
		/* MAX?
		public delegate void HideHistoryClickedHandler();
		*/

		public event HistoryItemClickedHandler HistoryItemClicked;
		/* MAX?
		public delegate void HistoryItemClickedHandler(HistoryItemViewModel e);
		*/

		public void ShowItem(HistoryItemViewModel e)
		{
			HistoryItemClicked(e);
		}
		
		public void RestoreCompleteHistory()
		{
			RestoreHistory(CalculationManager.CALCULATOR_MODE.CM_STD);
			RestoreHistory(CalculationManager.CALCULATOR_MODE.CM_SCI);
		}

		// PRIVATE

		private void RestoreHistory(CalculationManager.CALCULATOR_MODE cMode)
		{
			ApplicationDataContainer historyContainer = GetHistoryContainer(cMode);
			/* MAX?
			std::shared_ptr<std::vector<std::shared_ptr<CalculationManager::HISTORYITEM>>> historyVector = std::make_shared<std::vector<std::shared_ptr<CalculationManager::HISTORYITEM>>>();
			*/
			Vector<CalculationManager.HISTORYITEM> historyVector = Vector<CalculationManager.HISTORYITEM>();
			var historyVectorLength = historyContainer.Values.Lookup(HistoryVectorLengthKey);
			bool failure = false;

			if (historyVectorLength > 0)
			{
				for (int i = 0; i < historyVectorLength; ++i)
				{
					try
					{
						// deserialize each item
						var item = DeserializeHistoryItem(i.ToString(), historyContainer);
						CalculationManager.HISTORYITEM Item = new CalculationManager.HISTORYITEM(item);
						historyVector.push_back(Item);
					}
					catch (Exception e)
					{
						failure = true;
						break;
					}
				}
			}

			if (!failure)
			{
				// if task has been cancelled set history to 0
				m_calculatorManager.SetHistory(cMode, historyVector);

				// update length once again for consistency between stored number of items and length
				UpdateHistoryVectorLength(historyVector.Length, cMode);
			}
			else
			{
				// in case of failure do not show any item
				UpdateHistoryVectorLength(0, cMode);
			}
		}

		private CalculationManager.HISTORYITEM DeserializeHistoryItem(string historyItemKey, ApplicationDataContainer historyContainer)
		{

			CalculationManager.HISTORYITEM historyItem;

			if (historyContainer.Values.HasKey(historyItemKey))
			{
				Object historyItemValues = historyContainer.Values.Lookup(historyItemKey);

				if (historyItemValues == null)
				{
					throw new Exception(E_POINTER, "History Item is NULL");
				}

				string historyData = historyItemValues.ToString(); // MAX? safe_cast < Platform::String ^> (historyItemValues);
				IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(historyData);

				if (buffer == null)
				{
					throw new Exception(E_POINTER, "History Item is NULL");
				}

				DataReader reader = DataReader.FromBuffer(buffer);
				var exprLen = reader.ReadUInt32();
				var expression = reader.ReadString(exprLen);
				historyItem.historyItemVector.expression = expression.Data();

				var resultLen = reader.ReadUInt32();
				var result = reader.ReadString(resultLen);
				historyItem.historyItemVector.result = result.Data();

				historyItem.historyItemVector.spCommands = Utils.DeserializeCommands(reader);
				historyItem.historyItemVector.spTokens = Utils.DeserializeTokens(reader);
			}
			else
			{
				throw new Exception(E_ACCESSDENIED, "History Item not found");
			}

			return historyItem;
		}

		private ApplicationDataContainer GetHistoryContainer(CalculationManager.CALCULATOR_MODE cMode)
		{
			ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
			ApplicationDataContainer historyContainer;

			// naming container based on mode
			string historyContainerKey = GetHistoryContainerKey(cMode);

			if (localSettings.Containers.HasKey(historyContainerKey))
			{
				historyContainer = localSettings.Containers.Lookup(historyContainerKey);
			}
			else
			{
				// create container for adding data
				historyContainer = localSettings.CreateContainer(historyContainerKey, ApplicationDataCreateDisposition.Always);
				int initialHistoryVectorLength = 0;
				historyContainer.Values.Insert(HistoryVectorLengthKey, initialHistoryVectorLength);
			}

			return historyContainer;
		}

		private string GetHistoryContainerKey(CalculationManager.CALCULATOR_MODE cMode)
		{
			ValueType modeValue = (int)cMode;
			return string.Concat(modeValue.ToString(), "_History");
		}

		private void ClearHistoryContainer(CalculationManager.CALCULATOR_MODE cMode)
		{
			ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
			localSettings.DeleteContainer(GetHistoryContainerKey(cMode));
		}

		private void ClearHistory()
		{
			ClearHistoryContainer(CalculationManager.CALCULATOR_MODE.CM_STD);
			ClearHistoryContainer(CalculationManager.CALCULATOR_MODE.CM_SCI);
		}

		private void UpdateHistoryVectorLength(int newValue, CalculationManager.CALCULATOR_MODE cMode)
		{
			ApplicationDataContainer historyContainer = GetHistoryContainer(cMode);
			historyContainer.Values.Remove(HistoryVectorLengthKey);
			historyContainer.Values.Insert(HistoryVectorLengthKey, newValue);
		}

		private bool IsValid(CalculationManager.HISTORYITEM item)
		{
			return (!item.historyItemVector.expression.empty() && !item.historyItemVector.result.empty() && (bool)item.historyItemVector.spCommands && (bool)item.historyItemVector.spTokens);
		}

		private void SaveHistory()
		{
			ApplicationDataContainer historyContainer = GetHistoryContainer(m_currentMode);
			var currentHistoryVector = m_calculatorManager.GetHistoryItems(m_currentMode);
			bool failure = false;
			int index = 0;
			string serializedHistoryItem;

			for (var iter = currentHistoryVector.begin(); iter != currentHistoryVector.end(); ++iter)
			{
				try
				{
					serializedHistoryItem = SerializeHistoryItem(iter);
					historyContainer.Values.Insert(index.ToString(), serializedHistoryItem);
				}
				catch (Exception)
				{
					failure = true;
					break;
				}
				
				++index;
			}

			if (!failure)
			{
				// insertion is successful
				UpdateHistoryVectorLength(currentHistoryVector.Length, m_currentMode);
			}
			else
			{
				UpdateHistoryVectorLength(0, m_currentMode);
			}
		}

		// this serializes a history item into a base64 encoded string
		private string SerializeHistoryItem(CalculationManager.HISTORYITEM item)
		{
			HRESULT hr = S_OK;
			DataWriter writer = new DataWriter();
			var expr = item.historyItemVector.expression;
			var result = item.historyItemVector.result;
			var platformExpr = expr.ToString();

			writer.WriteUInt32(writer.MeasureString(platformExpr));
			writer.WriteString(platformExpr);
			var platformResult = result.ToString();

			writer.WriteUInt32(writer.MeasureString(platformResult));
			writer.WriteString(platformResult);

			Utils.SerializeCommandsAndTokens(item.historyItemVector.spTokens, item.historyItemVector.spCommands, writer);

			IBuffer buffer = writer.DetachBuffer();
			if (buffer == null)
			{
				throw new Platform.Exception(E_POINTER, "History Item is NULL");
			}

			return CryptographicBuffer.EncodeToBase64String(buffer);
		}

		private void MakeHistoryClearedNarratorAnnouncement(string resourceKey, string formatVariable);
		{
			String announcement = LocalizationStringUtil.GetLocalizedNarratorAnnouncement(resourceKey, formatVariable);
			HistoryAnnouncement = CalculatorAnnouncement.GetHistoryClearedAnnouncement(announcement);
		}

		/* MAX?
		friend class CalculatorDisplay;
		*/
		private void UpdateItemSize()
		{
			ItemSize = Items.Length;
		}
	}
}