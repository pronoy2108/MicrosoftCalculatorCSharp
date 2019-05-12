// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CalculationManager
{
    public class ISerializeCommandVisitor
    {
        void Visit(COpndCommand opndCmd);
        void Visit(CUnaryCommand unaryCmd);
        void Visit(CBinaryCommand binaryCmd);
        void Visit(CParentheses paraCmd);
    };


    public interface IExpressionCommand
    {
        CalculationManager.CommandType GetCommandType();
        void Accept(ISerializeCommandVisitor commandVisitor);
    }

    public interface IOperatorCommand : IExpressionCommand
    {
        void SetCommand(int command);
    }

    public interface IUnaryCommand : IOperatorCommand
    {
        const std::shared_ptr<CalculatorVector<int>>& GetCommands();
        void SetCommands(int command1, int command2);
    }

    public interface IBinaryCommand : IOperatorCommand
    {
        new void SetCommand(int command);
        int GetCommand();
    }

    public interface IOpndCommand : IExpressionCommand
    {
        const std::shared_ptr<CalculatorVector<int>>& GetCommands();
        void AppendCommand(int command);
        void ToggleSign();
        void RemoveFromEnd();
        bool IsNegative();
        bool IsSciFmt();
        bool IsDecimalPresent();
        string GetToken(char decimalSymbol);
        void SetCommands(std::shared_ptr<CalculatorVector<int>> const& commands);
    }

    public interface IParenthesisCommand : IExpressionCommand
    {
        int GetCommand();
    }
}