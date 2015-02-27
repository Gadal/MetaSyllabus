using GraphX;
using GraphX.Logic;
using QuickGraph;

namespace MetaSyllabus.Graphing
{
    /// <summary>
    /// Manages graph layout logic 
    /// </summary>
    public class LogicCore<T> : GXLogicCore<Vertex<T>, Edge<T>, BidirectionalGraph<Vertex<T>, Edge<T>>> 
    {
        public LogicCore()
        {
            // Sugiyama is a layered graph.  Looks like this:
            //                                                  A   B
            //                                                 / \ /
            //                                                C   D
            //                                               / \   \
            //                                              E   F   G

            DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama;
            DefaultLayoutAlgorithmParams =
                AlgorithmFactory.CreateLayoutParameters(
                    LayoutAlgorithmTypeEnum.EfficientSugiyama);
            
            DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            DefaultOverlapRemovalAlgorithmParams =
                AlgorithmFactory.CreateOverlapRemovalParameters(
                    OverlapRemovalAlgorithmTypeEnum.FSA);

            DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            AsyncAlgorithmCompute = false;
        }
    }
}
