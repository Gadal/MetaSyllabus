using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// A textblock that supports rich text binding using its BindableInlines dependency property
    /// </summary>
    public class BindableTextBlock : TextBlock
    {
        public static readonly DependencyProperty BindableInlinesProperty = DependencyProperty.Register(
            "BindableInlines",
            typeof(ObservableCollection<Inline>),
            typeof(BindableTextBlock),
            new UIPropertyMetadata(null, OnPropertyChanged));

        public ObservableCollection<Inline> BindableInlines
        {
            get { return (ObservableCollection<Inline>)GetValue(BindableInlinesProperty); }
            set { SetValue(BindableInlinesProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindableTextBlock bindableTextBlock = sender as BindableTextBlock;
            ObservableCollection<Inline> bindableInlines = e.NewValue as ObservableCollection<Inline>;

            if (bindableTextBlock == null || bindableInlines == null) { return; }

            bindableTextBlock.Inlines.Clear();
            foreach (Inline inline in bindableInlines)
            {
                bindableTextBlock.Inlines.Add(inline);
            }
        }
    }
}
