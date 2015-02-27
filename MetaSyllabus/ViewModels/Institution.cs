using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// A view model wrapper for displaying CourseViewModels.  Institutions contain Faculties.
    /// </summary>
    public class Institution : INotifyPropertyChanged
    {
        #region Constructors
        public Institution(string name)
        {
            Faculties = new ObservableCollection<Faculty>();
            Name = name;
        }
        #endregion // Constructors

        #region Public Methods
        public void AddCourse(CourseViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.CourseModel.FacultyName)) { return; }

            // Create a new faculty if the current course's faculty hasn't been seen before.
            if (!Faculties.Any(x => String.Equals(x.Name, viewModel.CourseModel.FacultyName)))
            {
                Faculties.Add(new Faculty(viewModel.CourseModel.FacultyName));
            }

            Faculties.First(x => String.Equals(x.Name, viewModel.CourseModel.FacultyName))
                     .AddCourse(viewModel);
        }

        public void Sort()
        {
            Faculties = new ObservableCollection<Faculty>(Faculties.OrderBy(x => x.Name));

            foreach (Faculty faculty in Faculties)
            {
                faculty.Sort();
            }
        }

        #endregion // Public Methods

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value, "Name"); }
        }

        private ObservableCollection<Faculty> _faculties
            = new ObservableCollection<Faculty>();
        public ObservableCollection<Faculty> Faculties
        {
            get { return _faculties; }
            set { SetField(ref _faculties, value, "Faculties"); }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetField(ref _isExpanded, value, "IsExpanded"); }
        }

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
    }
}
