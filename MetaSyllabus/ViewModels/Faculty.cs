using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// A view model wrapper for displaying CourseViewModels.  Faculties contain Departments.
    /// </summary>
    public class Faculty : INotifyPropertyChanged
    {
        #region Constructors
        public Faculty(string name)
        {
            Name = name;
        }
        #endregion // Constructors

        #region Public Methods
        public void AddCourse(CourseViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.CourseModel.DepartmentName)) { return; }

            // Create a new department if the current course's department hasn't been seen before.
            if (!Departments.Any(x => String.Equals(x.Name, viewModel.CourseModel.DepartmentName)))
            {
                Departments.Add(new Department(viewModel.CourseModel.DepartmentName));
            }

            Departments.First(x => String.Equals(x.Name, viewModel.CourseModel.DepartmentName))
                       .AddCourse(viewModel);
        }

        public void Sort()
        {
            Departments = new ObservableCollection<Department>(Departments.OrderBy(x => x.Name));

            foreach (Department department in Departments)
            {
                department.Sort();
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

        private ObservableCollection<Department> _departments
            = new ObservableCollection<Department>();
        public ObservableCollection<Department> Departments
        {
            get { return _departments; }
            set { SetField(ref _departments, value, "Departments"); }
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
