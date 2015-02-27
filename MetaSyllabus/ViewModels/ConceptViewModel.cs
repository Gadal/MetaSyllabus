using MetaSyllabus.Graphing;
using MetaSyllabus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// A view model wrapper for concept objects.  Supports binding, listviews/treeviews, graphs
    /// </summary>
    public class ConceptViewModel : INotifyPropertyChanged
    {
        #region Public Members

        public Concept ConceptModel;
        public Vertex<ConceptViewModel> VertexWrapper;
        public List<CourseViewModel> AssociatedCourseViewModels = new List<CourseViewModel>();
        public event RequestNavigateToCourseHandler RequestNavigateToCourse;
        public delegate void RequestNavigateToCourseHandler(CourseViewModel c, EventArgs e);

        #endregion // Public Members

        #region Public Methods

        public override string ToString()
        {
            return ConceptModel.Name;
        }

        public void UpdateRichText()
        {
            // We have to create the collection of Inlines on the application's UI thread, since we're
            // breaking MVVM here and working directly with the UI's ContentElements.
            // Hopefully someday RichTextBox's Inlines property will be a DependencyProperty and support binding natively.
            Application.Current.Dispatcher.Invoke(new Action(delegate()
            {
                ObservableCollection<Inline> inlines = new ObservableCollection<Inline>(); ;

                inlines.Add(new Run(ConceptModel.Name)
                {
                    FontWeight = FontWeights.ExtraBlack,
                    FontSize = 14,
                });

                inlines.Add(new Run(Environment.NewLine));

                inlines.Add(new Run(ConceptModel.Summary));

                inlines.Add(new Run(Environment.NewLine));
                inlines.Add(new Run(Environment.NewLine));

                if (ConceptModel.AssociatedCourses.Count > 0)
                {
                    string s = ConceptModel.AssociatedCourses.Count == 1 ?
                                    "Involved in the following course:" :
                                    "Involved in the following courses:";

                    inlines.Add(new Run(s)
                    {
                        FontWeight = FontWeights.Black
                    });
                    inlines.Add(new Run(Environment.NewLine));
                }

                foreach (CourseViewModel viewModel in AssociatedCourseViewModels.OrderBy(x => x.Title))
                {
                    string s = String.Format("{0} — {1}", viewModel.Title, viewModel.Code);
                    CourseHyperlink courseHyperlink = new CourseHyperlink(new Run(s), viewModel);
                    courseHyperlink.Click += Hyperlink_RequestNavigate;

                    inlines.Add(courseHyperlink);
                    inlines.Add(new Run(Environment.NewLine));
                }

                RichText = inlines;
            }));
        }

        #endregion // Public Methods

        #region Private Methods

        private void Hyperlink_RequestNavigate(object sender, RoutedEventArgs e)
        {
            CourseHyperlink link = sender as CourseHyperlink;
            if (link == null) { return; }

            CourseViewModel course = link.LinkedCourse;
            if (course == null) { return; }

            if (RequestNavigateToCourse == null) { return; }
            RequestNavigateToCourse(course, null);
        }

        #endregion // Private Methods

        #region Properties

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetField(ref _isSelected, value, "IsSelected"); }
        }

        private ObservableCollection<Inline> _richText;
        public ObservableCollection<Inline> RichText
        {
            get { return _richText; }
            set { SetField(ref _richText, value, "RichText"); }
        }

        public string Name { get { return ConceptModel.Name; } }

        #endregion // Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion // INotifyPropertyChanged

        #region CourseHyperlink Nested Class
        private class CourseHyperlink : Hyperlink
        {
            public CourseHyperlink(Inline childInline, CourseViewModel linkedCourse) 
                : base(childInline)
            {
                LinkedCourse = linkedCourse;
            }

            public CourseViewModel LinkedCourse;
        }
        #endregion // CourseHyperlink Nested Class
    }
}
