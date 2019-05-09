using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Interop;

namespace WindowsCalculator.Shared.ViewModels
{
	[Bindable]
	sealed public class MemoryItemViewModel; INotifyPropertyChanged, ICustomPropertyProvider
	{
		/* MAX?
		ref class StandardCalculatorViewModel;
		*/

		/// <summary>
		/// Model representation of a single item in the Memory list
		/// </summary>
        public MemoryItemViewModel(StandardCalculatorViewModel calcVM) : m_Position(-1), m_calcVM(calcVM)
		{

		}

		public virtual int Position { get; set; }
		public virtual string Value { get; set; }

		public virtual ICustomProperty GetCustomProperty(string name)
		{
			return null;
		}

		public virtual ICustomProperty GetIndexedProperty(string name, TypeName type)
		{
			return null;
		}

		public virtual TypeName Type
		{
			get{ return this.GetType(); }
		}

		public virtual string GetStringRepresentation()
		{
			return Value;
		}

		public void Clear();
		public void MemoryAdd();
		public void MemorySubtract();

		private StandardCalculatorViewModel m_calcVM;
}
/*

void MemoryItemViewModel::Clear()
{
    m_calcVM->OnMemoryClear(Position);
};

void MemoryItemViewModel::MemoryAdd()
{
    m_calcVM->OnMemoryAdd(Position);
};

void MemoryItemViewModel::MemorySubtract()
{
    m_calcVM->OnMemorySubtract(Position);
};

*/
