using Lab1.Cipher;
using Lab1.Models.Alphabets;
using Lab1.Models.Operations;
using Lab1.Services;
using Lab1.Services.Input;
using Lab1.Services.Message;
using Lab1.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lab1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly Page _page;
    private readonly IMessageService _messages;
    private readonly IDataInstaller _dataInstaller;
    private readonly InputValidator _validator;
    private readonly IClipboardService _clipboard;

    private Alphabet _selectedAlphabet;
    private Operation _selectedOperation;
    private int _selectedShift;
    private string _inputText = string.Empty;
    private string _outputText = string.Empty;
    private int? _outputShift;

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
            OnPropertyChanged(nameof(OutputShiftVisibility));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public int? OutputShift
    {
        get => _outputShift;
        set
        {
            SetField(ref _outputShift, value);
            OnPropertyChanged(nameof(OutputShiftLabel));
        }
    }

    public string OutputShiftLabel => OutputShift.HasValue ? $"Сдвиг: {OutputShift}" : string.Empty;

    public Visibility ShiftVisibility { get; private set; } = Visibility.Visible;

    public Visibility OutputShiftVisibility => !string.IsNullOrWhiteSpace(OutputText) && _selectedOperation.Type != OperationType.Decrypt ? Visibility.Visible : Visibility.Collapsed;

    public string ErrorMessage => _messages.Message;
    public MessageType ErrorType => _messages.Type;
    public Visibility ErrorVisibility => _messages.HasMessage ? Visibility.Visible : Visibility.Collapsed;

    public ICommand PasteCommand { get; }
    public ICommand CalculateCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand CopyShiftCommand { get; }

    public MainViewModel(Page page)
    {
        _page = page;
        _messages = new MessageService();
        _dataInstaller = new DefaultDataInstaller();
        _validator = new InputValidator();
        _clipboard = new ClipboardService();

        Alphabets = _dataInstaller.GetAlphabets();
        Operations = _dataInstaller.GetOperations();

        SelectedAlphabet = Alphabets.First();
        SelectedOperation = Operations.First();

        PasteCommand = new RelayCommand(ExecutePaste);
        CalculateCommand = new RelayCommand(ExecuteCalculate, CanExecuteCalculate);
        CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);
        CopyShiftCommand = new RelayCommand(ExecuteCopyShift, CanExecuteCopyShift);

        CommandManager.RequerySuggested += (s, e) =>
        {
            ((RelayCommand)CalculateCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CopyCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CopyShiftCommand).RaiseCanExecuteChanged();
        };
    }

    private void RefreshErrorBindings()
    {
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(ErrorType));
        OnPropertyChanged(nameof(ErrorVisibility));
    }

    private void ExecutePaste()
    {
        try
        {
            InputText = _clipboard.Paste();
        }
        catch (Exception ex)
        {
            _messages.ShowError($"Ошибка: {ex.Message}");
            RefreshErrorBindings();
        }
    }

    private void ExecuteCalculate()
    {
        try
        {
            InputValidationResult result = _validator.Validate(InputText.ToLower(), SelectedAlphabet);

            if (!result.IsValid)
            {
                _messages.ShowError(result.Message);
                OutputText = string.Empty;
                OutputShift = null;
            }
            else
            {
                OutputText = ApplyCipher();
                if (result.Type == MessageType.Warning)
                    _messages.ShowWarning(result.Message);
                else
                    _messages.Clear();
            }
        }
        catch (Exception ex)
        {
            _messages.ShowError($"Ошибка при обработке: {ex.Message}");
            OutputText = string.Empty;
            OutputShift = null;
        }
        finally
        {
            RefreshErrorBindings();
        }
    }

    private string ApplyCipher()
    {
        switch (_selectedOperation.Type)
        {
            case OperationType.Decrypt:
                OutputShift = SelectedShift;
                return CaesarCipher.Decrypt(InputText, SelectedAlphabet, SelectedShift);

            case OperationType.Encrypt:
                OutputShift = SelectedShift;
                return CaesarCipher.Encrypt(InputText, SelectedAlphabet, SelectedShift);

            case OperationType.Cryptanalyze:
                (string Result, int Shift) result = CaesarCipher.Cryptanalyze(InputText, SelectedAlphabet);
                OutputShift = result.Shift;
                return result.Result;

            default:
                throw new InvalidOperationException("Неизвестный тип операции");
        }
    }

    private bool CanExecuteCalculate() => !string.IsNullOrWhiteSpace(InputText);

    private void ExecuteCopy()
    {
        try
        {
            _clipboard.Copy(OutputText);
        }
        catch (Exception ex)
        {
            _messages.ShowError($"Ошибка при копировании: {ex.Message}");
            RefreshErrorBindings();
        }
    }

    private bool CanExecuteCopy() => !string.IsNullOrWhiteSpace(OutputText);

    private void ExecuteCopyShift()
    {
        try
        {
            if (OutputShift.HasValue)
                _clipboard.Copy(OutputShift.Value.ToString());
        }
        catch (Exception ex)
        {
            _messages.ShowError($"Ошибка при копировании сдвига: {ex.Message}");
            RefreshErrorBindings();
        }
    }

    private bool CanExecuteCopyShift() => OutputShift.HasValue;

    private void UpdateShifts()
    {
        Shifts = Enumerable.Range(0, SelectedAlphabet.MaxShift + 1).ToList();
        OnPropertyChanged(nameof(Shifts));
        SelectedShift = Math.Min(SelectedShift, SelectedAlphabet.MaxShift);
    }

    private void UpdateShiftVisibility()
    {
        ShiftVisibility = SelectedOperation.Type != OperationType.Cryptanalyze
            ? Visibility.Visible
            : Visibility.Collapsed;
        OnPropertyChanged(nameof(ShiftVisibility));
        VisualStateManager.GoToState(_page, ShiftVisibility == Visibility.Visible ? "ShiftComboBoxVisible" : "ShiftComboBoxHidden", true);
    }
}