using GraphX;

namespace MetaSyllabus.Graphing
{
    public class Edge<T> : EdgeBase<Vertex<T>>
    {
        public Edge(Vertex<T> source, Vertex<T> target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public Edge()
            : base(null, null, 1)
        {
        }

        public enum EdgeVariety
        {
            Unspecified,
            SourcePrerequisiteForTarget,
            Equivalents // Corequisite, crosslisted
        }

        public EdgeVariety Variety;

        public override int GetHashCode()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
