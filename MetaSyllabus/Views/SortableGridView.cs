using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaSyllabus.Views
{
    /// <summary>
    /// Presents a list of objects.  Object properties bind to column headers.
    /// Clicking the header sorts the list lexicographically.
    /// </summary>
    public class SortableGridView
    {
        #region Public Attached Properties
 
        public static ICommand GetCommand(DependencyObject dependencyObject)
        {
            return (ICommand)dependencyObject.GetValue(CommandProperty);
        }
 
        public static void SetCommand(DependencyObject dependencyObject, ICommand value)
        {
            dependencyObject.SetValue(CommandProperty, value);
        }
 
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(SortableGridView),
                new UIPropertyMetadata(
                    null,
                    (o, e) =>
                    {
                        ItemsControl listView = o as ItemsControl;
                        if (listView != null)
                        {
                            if (!GetAutoSort(listView)) // Don't change click handler if AutoSort is enabled
                            {
                                if (e.OldValue != null && e.NewValue == null)
                                {
                                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                                if (e.OldValue == null && e.NewValue != null)
                                {
                                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }
                )
            );
 
        public static bool GetAutoSort(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(AutoSortProperty);
        }
 
        public static void SetAutoSort(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AutoSortProperty, value);
        }
 
        public static readonly DependencyProperty AutoSortProperty = DependencyProperty.RegisterAttached(
                "AutoSort",
                typeof(bool),
                typeof(SortableGridView),
                new UIPropertyMetadata(
                    false,
                    (o, e) =>
                    {
                        ListView listView = o as ListView;
                        if (listView != null)
                        {
                            if (GetCommand(listView) == null) // Don't change click handler if a command is set
                            {
                                bool oldValue = (bool)e.OldValue;
                                bool newValue = (bool)e.NewValue;
                                if (oldValue && !newValue)
                                {
                                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                                if (!oldValue && newValue)
                                {
                                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }
                )
            );
 
        public static string GetPropertyName(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PropertyNameProperty);
        }
 
        public static void SetPropertyName(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PropertyNameProperty, value);
        }
 
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
                "PropertyName",
                typeof(string),
                typeof(SortableGridView),
                new UIPropertyMetadata(null)
            );
 
        public static bool GetShowSortGlyph(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ShowSortGlyphProperty);
        }
 
        public static void SetShowSortGlyph(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ShowSortGlyphProperty, value);
        }
 
        public static readonly DependencyProperty ShowSortGlyphProperty = DependencyProperty.RegisterAttached(
            "ShowSortGlyph", 
            typeof(bool), 
            typeof(SortableGridView), 
            new UIPropertyMetadata(true));
 
        public static ImageSource GetSortGlyphAscending(DependencyObject dependencyObject)
        {
            return (ImageSource)dependencyObject.GetValue(SortGlyphAscendingProperty);
        }
 
        public static void SetSortGlyphAscending(DependencyObject dependencyObject, ImageSource value)
        {
            dependencyObject.SetValue(SortGlyphAscendingProperty, value);
        }

        public static readonly DependencyProperty SortGlyphAscendingProperty = DependencyProperty.RegisterAttached(
            "SortGlyphAscending", 
            typeof(ImageSource), 
            typeof(SortableGridView),
            new UIPropertyMetadata(null));
 
        public static ImageSource GetSortGlyphDescending(DependencyObject dependencyObject)
        {
            return (ImageSource)dependencyObject.GetValue(SortGlyphDescendingProperty);
        }
 
        public static void SetSortGlyphDescending(DependencyObject dependencyObject, ImageSource value)
        {
            dependencyObject.SetValue(SortGlyphDescendingProperty, value);
        }
 
        public static readonly DependencyProperty SortGlyphDescendingProperty = DependencyProperty.RegisterAttached(
            "SortGlyphDescending", 
            typeof(ImageSource), 
            typeof(SortableGridView), 
            new UIPropertyMetadata(null));
 
        #endregion // Public Attached Properties
 
        #region Private Attached Properties
 
        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject dependencyObject)
        {
            return (GridViewColumnHeader)dependencyObject.GetValue(SortedColumnHeaderProperty);
        }
 
        private static void SetSortedColumnHeader(DependencyObject dependencyObject, GridViewColumnHeader value)
        {
            dependencyObject.SetValue(SortedColumnHeaderProperty, value);
        }
 
        private static readonly DependencyProperty SortedColumnHeaderProperty = DependencyProperty.RegisterAttached(
            "SortedColumnHeader", 
            typeof(GridViewColumnHeader), 
            typeof(SortableGridView), 
            new UIPropertyMetadata(null));
 
        #endregion // Private Attached Properties
 
        #region Column Header Click Event Handler
 
        /// <summary>
        /// Creates an alphabetical order based on the property represented by
        /// whichever header was clicked.  First click sorts descending, second click sorts ascending.
        /// </summary>
        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null && headerClicked.Column != null)
            {
                string propertyName = GetPropertyName(headerClicked.Column);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView listView = GetAncestor<ListView>(headerClicked);
                    if (listView != null)
                    {
                        ICommand command = GetCommand(listView);
                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                            {
                                command.Execute(propertyName);
                            }
                        }
                        else if (GetAutoSort(listView))
                        {
                            ApplySort(listView.Items, propertyName, listView, headerClicked);
                        }
                    }
                }
            }
        }
 
        #endregion // Column Header Click Event Handler
 
        #region Helper methods
 
        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent == null ? null : (T)parent;
        }
 
        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();
 
                GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
                if (currentSortedColumnHeader != null)
                {
                    RemoveSortGlyph(currentSortedColumnHeader);
                }
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
                if (GetShowSortGlyph(listView))
                    AddSortGlyph(
                        sortedColumnHeader,
                        direction,
                        direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
                SetSortedColumnHeader(listView, sortedColumnHeader);
            }
        }
 
        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }
 
        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
            if (adorners != null)
            {
                foreach (Adorner adorner in adorners)
                {
                    if (adorner is SortGlyphAdorner)
                        adornerLayer.Remove(adorner);
                }
            }
        }
 
        #endregion // Helper Methods
 
        #region SortGlyphAdorner Nested Class
 
        private class SortGlyphAdorner : Adorner
        {
            private GridViewColumnHeader ColumnHeader;
            private ListSortDirection Direction;
            private ImageSource SortGlyph;
 
            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
                : base(columnHeader)
            {
                ColumnHeader = columnHeader;
                Direction = direction;
                SortGlyph = sortGlyph;
            }
 
            private Geometry GetDefaultGlyph()
            {
                //                            __
                // The glyph looks like this: \/

                double x1 = ColumnHeader.ActualWidth - 17;
                double x2 = x1 + 10;
                double x3 = x1 + 5;
                double y1 = ColumnHeader.ActualHeight / 2 - 3;
                double y2 = y1 + 5;
 
                if (Direction == ListSortDirection.Ascending)
                {
                    double temp = y1;
                    y1 = y2;
                    y2 = temp;
                }
 
                PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
                pathSegmentCollection.Add(new LineSegment(new Point(x2, y1), true));
                pathSegmentCollection.Add(new LineSegment(new Point(x3, y2), true));
 
                PathFigure pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);
 
                PathFigureCollection pathFigureCollection = new PathFigureCollection();
                pathFigureCollection.Add(pathFigure);
 
                return new PathGeometry(pathFigureCollection);
            }
 
            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                drawingContext.DrawGeometry
                (
                    new SolidColorBrush(Colors.LightBlue) { Opacity = 0.5 },
                    new Pen(Brushes.DodgerBlue, 0.5),
                    GetDefaultGlyph()
                );
            }
        }
 
        #endregion // SortGlyphAdorner Nested Class
    }
}
