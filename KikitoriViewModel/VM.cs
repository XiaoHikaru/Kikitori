// Kikitori
// (C) 2021, Andreas Gaiser

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kikitori.ViewModel
{
    abstract public class VM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public abstract void NotifyAll();
    }
}
