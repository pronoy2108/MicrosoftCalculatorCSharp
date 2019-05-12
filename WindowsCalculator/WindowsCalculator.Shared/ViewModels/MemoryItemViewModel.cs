// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CalculatorApp;
using CalculatorApp.Common;
using CalculatorApp.Common.Automation;
using CalculatorApp.ViewModel;
using Platform;
using std;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Foundation.Collections;
using System.ComponentModel;
using Windows.UI.Xaml.Data;
using System;

namespace CalculatorApp
{
    namespace ViewModel
    {



        /// <summary>
        /// Model representation of a single item in the Memory list
        /// </summary>
        [Windows.UI.Xaml.Data.Bindable]
        public class MemoryItemViewModel : INotifyPropertyChanged, ICustomPropertyProvider
        {
            StandardCalculatorViewModel m_calcVM;

            public MemoryItemViewModel(StandardCalculatorViewModel calcVM)
            {
                m_Position = -1;
                m_calcVM = calcVM;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string p = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
            private int m_Position;
            public int Position { get => m_Position; set { m_Position = value; RaisePropertyChanged("Position"); } }

            private string m_Value;
            public string Value { get => m_Value; set { m_Value = value; RaisePropertyChanged("Value"); } }

            public Type Type => GetType();
            public ICustomProperty GetCustomProperty(string name) => null;
            public ICustomProperty GetIndexedProperty(string name, Type type) => null;
            public string GetStringRepresentation() => Value;

            void Clear()
            {
                m_calcVM.OnMemoryClear(Position);
            }

            void MemoryAdd()
            {
                m_calcVM.OnMemoryAdd(Position);
            }

            void MemorySubtract()
            {
                m_calcVM.OnMemorySubtract(Position);
            }
        }
    }
}