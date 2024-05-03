using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace Laboratorium2.Zadanie1.NET;

public partial class MainWindow
{
    private const string EventLogSource = "Application";
    private const string DivisionErrorMessage = "Błąd podczas operacji dzielenia: ";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void divideButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ValidateInput(DividendTextBox.Text, DivisorTextBox.Text);

            var culture = new CultureInfo("pl-PL");
            var dividend = double.Parse(DividendTextBox.Text, culture);
            var divisor = double.Parse(DivisorTextBox.Text, culture);

            ValidateDivisor(divisor);

            var result = PerformDivision(dividend, divisor);

            ResultTextBox.Text = result.ToString(culture);
            LogEvent("Operacja dzielenia zakończona sukcesem.", EventLogEntryType.Information);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void ValidateInput(string dividend, string divisor)
    {
        if (string.IsNullOrEmpty(dividend) || string.IsNullOrEmpty(divisor))
            throw new ArgumentException("Proszę wprowadzić obie liczby.");
    }

    private void ValidateDivisor(double divisor)
    {
        if (divisor == 0) throw new DivideByZeroException();
    }

    private double PerformDivision(double dividend, double divisor)
    {
        var result = dividend / divisor;

        if (double.IsInfinity(result) || double.IsNaN(result)) throw new OverflowException();

        return result;
    }

    private void LogEvent(string message, EventLogEntryType type)
    {
        EventLog.WriteEntry(EventLogSource, message, type);
    }

    private void HandleException(Exception ex)
    {
        var message = ex switch
        {
            FormatException => "Proszę wprowadzić prawidłowe liczby.",
            DivideByZeroException => "Nie można dzielić przez zero.",
            OverflowException => "Wynik operacji jest poza zakresem typu liczbowego.",
            ArgumentException => "Proszę wprowadzić obie liczby.",
            _ => "Wystąpił nieoczekiwany błąd podczas operacji dzielenia."
        };

        LogEvent(DivisionErrorMessage + ex.Message, EventLogEntryType.Error);
        MessageBox.Show(message);
    }
}