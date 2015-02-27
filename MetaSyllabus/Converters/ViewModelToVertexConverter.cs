using MetaSyllabus.Graphing;
using MetaSyllabus.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MetaSyllabus.Converters
{
    public class ViewModelToVertexConverter : IValueConverter
    {
        /// <summary>
        /// Fetches the VertexWrapper property of a view model for graphing purposes.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as CourseViewModel != null)
                return ((CourseViewModel)value).VertexWrapper;

            if (value as ConceptViewModel != null)
                return ((ConceptViewModel)value).VertexWrapper;

            return null;
        }

        /// <summary>
        /// Fetches the Data property of a Vertex for updating the view model based on interactions
        /// with a graph-based UI element.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as Vertex<CourseViewModel> != null)
                return ((Vertex<CourseViewModel>)value).Data;

            if (value as Vertex<ConceptViewModel> != null)
                return ((Vertex<ConceptViewModel>)value).Data;

            return null;
        }
    }
}
