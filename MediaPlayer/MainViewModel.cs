using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MediaPlayer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Window _window;

        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public MainViewModel(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));

            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            MaximizeCommand = new RelayCommand(ExecuteMaximize);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private void ExecuteMinimize(object parameter)
        {
            _window.WindowState = WindowState.Minimized;
        }

        private void ExecuteMaximize(object parameter)
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void ExecuteClose(object parameter)
        {
            _window.Close();
        }


       

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
