using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Finance_Pro.Login
{
    public class LoginViewModel
    {
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            RegisterCommand = new RelayCommand(OnRegister);
        }

        private void OnRegister()
        {
            MessageBox.Show("Bạn đã bấm vào Register.");
            // hoặc mở cửa sổ đăng ký mới
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
    }
}
