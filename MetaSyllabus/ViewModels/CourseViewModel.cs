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
    /// A view model wrapper for Course objects.  Supports binding, listviews/treeviews, graphs
    /// </summary>
    public class CourseViewModel : INotifyPropertyChanged
    {
        #region Public Members

        public Course CourseModel;
        public Vertex<CourseViewModel> VertexWrapper;
        public List<ConceptViewModel> AssociatedConceptViewModels = new List<ConceptViewModel>();
        public event RequestNavigateToConceptHandler RequestNavigateToConcept;
        public delegate void RequestNavigateToConceptHandler(ConceptViewModel c, EventArgs e);

        #endregion // Public Members

        #region Public Methods

        public override string ToString()
        {
            return Title;
        }

        public void UpdateRichText()
        {
            // We have to create the collection of Inlines on the application's UI thread, since we're
            // breaking MVVM here and working directly with the UI's ContentElements.
            // Hopefully someday RichTextBox's Inlines property will be a DependencyProperty and support binding natively.
            Application.Current.Dispatcher.Invoke(new Action(delegate()
            {
                ObservableCollection<Inline> inlines = new ObservableCollection<Inline>(); ;

                inlines.Add(new Run(CourseModel.Title)
                {
                    FontWeight = FontWeights.ExtraBlack,
                    FontSize = 14,
                });

                inlines.Add(new Run(Environment.NewLine));

                inlines.Add(new Run(CourseModel.Description));

                inlines.Add(new Run(Environment.NewLine));
                inlines.Add(new Run(Environment.NewLine));

                if (CourseModel.AssociatedConcepts.Count > 0)
                {
                    string s = CourseModel.AssociatedConcepts.Count == 1 ?
                                    "Involves the following concept:" :
                                    "Involves the following concepts:";

                    inlines.Add(new Run(s)
                    {
                        FontWeight = FontWeights.Black
                    });
                    inlines.Add(new Run(Environment.NewLine));
                }

                foreach (ConceptViewModel viewModel in AssociatedConceptViewModels.OrderBy(x => x.Name))
                {
                    ConceptHyperlink courseHyperlink = new ConceptHyperlink(new Run(viewModel.Name), viewModel);
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
            ConceptHyperlink link = sender as ConceptHyperlink;
            if (link == null) { return; }

            ConceptViewModel concept = link.LinkedConcept;
            if (concept == null) { return; }

            if (RequestNavigateToConcept == null) { return; }
            RequestNavigateToConcept(concept, null);
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

        public string Title           { get { return CourseModel.Title;           } }
        public string InstitutionName { get { return CourseModel.InstitutionName; } }
        public string FacultyName     { get { return CourseModel.FacultyName;     } }
        public string DepartmentName  { get { return CourseModel.DepartmentName;  } }
        public string Code            { get { return CourseModel.Code;            } }

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

        #region ConceptHyperlink Nested Class
        private class ConceptHyperlink : Hyperlink
        {
            public ConceptHyperlink(Inline childInline, ConceptViewModel linkedConcept)
                : base(childInline)
            {
                LinkedConcept = linkedConcept;
            }

            public ConceptViewModel LinkedConcept;
        }
        #endregion // ConceptHyperlink Nested Class
    }
}
