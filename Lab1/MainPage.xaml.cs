using Lab1.Data.Alphabets;
using Lab1.Data.Operations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace Lab1
{
    public sealed partial class MainPage : Page
    {
        private ViewModel viewModel;

        public TextBox InputTextBox => inputTextBox;
        public TextBox ResultTextBox => resultTextBox;

        public ComboBox AlphabetComboBox => alphabetComboBox;
        public ComboBox OperationComboBox => operationComboBox;
        public ComboBox ShiftComboBox => shiftComboBox;

        public event Action<string?> OnAlphabetComboBoxValueChanged;

        public event Action<string?> OnOperationComboBoxValueChanged;

        public event Action<string?> OnShiftComboBoxValueChanged;

        public event Action OnCalculateButtonPressed;

        public MainPage()
        {
            this.InitializeComponent();

            Alphabet[] availableAplhabets = CreateAvailableAlpabets();
            Operation[] availableOperations = CreateAvailableOperations();
            viewModel = new ViewModel(this, availableAplhabets, availableOperations);
        }

        private void AlphabetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (alphabetComboBox.SelectedItem == null)
            {
                return;
            }
            OnAlphabetComboBoxValueChanged?.Invoke(alphabetComboBox.SelectedItem.ToString());
        }

        private void OperationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (operationComboBox.SelectedItem == null)
            {
                return;
            }
            OnOperationComboBoxValueChanged?.Invoke(operationComboBox.SelectedItem.ToString());
        }

        private void ShiftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (shiftComboBox.SelectedItem == null)
            {
                return;
            }
            OnShiftComboBoxValueChanged?.Invoke(shiftComboBox.SelectedItem.ToString());
        }

        private async void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = Clipboard.GetContent();
            if (dataPackage.Contains(StandardDataFormats.Text))
            {
                inputTextBox.Text = await dataPackage.GetTextAsync(StandardDataFormats.Text);
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            OnCalculateButtonPressed?.Invoke();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            string text = resultTextBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
            }
        }

        private Alphabet[] CreateAvailableAlpabets()
        {
            double[] russianFrequencies = [0.0815, 0.0185, 0.0454, 0.0170, 0.0298, 0.0088, 0.0094, 0.0165, 0.0735,
                0.0121, 0.0349, 0.0440, 0.0321, 0.0670, 0.1097, 0.0281, 0.0473, 0.0547, 0.0262, 0.0262, 0.0026,
                0.0097, 0.0048, 0.0144, 0.0072, 0.0036, 0.0190, 0.0043, 0.0062, 0.0185, 0.0210, 0.0170];
            Alphabet russianAlphabet = new(
                AlphabetType.Russian,
                "Русский",
                'а',
                'я',
                new() { { 'ё', 'е' } },
                russianFrequencies);

            double[] englishFrequencies = [0.08167, 0.01492, 0.02782, 0.04253, 0.12702, 0.02228, 0.02015, 0.06094,
                0.06966, 0.00153, 0.00772, 0.04025, 0.02406, 0.06749, 0.07507, 0.01929, 0.00095, 0.05987, 0.06327,
                0.09056, 0.02758, 0.00978, 0.02360, 0.00150, 0.01974, 0.00074];
            Alphabet englishAlphabet = new(
                AlphabetType.English,
                "Английский",
                'a',
                'z',
                [],
                englishFrequencies);

            return [russianAlphabet, englishAlphabet];
        }

        private Operation[] CreateAvailableOperations()
        {
            Operation decodeOperation = new Operation(OperationType.Decode, "Расшифровать");
            Operation encodeOperation = new Operation(OperationType.Encode, "Зашифровать");
            Operation hackOperation = new Operation(OperationType.Hack, "Взломать");

            return [decodeOperation, encodeOperation, hackOperation];
        }
    }
}