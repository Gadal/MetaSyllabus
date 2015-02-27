using System.Collections.Generic;

namespace MetaSyllabus.Models
{
    public class Concept
    {
        #region Public Members

        // When we build the graph using this data, each KVP is an edge, and the float is the weight
        public Dictionary<Course, float> AssociatedCourses = new Dictionary<Course, float>();

        public string Summary;
        public string Name;

        #endregion // Public Members
    }
}
