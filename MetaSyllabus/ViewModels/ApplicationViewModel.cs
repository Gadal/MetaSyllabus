using MetaSyllabus.Commands;
using MetaSyllabus.Graphing;
using MetaSyllabus.Models;
using MetaSyllabus.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

// Visual Studio: Ctrl + M, Ctrl + O to collapse all.  Ctrl + M, Ctrl + L to expand all.
namespace MetaSyllabus.ViewModels
{
    /// <summary>
    /// Manages application state across page navigations.
    /// </summary>
    public class ApplicationViewModel : DependencyObject,
                                        INotifyPropertyChanged
    {
        #region Private Members

        private NavigationService NavigationService;

        private PatriciaTrie<CourseViewModel> CourseTrie = new PatriciaTrie<CourseViewModel>();
        private PatriciaTrie<ConceptViewModel> ConceptTrie = new PatriciaTrie<ConceptViewModel>();

        private string MostRecentSearch;

        #endregion // Private Members

        #region Constructors
        public ApplicationViewModel(NavigationService navigationService)
        {
            // Construct the view model once the data model is finished loading
            BackgroundWorker viewModelBuilder = new BackgroundWorker();
            viewModelBuilder.DoWork += ViewModelBuilder_DoWork;
            _dataModel.DataModelLoaded += (sender, args) => { viewModelBuilder.RunWorkerAsync(); };

            // Link commands
            SearchAndNavCommand = new SimpleCommand 
                (
                    (param) => { Search(); NavigateToPage(param); },
                    (param) => { return CanSearch(param) && CanNavigateToPage(param); }
                );
            SearchCommand = new SimpleCommand(Search, CanSearch);
            NavToPageCommand = new SimpleCommand(NavigateToPage, CanNavigateToPage);

            // Load the view
            NavigationService = navigationService;
            NavigateToPage("Views/Pages/BeginPage.xaml");
        }

        #endregion // Constructors

        #region Commands

        public ICommand SearchAndNavCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand NavToPageCommand { get; set; }

        private void Search(object parameter)
        {
            Search(SearchString);
        }

        private bool CanSearch(object parameter)
        {
            return true;
        }

        private void NavigateToPage(object parameter)
        {
            Uri uri = new Uri((string)parameter, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private bool CanNavigateToPage(object parameter)
        {
            return !String.IsNullOrWhiteSpace((string)parameter);
        }

        #endregion // Commands

        #region Private Methods

        private void ConceptViewModel_RequestNavigateToCourse(object sender, EventArgs e)
        {
            if (sender is CourseViewModel)
            {
                SelectedCourse = (CourseViewModel)sender;
                SelectedConcept = null;
                NavigateToPage("Views/Pages/CourseListingsPage.xaml");
            }
        }

        private void CourseViewModel_RequestNavigateToConcept(object sender, EventArgs e)
        {
            if (sender is ConceptViewModel)
            {
                SelectedConcept = (ConceptViewModel)sender;
                SelectedCourse = null;
                NavigateToPage("Views/Pages/ConceptListingsPage.xaml");
            }
        }

        #endregion // Private Methods

        #region Public Methods

        #endregion // Public Methods

        /// <summary>
        /// Asynchronously updates the 'CourseSearchResult' and 'ConceptSearchResult' properties.
        /// </summary>
        private void SearchBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string query = e.Argument as string;
            if (!String.IsNullOrWhiteSpace(query))
            {
                IEnumerable<CourseViewModel>  courseResults  = CourseTrie .Search(query).OrderBy(x => x.CourseModel.Title);
                IEnumerable<ConceptViewModel> conceptResults = ConceptTrie.Search(query).OrderBy(x => x.ConceptModel.Name);

                Dispatcher.Invoke(() =>
                {
                    CourseSearchResults = new ObservableCollection<CourseViewModel>(courseResults);
                    ConceptSearchResults = new ObservableCollection<ConceptViewModel>(conceptResults);
                });
            }
        }

        public void Search()
        {
            Search(SearchString);
        }

        public void Search(string query)
        {
            // Queue up a new search and run it.
            BackgroundWorker SearchAsync = new BackgroundWorker();
            SearchAsync.DoWork += SearchBackgroundWorker_DoWork;
            SearchAsync.RunWorkerAsync(query);

            // Sometimes we want to rerun the previous search without user intervention, for instance if
            // the model changes, we want to update the search results.  Privately store that search string.
            MostRecentSearch = query;
        }

        private void ViewModelBuilder_DoWork(object sender, DoWorkEventArgs e)
        {
            //
            // Generate View Models
            //

            // Concept's name -> Concept view model
            Dictionary<string, ConceptViewModel> conceptViewModelLookup =
                new Dictionary<string, ConceptViewModel>();

            // Course's institution -> Course's course code -> Course view model
            Dictionary<string, Dictionary<string, CourseViewModel>> courseViewModelLookup =
                new Dictionary<string, Dictionary<string, CourseViewModel>>();

            List<CourseViewModel> courseViewModels = new List<CourseViewModel>();
            List<ConceptViewModel> conceptViewModels = new List<ConceptViewModel>();

            PatriciaTrie<CourseViewModel> courseTrie = new PatriciaTrie<CourseViewModel>();
            PatriciaTrie<ConceptViewModel> conceptTrie = new PatriciaTrie<ConceptViewModel>();

            ObservableCollection<Institution> courseTree = new ObservableCollection<Institution>();

            foreach (Course course in DataModel.Courses)
            {
                CourseViewModel courseViewModel = new CourseViewModel() { CourseModel = course };

                courseViewModel.RequestNavigateToConcept += CourseViewModel_RequestNavigateToConcept;

                if (!courseViewModelLookup.ContainsKey(course.InstitutionName))
                    courseViewModelLookup[course.InstitutionName] = new Dictionary<string, CourseViewModel>();

                courseViewModelLookup[course.InstitutionName][course.Code] = courseViewModel;
                courseViewModels.Add(courseViewModel);
                Index(courseViewModel, courseTrie, courseTree);
            }

            CourseTrie = courseTrie;
            CourseTree = courseTree;
            CourseViewModels = new ObservableCollection<CourseViewModel>(courseViewModels);

            foreach (Concept concept in DataModel.Concepts)
            {
                ConceptViewModel conceptViewModel = new ConceptViewModel() { ConceptModel = concept };
                conceptViewModel.RequestNavigateToCourse += ConceptViewModel_RequestNavigateToCourse;
                conceptViewModelLookup[concept.Name] = conceptViewModel;
                conceptViewModels.Add(conceptViewModel);
                Index(conceptViewModel, conceptTrie);
            }

            ConceptTrie = conceptTrie;
            ConceptViewModels = new ObservableCollection<ConceptViewModel>(conceptViewModels);

            // Sort the course tree then rerun the most recent search
            CourseTree.OrderByDescending(x => x.Name);
            foreach(Institution institution in CourseTree)
            {
                institution.Sort();
            }
            Search(MostRecentSearch);

            foreach (CourseViewModel courseViewModel in courseViewModels)
            {
                foreach (Concept concept in courseViewModel.CourseModel.AssociatedConcepts.Keys)
                {
                    courseViewModel.AssociatedConceptViewModels.Add(conceptViewModelLookup[concept.Name]);
                }

                courseViewModel.UpdateRichText();
            }

            foreach (ConceptViewModel conceptViewModel in conceptViewModels)
            {
                foreach (Course course in conceptViewModel.ConceptModel.AssociatedCourses.Keys)
                {
                    if (courseViewModelLookup.ContainsKey(course.InstitutionName) &&
                        courseViewModelLookup[course.InstitutionName].ContainsKey(course.Code))
                    {
                        conceptViewModel.AssociatedCourseViewModels.Add(courseViewModelLookup[course.InstitutionName][course.Code]);
                    }
                }

                conceptViewModel.UpdateRichText();
            }

            //
            // Generate Graphs
            //

            // GraphX needs a unique ID for every graph component.  It doesn't need to be a hash,
            // so this is sufficient.  This gets incremented every time we make a new graph component.
            int GraphComponentIdGenerator = 0;

            // 1. Create a vertex wrapper for each course view model

            // Associate a vertex with each view model.
            foreach (CourseViewModel viewModel in courseViewModels)
            {
                Vertex<CourseViewModel> vertex = new Vertex<CourseViewModel>()
                {
                    Data = viewModel,
                    ID = GraphComponentIdGenerator++
                };

                viewModel.VertexWrapper = vertex;
            }

            // 2. Add an edge for every prerequisite/corequisite/crosslisting relationship.
            foreach (CourseViewModel viewModel in courseViewModels)
            {
                string institution = viewModel.CourseModel.InstitutionName;

                // Create all graph edges for prerequisites
                foreach (string prerequisite in viewModel.CourseModel.Prerequisites)
                {
                    if (!courseViewModelLookup.ContainsKey(institution) ||
                        !courseViewModelLookup[institution].ContainsKey(prerequisite) ||
                         courseViewModelLookup[institution][prerequisite] == null)
                    {
                        // Sometimes courses list prerequisites that don't even exist.
                        // Nothing to do but ignore the entry.
                        continue;
                    }

                    CourseViewModel prereqViewModel = courseViewModelLookup[institution][prerequisite];

                    // In case a course somehow lists itself as a prerequisite.  Possibly unnecessary.
                    if (viewModel == prereqViewModel) { continue; }

                    Edge<CourseViewModel> edge = new Edge<CourseViewModel>()
                    {
                        Source = prereqViewModel.VertexWrapper,
                        Target = viewModel.VertexWrapper,
                        Variety = Edge<CourseViewModel>.EdgeVariety.SourcePrerequisiteForTarget,
                        ID = GraphComponentIdGenerator++
                    };

                    viewModel.VertexWrapper.Edges.Add(edge);
                    prereqViewModel.VertexWrapper.Edges.Add(edge);
                }

                // Create all graph edges for corequisites and crosslistings
                foreach (string related in viewModel.CourseModel.Corequisites.Union(viewModel.CourseModel.Crosslistings))
                {
                    if (!courseViewModelLookup.ContainsKey(institution) ||
                        !courseViewModelLookup[institution].ContainsKey(related) ||
                         courseViewModelLookup[institution][related] == null)
                    {
                        // Sometimes courses list prerequisites that don't even exist.
                        // Nothing to do but ignore the entry.
                        continue;
                    }

                    CourseViewModel relatedViewModel = courseViewModelLookup[institution][related];

                    if (viewModel == relatedViewModel) { continue; } // Ignore self loops

                    Edge<CourseViewModel> edge = new Edge<CourseViewModel>()
                    {
                        Source = viewModel.VertexWrapper,
                        Target = relatedViewModel.VertexWrapper,
                        Variety = Edge<CourseViewModel>.EdgeVariety.Equivalents,
                        ID = GraphComponentIdGenerator++
                    };

                    viewModel.VertexWrapper.Edges.Add(edge);
                    relatedViewModel.VertexWrapper.Edges.Add(edge);
                }
            }

            // 3. Create a vertex wrapper for each concept view model.

            Dictionary<Concept, ConceptViewModel> conceptLookup = new Dictionary<Concept, ConceptViewModel>();

            foreach (ConceptViewModel viewModel in conceptViewModels)
            {
                Vertex<ConceptViewModel> vertex = new Vertex<ConceptViewModel>()
                {
                    ID = GraphComponentIdGenerator++,
                    Data = viewModel
                };

                viewModel.VertexWrapper = vertex;
                conceptLookup[viewModel.ConceptModel] = viewModel;
            }

            // 4. Create an edge for each concept dependency.  A concept depends on another concept in the following scenarios:
            //
            //    Course C_1 contains a concept c_1.  
            //    Course C_2 has C_1 as a prerequisite.
            //    C_2 contains a concept c_2.
            //    C_2 depends on C_1, therefore 
            //    c_2 depends on c_1.
            //
            //    Also: 
            //    if C_1 is crosslisted/corequisite with C_3, and 
            //    C_3 contains concept c_3, then 
            //    c_2 also depends on c_3.
            //
            // Note:
            // 
            //    We only want to catch dependencies one level deep, so:
            //    if C_2 depends on C_1, and 
            //    C_1 depends on course C_0, then
            //    if C_0 contains a concept c_0, then 
            //    c_2 does not depend on c_0.
            //    (But c_1 does depend on c_0, so there will still be a path from c_0 to c_2.  
            //    Algebraic topology DOES depend on basic arithmetic, but a direct edge between the two isn't useful.)

            foreach (ConceptViewModel conceptViewModel in conceptViewModels)
            {
                // We will aggregate all the concepts that depend on conceptViewModel into here.
                // The float represents how strongly a given concept depends on conceptViewModel.
                Dictionary<Concept, float> conceptEdges = new Dictionary<Concept, float>();

                // Get all courses that contain the conceptViewModel
                foreach (Course course in conceptViewModel.ConceptModel.AssociatedCourses.Keys)
                {
                    if (courseViewModelLookup.ContainsKey(course.InstitutionName) &&
                        courseViewModelLookup[course.InstitutionName].ContainsKey(course.Code))
                    {
                        // Get all the courses that depend on any of those courses
                        Vertex<CourseViewModel> dependee = courseViewModelLookup[course.InstitutionName][course.Code].VertexWrapper;
                        foreach (Vertex<CourseViewModel> dependant in dependee.GetDependants(includeNeighbours: true))
                        {
                            // Get all the concepts in those courses.  Add weight to them every time they're seen.
                            foreach (KeyValuePair<Concept, float> conceptEdge in ((CourseViewModel)dependant.Data).CourseModel.AssociatedConcepts)
                            {
                                if (!conceptEdges.ContainsKey(conceptEdge.Key))
                                    conceptEdges[conceptEdge.Key] = 0;

                                float prior = conceptEdges[conceptEdge.Key];
                                float delta = conceptEdge.Value * conceptEdge.Value;

                                conceptEdges[conceptEdge.Key] = (1 - prior) * delta;
                            }
                        }
                    }
                }

                // Create graph edges out of the information in dependantConcepts.
                foreach (KeyValuePair<Concept, float> conceptEdge in conceptEdges)
                {
                    float edgeWeight = conceptEdge.Value;
                    ConceptViewModel dependantConcept = conceptLookup[conceptEdge.Key];

                    Edge<ConceptViewModel> edge = new Edge<ConceptViewModel>()
                    {
                        Source = conceptViewModel.VertexWrapper,
                        Target = dependantConcept.VertexWrapper,
                        Weight = edgeWeight,
                        Variety = Edge<ConceptViewModel>.EdgeVariety.SourcePrerequisiteForTarget,
                        ID = GraphComponentIdGenerator++
                    };

                    conceptViewModel.VertexWrapper.Edges.Add(edge);
                    dependantConcept.VertexWrapper.Edges.Add(edge);
                }
            }
        }

        /// <summary>
        /// Adds a course to a searchable trie.
        /// </summary>
        private void Index(CourseViewModel viewModel, PatriciaTrie<CourseViewModel> courseTrie, ObservableCollection<Institution> courseTree)
        {
            // Index all words in the course's title and description
            courseTrie.Insert(viewModel.CourseModel.Title, viewModel);
            courseTrie.Insert(viewModel.CourseModel.Description, viewModel);

            // Add the view model to the course hierarchical view model.
            Institution institution = courseTree.FirstOrDefault(x => String.Equals(x.Name, viewModel.CourseModel.InstitutionName));
            if (institution == null) // Not found?  Create a new one.
            {
                institution = new Institution(viewModel.CourseModel.InstitutionName);
                courseTree.Add(institution);
            }
            institution.AddCourse(viewModel);
        }

        /// <summary>
        /// Adds a concept to a searchable trie.
        /// </summary>
        private void Index(ConceptViewModel viewModel, PatriciaTrie<ConceptViewModel> conceptTrie)
        {
            // Index all words in the concept's name and description
            conceptTrie.Insert(viewModel.ConceptModel.Name, viewModel);
            conceptTrie.Insert(viewModel.ConceptModel.Summary, viewModel);
        }

        #region Properties

        private DataModel _dataModel = new DataModel();
        public DataModel DataModel
        {
            get { return _dataModel; }
            set { SetField(ref _dataModel, value, "DataModel"); }
        }

        private ObservableCollection<Institution> _courseTree
            = new ObservableCollection<Institution>();
        /// <summary>
        /// For displaying course catalogues hierarchically.  Institutions->Faculties->Departments->Courses
        /// </summary>
        public ObservableCollection<Institution> CourseTree
        {
            get { return _courseTree; }
            set { SetField(ref _courseTree, value, "CourseTree"); }
        }

        private ObservableCollection<CourseViewModel> _courseViewModels =
            new ObservableCollection<CourseViewModel>();
        public ObservableCollection<CourseViewModel> CourseViewModels
        {
            get { return _courseViewModels; }
            set { SetField(ref _courseViewModels, value, "CourseViewModels"); }
        }

        private ObservableCollection<ConceptViewModel> _conceptViewModels =
            new ObservableCollection<ConceptViewModel>();
        public ObservableCollection<ConceptViewModel> ConceptViewModels
        {
            get { return _conceptViewModels; }
            set { SetField(ref _conceptViewModels, value, "ConceptViewModels"); }
        }

        private ObservableCollection<CourseViewModel> _courseSearchResults 
            = new ObservableCollection<CourseViewModel>();
        public ObservableCollection<CourseViewModel> CourseSearchResults
        {
            get { return _courseSearchResults; }
            set { SetField(ref _courseSearchResults, value, "CourseSearchResults"); }
        }

        private ObservableCollection<ConceptViewModel> _conceptSearchResults 
            = new ObservableCollection<ConceptViewModel>();
        public ObservableCollection<ConceptViewModel> ConceptSearchResults
        {
            get { return _conceptSearchResults; }
            set { SetField(ref _conceptSearchResults, value, "ConceptSearchResults"); }
        }

        private CourseViewModel _selectedCourse = null;
        public CourseViewModel SelectedCourse
        {
            get { return _selectedCourse; }
            set { SetField(ref _selectedCourse, value, "SelectedCourse"); }
        }

        private ConceptViewModel _selectedConcept = null;
        public ConceptViewModel SelectedConcept
        {
            get { return _selectedConcept; }
            set { SetField(ref _selectedConcept, value, "SelectedConcept"); }
        }

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set { SetField(ref _searchString, value, "SearchString"); }
        }

        #endregion // Properties


        #region INotifyPropertyChanged

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

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged
    }
}
