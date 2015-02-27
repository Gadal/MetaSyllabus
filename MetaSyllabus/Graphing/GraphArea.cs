using GraphX;
using MetaSyllabus.ViewModels;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MetaSyllabus.Graphing
{
    // WPF, unfortunately, doesn't support generics on arbitrary elements.
    // See section "Generics Support in WPF and Other v3.5 Frameworks" at
    // https://msdn.microsoft.com/en-us/library/ee956431%28v=vs.110%29.aspx
    // for more details.

    public class CourseGraphArea  : GraphArea<CourseViewModel>  { }
    public class ConceptGraphArea : GraphArea<ConceptViewModel> { }

    public class GraphArea<T> : GraphArea<Vertex<T>, Edge<T>, BidirectionalGraph<Vertex<T>, Edge<T>>>
    {
        #region Private Members

        protected readonly int MaxEdgesPerLayer = 5;

        #endregion // Private Members

        #region Constructors
        public GraphArea()
        {
            LogicCore = new LogicCore<T>();
        }
        #endregion // Constructors

        private void Update()
        {
            InvalidateArrange();
            ClearLayout();
            
            Graph<T> graph = new Graph<T>();
            LogicCore.Graph = graph;

            if (SelectedVertex != null)
            {
                SelectedVertex.Edges.OrderByDescending(x => x.Weight);

                HashSet<Vertex<T>> seenVertices = new HashSet<Vertex<T>>();
                seenVertices.Add(SelectedVertex);

                List<Edge<T>> edges = new List<Edge<T>>();

                // Get incident edges
                for (int i = 0; i < SelectedVertex.Edges.Count; i++)
                {
                    Edge<T> edge = SelectedVertex.Edges.ElementAt(i);

                    if (edge.Source == SelectedVertex || seenVertices.Contains(edge.Source)) { continue; }

                    seenVertices.Add(edge.Source);
                    edges.Add(edge);

                    if (edges.Count >= MaxEdgesPerLayer) { break; }
                }

                int incidentEdgeCount = edges.Count;

                // Get outgoing edges
                for (int i = 0; i < SelectedVertex.Edges.Count; i++)
                {
                    Edge<T> edge = SelectedVertex.Edges.ElementAt(i);

                    if (edge.Target == SelectedVertex || seenVertices.Contains(edge.Target)) { continue; }

                    seenVertices.Add(edge.Target);
                    edges.Add(edge);

                    if (edges.Count >= MaxEdgesPerLayer + incidentEdgeCount) { break; }
                }

                Dictionary<Vertex<T>, VertexControl> VertexControlLookup = new Dictionary<Vertex<T>, VertexControl>();
                foreach (Vertex<T> vertex in seenVertices.ToList())
                {
                    VertexControl vertexControl = new VertexControl(vertex);
                    vertexControl.MouseLeftButtonDown += VertexControl_MouseLeftButtonDown;
                    vertexControl.MouseEnter += VertexControl_MouseEnter;
                    VertexControlLookup[vertex] = vertexControl;
                    
                    graph.AddVertex(vertex);
                    AddVertex(vertex, VertexControlLookup[vertex]);
                }

                foreach (Edge<T> edge in edges)
                {    
                    InsertEdge(edge, new EdgeControl(VertexControlLookup[edge.Source], VertexControlLookup[edge.Target], edge));
                    graph.AddEdge(edge);
                }
            }
            
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                // Sometimes RelayoutGraph botches the graph and tries to create a rectangle with negative dimensions.
                // I don't control GraphX, so I suppose this will suffice.
                try { RelayoutGraph(); }
                catch (ArgumentException) { }
            }));
        }

        private void VertexControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VertexControl vertexControl = sender as VertexControl;

            if (vertexControl == null) { return; }

            SelectedVertex = vertexControl.Vertex as Vertex<T>;
        }

        private void VertexControl_MouseEnter(object sender, MouseEventArgs e)
        {
            VertexControl vertexControl = sender as VertexControl;

            if (vertexControl == null) { return; }

            vertexControl.BringIntoView();
        }

        #region Properties

        #region SelectedVertex
        public Vertex<T> SelectedVertex
        {
            get { return (Vertex<T>)GetValue(SelectedVertexProperty); }
            set { SetValue(SelectedVertexProperty, value); }
        }

        public static DependencyProperty SelectedVertexProperty = DependencyProperty.Register(
            "SelectedVertex",
            typeof(Vertex<T>),
            typeof(GraphArea<T>),
            new FrameworkPropertyMetadata(SelectedVertexChanged));

        private static void SelectedVertexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GraphArea<T> graphArea = sender as GraphArea<T>;

            if (graphArea != null)
                graphArea.Update();
        }
        #endregion // SelectedVertex

        #endregion // Properties
    }
}
