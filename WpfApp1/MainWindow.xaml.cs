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
            try
            {
                var btn = sender as Button;
                if (btn == null) return;
                var digit = btn.Content.ToString();

                if (_isNewEntry || Display.Text == "0")
                {
                    SetDisplayText(digit);
                    _isNewEntry = false;
                }
                else
                {
                    SetDisplayText(Display.Text + digit);
                }
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isNewEntry)
                {
                    SetDisplayText("0.");
                    _isNewEntry = false;
                    return;
                }

                if (!Display.Text.Contains("."))
                    SetDisplayText(Display.Text + ".");
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Op_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                if (btn == null) return;
                var op = btn.Content.ToString();

                double current;
                if (!double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out current))
                {
                    ClearAll();
                    SetDisplayText("Error");
                    return;
                }

                if (_accumulator.HasValue && !_isNewEntry)
                {
                    var result = Calculate(_accumulator.Value, current, _operator);
                    SetDisplayText(FormatResult(result));
                    _accumulator = result;
                }
                else
                {
                    _accumulator = current;
                }

                _operator = op;
                _isNewEntry = true;
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_operator == null || !_accumulator.HasValue) return;

                double current;
                if (!double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out current))
                {
                    ClearAll();
                    SetDisplayText("Error");
                    return;
                }

                var result = Calculate(_accumulator.Value, current, _operator);
                SetDisplayText(FormatResult(result));
                _accumulator = null;
                _operator = null;
                _isNewEntry = true;
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearAll();
                SetDisplayText("0");
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isNewEntry) return;

                if (Display.Text.Length > 1)
                {
                    SetDisplayText(Display.Text.Substring(0, Display.Text.Length - 1));
                    if (Display.Text == "-")
                    {
                        SetDisplayText("0");
                        _isNewEntry = true;
                    }
                }
                else
                {
                    SetDisplayText("0");
                    _isNewEntry = true;
                }
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
        }

        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Display.Text == "0") return;
                if (Display.Text.StartsWith("-"))
                    SetDisplayText(Display.Text.Substring(1));
                else
                    SetDisplayText("-" + Display.Text);
            }
            catch (Exception)
            {
                ClearAll();
                SetDisplayText("Error");
            }
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
                        SetDisplayText("Cannot divide by zero");
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
            // Handle special floating-point values explicitly
            if (double.IsNaN(value))
                return "NaN";
            if (double.IsInfinity(value))
                return value > 0 ? "Infinity" : "-Infinity";

            // If value is effectively an integer, and within long range, format without decimal point.
            const double epsilon = 1e-12;
            if (Math.Abs(value % 1) < epsilon)
            {
                double abs = Math.Abs(value);
                if (abs <= (double)long.MaxValue)
                {
                    return ((long)value).ToString(CultureInfo.InvariantCulture);
                }
                // For integer-valued doubles outside long range, fall through to general formatting.
            }

            // General formatting for floating values (use G15 for precision)
            return value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private void SetDisplayText(string text)
        {
            Display.Text = text;
            AdjustDisplayFont();
        }
        private void AdjustDisplayFont()
        {
            const double defaultSize = 32.0;
            const double minSize = 16.0;
            int len = string.IsNullOrEmpty(Display.Text) ? 0 : Display.Text.Length;
            double newSize = defaultSize;

            // Start shrinking when text length exceeds 12 characters.
            if (len > 12)
            {
                // reduce 1.2 points per extra character as a simple heuristic
                newSize = defaultSize - (len - 12) * 1.2;
                if (newSize < minSize) newSize = minSize;
            }

            Display.FontSize = newSize;
        }

        private void ClearAll()
        {
            _accumulator = null;
            _operator = null;
            _isNewEntry = true;
        }
    }
}
