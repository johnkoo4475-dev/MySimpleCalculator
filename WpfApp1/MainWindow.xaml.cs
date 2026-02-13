using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double? _accumulator = null;
        private string _operator = null;
        private bool _isNewEntry = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var digit = btn.Content.ToString();

            if (_isNewEntry || Display.Text == "0")
            {
                Display.Text = digit;
                _isNewEntry = false;
            }
            else
            {
                Display.Text += digit;
            }
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry)
            {
                Display.Text = "0.";
                _isNewEntry = false;
                return;
            }

            if (!Display.Text.Contains("."))
                Display.Text += ".";
        }

        private void Op_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var op = btn.Content.ToString();

            double current;
            if (!double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out current))
            {
                ClearAll();
                Display.Text = "Error";
                return;
            }

            if (_accumulator.HasValue && !_isNewEntry)
            {
                var result = Calculate(_accumulator.Value, current, _operator);
                Display.Text = FormatResult(result);
                _accumulator = result;
            }
            else
            {
                _accumulator = current;
            }

            _operator = op;
            _isNewEntry = true;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (_operator == null || !_accumulator.HasValue) return;

            double current;
            if (!double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out current))
            {
                ClearAll();
                Display.Text = "Error";
                return;
            }

            var result = Calculate(_accumulator.Value, current, _operator);
            Display.Text = FormatResult(result);
            _accumulator = null;
            _operator = null;
            _isNewEntry = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            Display.Text = "0";
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry) return;

            if (Display.Text.Length > 1)
            {
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
                if (Display.Text == "-")
                {
                    Display.Text = "0";
                    _isNewEntry = true;
                }
            }
            else
            {
                Display.Text = "0";
                _isNewEntry = true;
            }
        }

        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            if (Display.Text == "0") return;
            if (Display.Text.StartsWith("-"))
                Display.Text = Display.Text.Substring(1);
            else
                Display.Text = "-" + Display.Text;
        }

        private double Calculate(double left, double right, string op)
        {
            switch (op)
            {
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "*":
                    return left * right;
                case "/":
                    if (Math.Abs(right) < 1e-15)
                    {
                        ClearAll();
                        Display.Text = "Cannot divide by zero";
                        _isNewEntry = true;
                        return 0;
                    }
                    return left / right;
                default:
                    return right;
            }
        }

        private string FormatResult(double value)
        {
            if (Math.Abs(value % 1) < 1e-12)
                return ((long)value).ToString(CultureInfo.InvariantCulture);
            return value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private void ClearAll()
        {
            _accumulator = null;
            _operator = null;
            _isNewEntry = true;
        }
    }
}
