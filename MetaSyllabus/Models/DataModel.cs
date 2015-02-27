using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MetaSyllabus.Models
{
    /// <summary>
    /// Reads in and stores Course and Concept data from the disk.
    /// </summary>
    public class DataModel : INotifyPropertyChanged
    {
        #region Public Members
        public event RunWorkerCompletedEventHandler DataModelLoaded;
        #endregion // Public Members

        #region Private Members

        private const string CourseDataSource = "..\\..\\Data\\CourseData.txt";
        private const string ConceptDataSource = "..\\..\\Data\\ConceptData.txt";

        //private PatriciaTrie<Course> CourseTrie = new PatriciaTrie<Course>();
        //private PatriciaTrie<Concept> ConceptTrie = new PatriciaTrie<Concept>();

        //private Graph FullCourseGraph  = new Graph();
        //private Graph FullConceptGraph = new Graph();

        //private Dictionary<Course, Vertex>  CourseVertexLookup  = new Dictionary<Course, Vertex>();
        //private Dictionary<Concept, Vertex> ConceptVertexLookup = new Dictionary<Concept, Vertex>();

        //private BackgroundWorker GraphUpdater = new BackgroundWorker();

        #endregion // Private Members

        #region Constructors

        public DataModel()
        {
            BackgroundWorker modelBuilder = new BackgroundWorker();
            modelBuilder.DoWork += ModelBuilder_DoWork;
            modelBuilder.RunWorkerCompleted += ModelBuilder_RunWorkerCompleted;
            modelBuilder.RunWorkerAsync();
        }

        #endregion // Constructors

        #region Private/Protected Methods

        private Dictionary<string, Concept> LoadConcepts()
        {
            // Read in the concept data
            string rawConceptData;
            using (Stream fileStream = File.OpenRead(ConceptDataSource))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    rawConceptData = reader.ReadToEnd();
                }
            }
            JsonTextReader conceptData = new JsonTextReader(new StringReader(rawConceptData));

            Dictionary<string, Concept> concepts = null;
            Concept concept = null;
            while (conceptData.Read())
            {
                // Start of concept array
                if (conceptData.TokenType == JsonToken.StartArray)
                {
                    concepts = new Dictionary<string, Concept>();
                    continue;
                }

                // End of concept array
                if (conceptData.TokenType == JsonToken.EndArray)
                    break;

                // Start of Concept object
                if (conceptData.TokenType == JsonToken.StartObject)
                {
                    concept = new Concept();
                    continue;
                }

                // End of Concept object
                if (conceptData.TokenType == JsonToken.EndObject)
                {
                    concepts[concept.Name] = concept;
                    concept = null;
                    continue;
                }

                // Populate concept properties
                if (conceptData.TokenType == JsonToken.PropertyName)
                {
                    string propertyType = (string)conceptData.Value;
                    conceptData.Read();
                    string propertyValue = (string)conceptData.Value;

                    switch (propertyType)
                    {
                        case "abstract":
                            concept.Summary = propertyValue;
                            continue;
                        case "text":
                            concept.Name = propertyValue;
                            continue;
                        default:
                            continue;
                    }
                }
            }

            return concepts;
        }

        private List<Course> LoadCourses(Dictionary<string, Concept> concepts)
        {
            // Read in the course data
            string rawCourseData;
            using (Stream fileStream = File.OpenRead(CourseDataSource))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    rawCourseData = reader.ReadToEnd();
                }
            }
            JsonTextReader courseData = new JsonTextReader(new StringReader(rawCourseData));

            List<Course> courses = null;
            Course course = null;

            // Parse our JSON into a list of courses
            while (courseData.Read())
            {
                // Start of Course array
                if (courseData.TokenType == JsonToken.StartArray)
                {
                    courses = new List<Course>();
                    continue;
                }

                // End of Course array
                if (courseData.TokenType == JsonToken.EndArray)
                    break;

                // Start of Course object
                if (courseData.TokenType == JsonToken.StartObject)
                {
                    course = new Course();

                    continue;
                }

                // End of Course object
                if (courseData.TokenType == JsonToken.EndObject)
                {
                    courses.Add(course);
                    
                    continue;
                }

                // Populate course properties
                if (courseData.TokenType == JsonToken.PropertyName)
                {
                    // Populate simple properties
                    switch ((string)courseData.Value)
                    {
                        case "faculty":
                            courseData.Read();
                            course.FacultyName = (string)courseData.Value;
                            continue;
                        case "institution":
                            courseData.Read();
                            course.InstitutionName = (string)courseData.Value;
                            continue;
                        case "description":
                            courseData.Read();
                            course.Description = (string)courseData.Value;
                            continue;
                        case "title":
                            courseData.Read();
                            course.Title = (string)courseData.Value;
                            continue;
                        case "department":
                            courseData.Read();
                            course.DepartmentName = (string)courseData.Value;
                            continue;
                        case "code":
                            courseData.Read();
                            course.Code = ((string)courseData.Value).Replace(" ", "");
                            continue;
                        // No default.
                    }

                    if ((string)courseData.Value == "corequisites")
                    {
                        while(courseData.Read())
                        {
                            // End of collection of corequisites
                            if (courseData.TokenType == JsonToken.EndArray)
                                break;

                            if (courseData.TokenType == JsonToken.String)
                            {
                                string courseCode = ((string)courseData.Value).Replace(" ", "");
                                course.Corequisites.Add(courseCode);
                            }
                        }
                    }

                    if ((string)courseData.Value == "prerequisites")
                    {
                        while (courseData.Read())
                        {
                            // End of collection of prerequisites
                            if (courseData.TokenType == JsonToken.EndArray)
                                break;

                            if (courseData.TokenType == JsonToken.String)
                            {
                                string courseCode = ((string)courseData.Value).Replace(" ", "");
                                course.Prerequisites.Add(courseCode);
                            }
                        }
                    }

                    if ((string)courseData.Value == "crosslisted")
                    {
                        while (courseData.Read())
                        {
                            // End of collection of crosslistings
                            if (courseData.TokenType == JsonToken.EndArray)
                                break;

                            if (courseData.TokenType == JsonToken.String)
                            {
                                string courseCode = ((string)courseData.Value).Replace(" ", "");
                                course.Crosslistings.Add(courseCode);
                            }
                        }
                    }

                    // Populate the course's list of concepts
                    if ((string)courseData.Value == "concepts")
                    {
                        Concept currentConcept = null;
                        float conceptRelevance = -1;

                        while (courseData.Read())
                        {
                            // Start of Concept array
                            if (courseData.TokenType == JsonToken.StartArray)
                            {
                                continue;
                            }

                            // End of Concept array
                            if (courseData.TokenType == JsonToken.EndArray)
                            {
                                break;
                            }

                            // Start of Concept object
                            if (courseData.TokenType == JsonToken.StartObject)
                            {
                                currentConcept = new Concept();
                                conceptRelevance = 0;

                                continue;
                            }

                            // End of Concept object
                            if (courseData.TokenType == JsonToken.EndObject)
                            {
                                // Note that edge weights won't be hysteresis-free.

                                if (currentConcept != null && conceptRelevance >= 0)
                                {
                                    if (!currentConcept.AssociatedCourses.ContainsKey(course))
                                        currentConcept.AssociatedCourses[course] = 0;
                                    if (!course.AssociatedConcepts.ContainsKey(currentConcept))
                                        course.AssociatedConcepts[currentConcept] = 0;

                                    float delta = conceptRelevance * conceptRelevance;
                                    currentConcept.AssociatedCourses[course]  += (1 - currentConcept.AssociatedCourses[course] ) * delta;
                                    course.AssociatedConcepts[currentConcept] += (1 - course.AssociatedConcepts[currentConcept]) * delta;
                                }

                                continue;
                            }

                            if (courseData.TokenType == JsonToken.PropertyName)
                            {
                                string propertyName = (string)courseData.Value;
                                courseData.Read();
                                string propertyValue = (string)courseData.Value;

                                if (propertyName == "text")
                                {
                                    currentConcept = concepts[propertyValue];
                                }
                                else if (propertyName == "relevance")
                                {
                                    conceptRelevance = float.Parse(propertyValue);
                                }
                                continue;
                            }
                        }
                    }

                    if ((string)courseData.Value == "keywords")
                    {
                        //TODO: Is there anything useful I can do with keywords?
                        while (courseData.Read())
                        {
                            if (courseData.TokenType == JsonToken.EndArray)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return courses;
        }

        private void ModelBuilder_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, Concept> concepts = LoadConcepts();
            List<Course> courses = LoadCourses(concepts);

            Concepts = concepts.Values.ToList();
            Courses = courses;
        }
        
        private void ModelBuilder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnDataModelLoaded();
        }

        protected void OnDataModelLoaded()
        {
            RunWorkerCompletedEventHandler handler = DataModelLoaded;

            if (handler != null)
                handler(this, new RunWorkerCompletedEventArgs(null, null, false));
        }
        
        #endregion // Private/Protected Methods

        #region Properties

        private List<Concept> _concepts = new List<Concept>();
        public List<Concept> Concepts
        {
            get { return _concepts; }
            private set { SetField(ref _concepts, value, "Concepts"); }
        }

        private List<Course> _courses = new List<Course>();
        public List<Course> Courses
        {
            get { return _courses; }
            private set { SetField(ref _courses, value, "Courses"); }
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
