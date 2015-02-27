using GraphX;
using System.Collections.Generic;
using System.Linq;

namespace MetaSyllabus.Graphing
{
    public class Vertex<T> : VertexBase
    {
        #region Public Members
        
        public object Data;
        public List<Edge<T>> Edges = new List<Edge<T>>();
        
        #endregion // Public Members

        public override string ToString()
        {
            return Data.ToString();
        }
        public string Text { get; set; }


        #region Public Methods

        /// <summary>
        /// Collects all vertices in the the component this vertex belongs to formed by equivalent-type edges.
        /// </summary>
        public IEnumerable<Vertex<T>> GetEquivalents(int degree = 0)
        {
            HashSet<Vertex<T>> component = new HashSet<Vertex<T>>();
            component.Add(this);
            GetEquivalentsInternal(ref component, degree);
            return component;
        }

        /// <summary>
        /// Collects all vertices that directly depend on this vertex.
        /// </summary>
        /// <param name="includeNeighbours">If true, GetDependents instead returns all vertices that directly depend on
        /// this vertex or one of its equivalents.</param>
        public IEnumerable<Vertex<T>> GetDependants(bool includeNeighbours = false)
        {
            HashSet<Vertex<T>> uniqueDependants = new HashSet<Vertex<T>>();
            List<Vertex<T>> dependees = includeNeighbours ? GetEquivalents(1).ToList() : GetEquivalents(0).ToList();
            foreach (Vertex<T> vertex in dependees)
            {
                foreach (Edge<T> edge in vertex.Edges.Where(x => x.Variety == Edge<T>.EdgeVariety.SourcePrerequisiteForTarget)
                                                     .Where(x => x.Target != vertex))
                {
                    uniqueDependants.Add(edge.Target);
                }
            }
            return uniqueDependants;
        }

        #endregion // Public Methods

        #region Private Methods

        private void GetEquivalentsInternal(ref HashSet<Vertex<T>> component, int degree)
        {
            if (degree == 0) { return; }

            foreach (Edge<T> edge in Edges.Where(x => x.Variety == Edge<T>.EdgeVariety.Equivalents))
            {
                if (component.Add(edge.Source)) // Hashset.Add returns false if the element is already present.
                    edge.Source.GetEquivalentsInternal(ref component, degree - 1);

                else if (component.Add(edge.Target))
                    edge.Target.GetEquivalentsInternal(ref component, degree - 1);
            }
        }

        #endregion // Private Methods

        public override int GetHashCode()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            var x = GetHashCode().Equals(obj.GetHashCode());
            return x;
        }
    }
}
