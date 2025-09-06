using Lab1.Cipher;
using Lab1.Data;
using Lab1.Data.Alphabets;
using Lab1.Data.Operations;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Lab1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Alphabet _selectedAlphabet;
        private Operation _selectedOperation;
        private int _selectedShift;
        private string _inputText = string.Empty;
        private string _outputText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Alphabet> Alphabets { get; }
        public List<Operation> Operations { get; }
        public List<int> Shifts { get; private set; }

        public Alphabet SelectedAlphabet
        {
            get => _selectedAlphabet;
            set
            {
                SetField(ref _selectedAlphabet, value);
                UpdateShifts();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Operation SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                SetField(ref _selectedOperation, value);
                UpdateShiftVisibility();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int SelectedShift
        {
            get => _selectedShift;
            set => SetField(ref _selectedShift, value);
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                SetField(ref _inputText, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string OutputText
        {
            get => _outputText;
            set
            {
                SetField(ref _outputText, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Visibility ShiftVisibility { get; private set; } = Visibility.Visible;

        public ICommand PasteCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand CopyCommand { get; }

        public ViewModel()
        {
            Alphabets = CreateAlphabets();
            Operations = CreateOperations();

            SelectedAlphabet = Alphabets.First();
            SelectedOperation = Operations.First();

            PasteCommand = new RelayCommand(ExecutePaste);
            CalculateCommand = new RelayCommand(ExecuteCalculate, CanExecuteCalculate);
            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);

            CommandManager.RequerySuggested += (s, e) =>
            {
                ((RelayCommand)CalculateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CopyCommand).RaiseCanExecuteChanged();
            };
        }

        private async void ExecutePaste()
        {
            try
            {
                var dataPackage = Clipboard.GetContent();
                if (dataPackage.Contains(StandardDataFormats.Text))
                {
                    InputText = await dataPackage.GetTextAsync();
                }
            }
            catch (Exception ex)
            {
                OutputText = $"Ошибка: {ex.Message}";
            }
        }

        private void ExecuteCalculate()
        {
            try
            {
                OutputText = _selectedOperation.Type switch
                {
                    OperationType.Decode => CaesarCipher.Decode(InputText, SelectedAlphabet, SelectedShift),
                    OperationType.Encode => CaesarCipher.Encode(InputText, SelectedAlphabet, SelectedShift),
                    OperationType.Hack => CaesarCipher.Hack(InputText, SelectedAlphabet),
                    _ => throw new InvalidOperationException("Неизвестный тип операции")
                };
            }
            catch (Exception ex)
            {
                OutputText = $"Ошибка при обработке: {ex.Message}";
            }
        }

        private bool CanExecuteCalculate() => !string.IsNullOrWhiteSpace(InputText);

        private void ExecuteCopy()
        {
            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(OutputText);
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                OutputText = $"Ошибка при копировании: {ex.Message}";
            }
        }

        private bool CanExecuteCopy() => !string.IsNullOrWhiteSpace(OutputText);

        private void UpdateShifts()
        {
            Shifts = Enumerable.Range(0, SelectedAlphabet.MaxShift + 1).ToList();
            OnPropertyChanged(nameof(Shifts));
            SelectedShift = Math.Min(SelectedShift, SelectedAlphabet.MaxShift);
        }

        private void UpdateShiftVisibility()
        {
            ShiftVisibility = SelectedOperation.Type != OperationType.Hack
                ? Visibility.Visible
                : Visibility.Collapsed;
            OnPropertyChanged(nameof(ShiftVisibility));
        }

        private List<Alphabet> CreateAlphabets()
        {
            return new List<Alphabet>
            {
                AlphabetFactory.CreateRussianAlphabet(),
                AlphabetFactory.CreateEnglishAlphabet()
            };
        }

        private List<Operation> CreateOperations()
        {
            return new List<Operation>
            {
                new Operation(OperationType.Encode, "Зашифровать"),
                new Operation(OperationType.Decode, "Расшифровать"),
                new Operation(OperationType.Hack, "Взломать")
            };
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action<Exception> _errorHandler;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null, Action<Exception> errorHandler = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter))
                {
                    _execute();
                }
            }
            catch (Exception ex)
            {
                _errorHandler?.Invoke(ex);
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // Класс для управления командами
    public static class CommandManager
    {
        public static event EventHandler RequerySuggested;

        public static void InvalidateRequerySuggested()
        {
            RequerySuggested?.Invoke(null, EventArgs.Empty);
        }
    }
}