using System.Collections.Generic;

namespace MetaSyllabus.Models
{
    /// <summary>
    /// Represents a course offered at a post-secondary educational institution, like Stanford.
    /// </summary>
    public class Course
    {
        #region Public Members

        // Course code syntax varies between institutions.  It's usually just 
        // the concatenation of the department code and the course number.  
        // For example, Philosophy 101 would generally map to "PHIL101".
        public string Code;

        public string DepartmentName;
        public string Description;
        public string FacultyName;
        public string InstitutionName;
        public string Title;

        // When we build the graph using this data, each KVP is an edge, and the float is the weight
        public Dictionary<Concept, float> AssociatedConcepts = new Dictionary<Concept, float>();

        public List<string> Prerequisites = new List<string>();
        public List<string> Corequisites  = new List<string>();
        public List<string> Crosslistings = new List<string>();

        #endregion // Public Members
    }
}
