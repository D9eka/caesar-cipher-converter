using Lab1.Cipher;
using Lab1.Data;
using Lab1.Data.Alphabets;
using Lab1.Data.Operations;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lab1
{
    public class ViewModel
    {
        private MainWindow mainWindow;
        private Alphabet[] availableAlphabets;
        private Operation[] availableOperations;

        private Alphabet currentAlphabet;
        private Operation currentOperation;
        private int currentShift;

        public ViewModel(MainWindow mainWindow, Alphabet[] availableAlphabet, Operation[] availableOperations)
        {
            this.mainWindow = mainWindow;
            this.availableAlphabets = availableAlphabet;
            this.availableOperations = availableOperations;

            currentAlphabet = availableAlphabet[0];
            currentOperation = availableOperations[0];
            currentShift = 0;

            SetupComboBoxes();
            SetupButtons();
        }

        private void SetupComboBoxes()
        {
            SetupAlphabetComboBox();
            mainWindow.OnAlphabetComboBoxValueChanged += MainWindow_OnAlphabetComboBoxValueChanged;
            SetupOperationComboBox();
            mainWindow.OnOperationComboBoxValueChanged += MainWindow_OnOperationComboBoxValueChanged;
            SetupShiftComboBox();
            mainWindow.OnShiftComboBoxValueChanged += MainWindow_OnShiftComboBoxValueChanged;
        }

        private void SetupAlphabetComboBox()
        {
            string[] alphabetComboBoxOptions = GetUiNames(availableAlphabets);
            SetupComboBox(mainWindow.AlphabetComboBox, alphabetComboBoxOptions, currentAlphabet.UiName);
        }

        private void SetupOperationComboBox()
        {
            string[] operationComboBoxOptions = GetUiNames(availableOperations);
            SetupComboBox(mainWindow.OperationComboBox, operationComboBoxOptions, currentOperation.UiName);
        }

        private void SetupShiftComboBox()
        {
            string[] shiftComboBoxOptions = new string[currentAlphabet.MaxShift + 1];
            for (int i = 0; i < shiftComboBoxOptions.Length; i++)
            {
                shiftComboBoxOptions[i] = i.ToString();
            }
            SetupComboBox(mainWindow.ShiftComboBox, shiftComboBoxOptions, currentShift.ToString());
        }

        private void MainWindow_OnAlphabetComboBoxValueChanged(string? newValue)
        {
            foreach (var alphabet in availableAlphabets)
            {
                if (alphabet.UiName == newValue)
                {
                    currentAlphabet = alphabet;
                    SetupShiftComboBox();
                    System.Diagnostics.Debug.WriteLine($"Выбран алфавит: {newValue}");
                    return;
                }
            }
        }

        private void MainWindow_OnOperationComboBoxValueChanged(string? newValue)
        {
            foreach (var operation in availableOperations)
            {
                if (operation.UiName == newValue)
                {
                    currentOperation = operation;
                    mainWindow.ShiftComboBox.Visibility =
                        currentOperation.Type != OperationType.Hack ? Microsoft.UI.Xaml.Visibility.Visible :
                                                                      Microsoft.UI.Xaml.Visibility.Collapsed;
                    System.Diagnostics.Debug.WriteLine($"Выбрана операция: {newValue}");
                    return;
                }
            }
        }

        private void MainWindow_OnShiftComboBoxValueChanged(string? newValue)
        {
            currentShift = Convert.ToInt32(newValue);
            System.Diagnostics.Debug.WriteLine($"Выбран сдвиг: {newValue}");
        }

        private void SetupButtons()
        {
            mainWindow.OnCalculateButtonPressed += MainWindow_OnCalculateButtonPressed;
        }

        private void MainWindow_OnCalculateButtonPressed()
        {
            string inputText = mainWindow.InputTextBox.Text;
            mainWindow.ResultTextBox.Text = currentOperation.Type switch
            {
                OperationType.Decode => CaesarCipher.Decode(inputText, currentAlphabet, currentShift),
                OperationType.Encode => CaesarCipher.Encode(inputText, currentAlphabet, currentShift),
                OperationType.Hack => CaesarCipher.Hack(inputText, currentAlphabet),
                _ => throw new NotImplementedException()
            };
        }

        private string[] GetUiNames(IUiElement[] uiElements)
        {
            string[] uiNames = new string[uiElements.Length];
            for (int i = 0; i < uiElements.Length; i++)
            {
                uiNames[i] = uiElements[i].UiName;
            }
            return uiNames;
        }

        private void SetupComboBox(ComboBox comboBox, string[] availableItems, string selectedItem)
        {
            comboBox.ItemsSource = availableItems;
            comboBox.SelectedItem = selectedItem;
        }
    }
}
