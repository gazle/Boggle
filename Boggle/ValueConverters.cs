using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Boggle
{
    class SampleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class KeyDownEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = (KeyEventArgs)value;
            var border = (Border)args.OriginalSource;
            var container = (UIElement)FindAncestorChildOf<Panel>(border);
            var parent = (Panel)VisualTreeHelper.GetParent(container);
            return (parent.Children.IndexOf(container), args.Key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static DependencyObject FindAncestorChildOf<T>(DependencyObject element) where T : DependencyObject
        {
            DependencyObject child;
            do
            {
                child = element;
                element = VisualTreeHelper.GetParent(element);
            } while (element != null && !(element is T));
            return element is null ? null : child as DependencyObject;
        }
    }
}