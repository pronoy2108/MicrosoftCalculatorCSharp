using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Interop;

namespace WindowsCalculator.Shared.ViewModels
{
	[Bindable]
	sealed class HistoryItemViewModel : ICustomPropertyProvider
	{
         public internal HistoryItemViewModel(String expression,
				String result,
				CalculatorVector<pair<string, int>>> spTokens,
				CalculatorVector<IExpressionCommand> spCommands)
		{

		}

		internal const CalculatorVector<Vector<string, int>> GetTokens()
		{
			return m_spTokens;
		}

		internal const CalculatorVector<IExpressionCommand> GetCommands()
		{
			return m_spCommands;
		}

		//PUBLIC
		public string Expression
		{
			get { return m_expression; }
		}

		public string AccExpression
		{
			get { return m_accExpression; }
		}

		public string Result
		{
			get { return m_result; }
		}

		public string AccResult
		{
			get { return m_accResult; }
		}


		virtual ICustomProperty GetCustomProperty(string name)
		{
			return null;
		}

		virtual ICustomProperty GetIndexedProperty(string name, TypeName type)
		{
			return null;
		}

		virtual public TypeName Type
		{
			get
			{
				return this.GetType();
			}
		}

		virtual string GetStringRepresentation()
		{
			return m_accExpression + " " + m_accResult;
		}

		private static string GetAccessibleExpressionFromTokens(CalculatorVector<Vector<string, int>> spTokens, string fallbackExpression)
		{
			// updating accessibility names for expression and result
			wstringstream accExpression { };
			accExpression << "";

			uint nTokens;
			HRESULT hr = spTokens.GetSize(nTokens);

			if (SUCCEEDED(hr))
			{
				Vector<string, int> tokenItem;
				for (unsigned int i = 0; i<nTokens; i++)
				{
					hr = spTokens.GetAt(i, tokenItem);
					if (FAILED(hr))
					{
						break;
					}

					wstring token = tokenItem.first;
					accExpression << LocalizationService.GetNarratorReadableToken(StringReference(token.ToString())).Data();
				}
			}

			if (SUCCEEDED(hr))
			{
				string expressionSuffix { };
				hr = spTokens.GetExpressionSuffix(expressionSuffix);
				if (SUCCEEDED(hr))
				{
					accExpression << expressionSuffix;
				}
			}

			if (FAILED(hr))
			{
				return LocalizationService.GetNarratorReadableString(fallbackExpression);
			}
			else
			{
				return new String(accExpression.ToString());
			}
		}

		// PRIVATE
        private string m_expression;
        private string m_accExpression;
        private string m_accResult;
        private string m_result;
        private Object CalculatorVector<pair<string, int>> m_spTokens;
		private Object CalculatorVector<IExpressionCommand> m_spCommands;
	}
}
