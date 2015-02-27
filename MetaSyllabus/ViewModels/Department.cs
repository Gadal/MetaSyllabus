using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// A view model wrapper for displaying CourseViewModels.
    /// </summary>
    public class Department : INotifyPropertyChanged
    {
        #region Constructors
        public Department(string name)
        {
            Name = name;
        }
        #endregion // Constructors

        #region Public Methods
        public void AddCourse(CourseViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.CourseModel.Title))
                return;
            else
                CourseViewModels.Add(viewModel);
        }

        public void Sort()
        {
            CourseViewModels = new ObservableCollection<CourseViewModel>(CourseViewModels.OrderBy(x => x.CourseModel.Title));
        }
        #endregion // Public Methods

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value, "Name"); }
        }

        private ObservableCollection<CourseViewModel> _courseViewModels
            = new ObservableCollection<CourseViewModel>();
        public ObservableCollection<CourseViewModel> CourseViewModels
        {
            get { return _courseViewModels; }
            set { SetField(ref _courseViewModels, value, "CourseViewModels"); }
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
