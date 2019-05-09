using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;
using Windows.UI.Xaml;

namespace WindowsCalculator.Shared.Converters
{
	class ExpressionItemTemplateSelector : ConverterBase
	{
		Style ExpressionItemContainerStyle.SelectStyleCore(Object item, DependencyObject container)
        {
            DisplayExpressionToken token = dynamic_cast<DisplayExpressionToken>(item);

            if (token != nullptr)
            {
                Common.TokenType type = token.Type;

                switch (type)
                {
                case TokenType.Operator:
                    if (token.IsTokenEditable)
                    {
                        return m_editableOperatorStyle;
                    }
                    else
                    {
                        return m_nonEditableOperatorStyle;
                    }
                case TokenType.Operand:
                    return m_operandStyle;
                case TokenType.Separator:
                    return m_separatorStyle;
                default:
                    throw new Platform.Exception(E_FAIL, "Invalid token type");
                }
            }

            return m_separatorStyle;
        }
	}
}
