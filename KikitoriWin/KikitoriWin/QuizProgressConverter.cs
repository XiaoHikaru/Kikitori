using System;
using System.Windows.Data;
using Kikitori.ViewModel;

namespace Kikitori
{
    public class QuizProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MainWindowVM mainVM = values[0] as MainWindowVM;
            int mediumIndex = (int)values[1];
            if (mainVM.DBIsLoaded())
            {
                GapQuizVM gapQuizVM = new GapQuizVM(mainVM, mediumIndex); // TODO: create entire VM for that?
                return $"{gapQuizVM.NumberOfCorrectTokens} / {gapQuizVM.NumberOfTokens}";
            }
            else
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Bogus.");
        }
    }
}
