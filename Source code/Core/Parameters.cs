﻿using System.ComponentModel;

namespace Elysium.Core
{
    public class Parameters : INotifyPropertyChanged
    {
        public static readonly Parameters Instance = new Parameters();

        public uint AccentColor
        {
            get { return _accentColor; }
            set
            {
                _accentColor = value;
                OnPropertyChanged("AccentColor");
            }
        }

        private uint _accentColor;

        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged("IsDarkTheme");
            }
        }

        private bool _isDarkTheme;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} ;