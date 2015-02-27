using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace MetaSyllabus.Animations
{
    /// <summary>
    /// Animates transitions between random elements of the Petersen Graph's automorphism group
    /// </summary>
    class PetersenGraphAnimation : Canvas
    {
        #region Private Members

        private int[][] Automorphisms; // 120 x 10
        private Vertex[][] Embeddings; // 8 x 10

        // path describes a vertex walk that generates a closed polygon with the same
        // unique edge set as the petersen graph (this involves some backtracking.)
        // In other words, it draws the petersen graph without lifting the pen off the paper.
        private int[] Path; // 20        

        private int CurrentAutomorphism, NextAutomorphism, CurrentEmbedding, NextEmbedding = 0;

        private Polyline Polyline;
        private PointCollectionAnimation PolylineAnimation;

        private Storyboard Storyboard = new Storyboard();

        private Random RandomNumberGenerator = new Random();

        // We only want to initialise the animation the first time we load it.
        // This is used to keep from rebuilding each instance every time it's loaded.
        private bool AlreadyInitialised = false;
        #endregion // Private Members

        #region Constructors
        public PetersenGraphAnimation()
        {
            // We defer drawing the animation until after the class has loaded.
            // That way, all dependency properties set from the XAML are updated 
            // BEFORE the object is drawn for the first time.
            this.Loaded += On_Loaded;
        }
        #endregion // Constructors

        #region Private Methods
        private void On_Loaded(object sender, EventArgs e)
        {
            if (AlreadyInitialised) { return; }
            else { AlreadyInitialised = true; }

            //
            // Load graph data
            //

            // Parse out the pre-computed automorphism group of the Petersen Graph
            // The canonical labelling is:
            //          0          
            //      
            //          1          
            //     
            // 2   3         4   5
            //       
            //       
            //       6     7       
            //        
            //    8           9    
            Automorphisms = new int[120][];
            string[] automorphismsArray = PetersenGraphAutomorphismGroupEncoding.Replace(" ", "")
                                                                                      .Replace("\r", "")
                                                                                      .Replace("\n", "")
                                                                                      .Replace("\t", "")
                                                                                      .Split(';');
            for (int i = 0; i < automorphismsArray.Count(); i++)
            {
                Automorphisms[i] = automorphismsArray[i].Split(',').Select(x => int.Parse(x)).ToArray();
            }

            // Embeddings are the same graph, drawn a different way:
            // 1____2    1_2_3
            // |    | vs |  /
            // 4____3    | /
            //           4
            // We create an array of arrays of vertices that represent different embeddings of the same graph.
            Embeddings = new Vertex[7][];
            string[] embeddingArray = EmbeddingsEncoding.Replace(" ", "")
                                                              .Replace("\r", "")
                                                              .Replace("\n", "")
                                                              .Replace("\t", "")
                                                              .Split('#');
            for (int i = 0; i < embeddingArray.Count(); i++)
            {
                string[] lineArray = embeddingArray[i].Split(';');
                Embeddings[i] = new Vertex[lineArray.Count()];
                for (int j = 0; j < lineArray.Count(); j++)
                {
                    Embeddings[i][j] = new Vertex()
                    {
                        index = j,
                        point = new Point(double.Parse(lineArray[j].Split(',').First()) * ScalingFactor,
                                          double.Parse(lineArray[j].Split(',').Last ()) * ScalingFactor)
                
                    };
                }
            }

            // One solution to the route inspection problem for the petersen graph
            // (A minimal circuit that visits each edge at least once)
            Path = PathEncoding.Replace(" ", "").Split(',').Select(x => int.Parse(x)).ToArray();

            //
            // Add the graphical elements to our storyboard
            //

            // Height/Width = scaling factor + (2 * 1/2 * line stroke thickness)
            //                                  ^----^ One half of the line would poke out of frame
            //                                         on the left and the right unless we add this.
            Height = Width = ScalingFactor + EdgeThickness;

            Duration animationDuration = new Duration(TimeSpan.FromSeconds(AnimationDuration));
            SineEase animationEase = new SineEase() { EasingMode = EasingMode.EaseOut };

            BrushConverter brushConverter = new BrushConverter();
            Brush brush = (Brush)brushConverter.ConvertFrom((string)EdgeColour);

            Polyline = new Polyline() 
            {
                StrokeThickness = EdgeThickness,
                Stroke = brush,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap   = PenLineCap.Round,
                StrokeLineJoin     = PenLineJoin.Round,
                Effect = new DropShadowEffect()
                {
                    Color = Color.FromRgb(0, 0, 0),
                    Direction = 290,
                    Opacity = 0.20,
                }
            };

            this.Children.Add(Polyline);

            PolylineAnimation = new PointCollectionAnimation() 
            {
                Duration = animationDuration,
                EasingFunction = animationEase
            };

            foreach (int i in Path)
            {
                PolylineAnimation.From.Add(Image(i, CurrentAutomorphism, CurrentEmbedding).point);
                PolylineAnimation.To.Add(Image(i, NextAutomorphism, NextEmbedding).point);
            }

            //
            // Run the animation
            //

            Storyboard.Children.Add(PolylineAnimation);
            Storyboard.SetTarget(PolylineAnimation, Polyline);
            Storyboard.SetTargetProperty(PolylineAnimation, new PropertyPath("Points"));

            SetAnimations();
            Storyboard.Completed += UpdateStoryboard;
            Storyboard.Begin();
        }

        /// <summary>
        /// Called at the end of each animation cycle.  Calculates a new random keyframe and resets the animation.
        /// </summary>
        private void UpdateStoryboard(object sender, EventArgs e)
        {
            Storyboard.Stop();
            UpdateAnimations();
            Storyboard.Begin();
        }

        /// <summary>
        /// Discards the old animation keyframe, resets the current keyframe, then randomly generates a new keyframe.
        /// </summary>
        private void UpdateAnimations()
        {
            
            CurrentAutomorphism = NextAutomorphism;
            NextAutomorphism = RandomNumberGenerator.Next(119);
            
            CurrentEmbedding = NextEmbedding;
            if (RandomNumberGenerator.Next(2) == 0) // Makes it a 2/3 chance the embedding stays the same.
                NextEmbedding = RandomNumberGenerator.Next(7);

            SetAnimations();
        }

        /// <summary>
        /// Finds the image of each edge under the current automorphism, and then animate it to the image of that edge under the next automorphism.
        /// </summary>
        private void SetAnimations()
        {
            PointCollection newFrom = new PointCollection();
            PointCollection newTo   = new PointCollection();
            foreach (int i in Path)
            {
                newFrom.Add(Image(i, CurrentAutomorphism, CurrentEmbedding).point);
                newTo.Add(Image(i, NextAutomorphism, NextEmbedding).point);
            }
            PolylineAnimation.From = newFrom;
            PolylineAnimation.To   = newTo;
        }
        
        /// <summary>
        /// Finds the image of vertex v_i under a given automorphism and embedding.
        /// </summary>
        private Vertex Image(int v_i, int automorphism, int embedding)
        {
            return Embeddings[embedding][Automorphisms[automorphism][v_i]];
        }
        #endregion // Private Methods

        #region Vertex Nested Struct
        private struct Vertex
        {
            public int index;
            public Point point;
        }
        #endregion // Nested Struct

        #region Public Attached Properties

        #region ScalingFactor
        public static DependencyProperty ScalingFactorProperty = DependencyProperty.Register(
            "ScalingFactor",
            typeof(float),
            typeof(PetersenGraphAnimation),
            new PropertyMetadata(1f)); // Defaults to 1

        public float ScalingFactor
        {
            get { return (float)GetValue(ScalingFactorProperty); }
            set { SetValue(ScalingFactorProperty, value); }
        }
        #endregion // ScalingFactor

        #region AnimationDuration
        public static DependencyProperty AnimationDurationProperty = DependencyProperty.Register(
            "AnimationDuration",
            typeof(float),
            typeof(PetersenGraphAnimation),
            new PropertyMetadata(1f)); // Defaults to 1 second

        public float AnimationDuration
        {
            get { return (float)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }
        #endregion // AnimationDuration

        #region EdgeThickness
        public static DependencyProperty EdgeThicknessProperty = DependencyProperty.Register(
            "EdgeThickness",
            typeof(float),
            typeof(PetersenGraphAnimation),
            new PropertyMetadata(1f)); // Defaults to 1

        public float EdgeThickness
        {
            get { return (float)GetValue(EdgeThicknessProperty); }
            set { SetValue(EdgeThicknessProperty, value); }
        }
        #endregion // EdgeThickness

        #region EdgeColour
        public static DependencyProperty EdgeColourProperty = DependencyProperty.Register(
            "EdgeColour",
            typeof(string),
            typeof(PetersenGraphAnimation),
            new PropertyMetadata("#FFFFFF")); // Defaults to white

        public string EdgeColour
        {
            get { return (string)GetValue(EdgeColourProperty); }
            set { SetValue(EdgeColourProperty, value); }
        }
        #endregion // EdgeColour

        #endregion // Public Attached Properties

        #region Data

        // Generated this in python by reimplementing NAutY
        // Details @ http://www.math.unl.edu/~aradcliffe1/Papers/Canonical.pdf
        private const string PetersenGraphAutomorphismGroupEncoding =
              @"0, 1, 2, 3, 4, 5, 6, 7, 8, 9; 
                0, 1, 2, 8, 9, 5, 7, 6, 3, 4; 
                0, 1, 5, 4, 3, 2, 7, 6, 9, 8; 
                0, 1, 5, 9, 8, 2, 6, 7, 4, 3; 
                0, 2, 1, 6, 4, 5, 3, 8, 7, 9; 
                0, 2, 1, 7, 9, 5, 8, 3, 6, 4; 
                0, 2, 5, 4, 6, 1, 8, 3, 9, 7; 
                0, 2, 5, 9, 7, 1, 3, 8, 4, 6; 
                0, 5, 1, 6, 8, 2, 9, 4, 7, 3; 
                0, 5, 1, 7, 3, 2, 4, 9, 6, 8; 
                0, 5, 2, 3, 7, 1, 9, 4, 8, 6; 
                0, 5, 2, 8, 6, 1, 4, 9, 3, 7; 
                1, 0, 6, 4, 3, 7, 2, 5, 8, 9; 
                1, 0, 6, 8, 9, 7, 5, 2, 4, 3; 
                1, 0, 7, 3, 4, 6, 5, 2, 9, 8; 
                1, 0, 7, 9, 8, 6, 2, 5, 3, 4; 
                1, 6, 0, 2, 3, 7, 4, 8, 5, 9; 
                1, 6, 0, 5, 9, 7, 8, 4, 2, 3; 
                1, 6, 7, 3, 2, 0, 8, 4, 9, 5; 
                1, 6, 7, 9, 5, 0, 4, 8, 3, 2; 
                1, 7, 0, 2, 8, 6, 9, 3, 5, 4; 
                1, 7, 0, 5, 4, 6, 3, 9, 2, 8; 
                1, 7, 6, 4, 5, 0, 9, 3, 8, 2; 
                1, 7, 6, 8, 2, 0, 3, 9, 4, 5; 
                2, 0, 3, 4, 6, 8, 1, 5, 7, 9; 
                2, 0, 3, 7, 9, 8, 5, 1, 4, 6; 
                2, 0, 8, 6, 4, 3, 5, 1, 9, 7; 
                2, 0, 8, 9, 7, 3, 1, 5, 6, 4; 
                2, 3, 0, 1, 6, 8, 4, 7, 5, 9; 
                2, 3, 0, 5, 9, 8, 7, 4, 1, 6; 
                2, 3, 8, 6, 1, 0, 7, 4, 9, 5; 
                2, 3, 8, 9, 5, 0, 4, 7, 6, 1; 
                2, 8, 0, 1, 7, 3, 9, 6, 5, 4; 
                2, 8, 0, 5, 4, 3, 6, 9, 1, 7; 
                2, 8, 3, 4, 5, 0, 9, 6, 7, 1; 
                2, 8, 3, 7, 1, 0, 6, 9, 4, 5; 
                3, 2, 4, 5, 9, 7, 8, 0, 6, 1; 
                3, 2, 4, 6, 1, 7, 0, 8, 5, 9; 
                3, 2, 7, 1, 6, 4, 8, 0, 9, 5; 
                3, 2, 7, 9, 5, 4, 0, 8, 1, 6; 
                3, 4, 2, 0, 1, 7, 6, 5, 8, 9; 
                3, 4, 2, 8, 9, 7, 5, 6, 0, 1; 
                3, 4, 7, 1, 0, 2, 5, 6, 9, 8; 
                3, 4, 7, 9, 8, 2, 6, 5, 1, 0; 
                3, 7, 2, 0, 5, 4, 9, 1, 8, 6; 
                3, 7, 2, 8, 6, 4, 1, 9, 0, 5; 
                3, 7, 4, 5, 0, 2, 1, 9, 6, 8; 
                3, 7, 4, 6, 8, 2, 9, 1, 5, 0; 
                4, 3, 5, 0, 1, 6, 7, 2, 9, 8; 
                4, 3, 5, 9, 8, 6, 2, 7, 0, 1; 
                4, 3, 6, 1, 0, 5, 2, 7, 8, 9; 
                4, 3, 6, 8, 9, 5, 7, 2, 1, 0; 
                4, 5, 3, 2, 8, 6, 9, 0, 7, 1; 
                4, 5, 3, 7, 1, 6, 0, 9, 2, 8; 
                4, 5, 6, 1, 7, 3, 9, 0, 8, 2; 
                4, 5, 6, 8, 2, 3, 0, 9, 1, 7; 
                4, 6, 3, 2, 0, 5, 1, 8, 7, 9; 
                4, 6, 3, 7, 9, 5, 8, 1, 2, 0; 
                4, 6, 5, 0, 2, 3, 8, 1, 9, 7; 
                4, 6, 5, 9, 7, 3, 1, 8, 0, 2; 
                5, 0, 4, 3, 7, 9, 1, 2, 6, 8; 
                5, 0, 4, 6, 8, 9, 2, 1, 3, 7; 
                5, 0, 9, 7, 3, 4, 2, 1, 8, 6; 
                5, 0, 9, 8, 6, 4, 1, 2, 7, 3; 
                5, 4, 0, 1, 7, 9, 3, 6, 2, 8; 
                5, 4, 0, 2, 8, 9, 6, 3, 1, 7; 
                5, 4, 9, 7, 1, 0, 6, 3, 8, 2; 
                5, 4, 9, 8, 2, 0, 3, 6, 7, 1; 
                5, 9, 0, 1, 6, 4, 8, 7, 2, 3; 
                5, 9, 0, 2, 3, 4, 7, 8, 1, 6; 
                5, 9, 4, 3, 2, 0, 8, 7, 6, 1; 
                5, 9, 4, 6, 1, 0, 7, 8, 3, 2; 
                6, 1, 4, 3, 2, 8, 0, 7, 5, 9; 
                6, 1, 4, 5, 9, 8, 7, 0, 3, 2; 
                6, 1, 8, 2, 3, 4, 7, 0, 9, 5; 
                6, 1, 8, 9, 5, 4, 0, 7, 2, 3; 
                6, 4, 1, 0, 2, 8, 3, 5, 7, 9; 
                6, 4, 1, 7, 9, 8, 5, 3, 0, 2; 
                6, 4, 8, 2, 0, 1, 5, 3, 9, 7; 
                6, 4, 8, 9, 7, 1, 3, 5, 2, 0; 
                6, 8, 1, 0, 5, 4, 9, 2, 7, 3; 
                6, 8, 1, 7, 3, 4, 2, 9, 0, 5; 
                6, 8, 4, 3, 7, 1, 9, 2, 5, 0; 
                6, 8, 4, 5, 0, 1, 2, 9, 3, 7; 
                7, 1, 3, 2, 8, 9, 6, 0, 4, 5; 
                7, 1, 3, 4, 5, 9, 0, 6, 2, 8; 
                7, 1, 9, 5, 4, 3, 6, 0, 8, 2; 
                7, 1, 9, 8, 2, 3, 0, 6, 5, 4; 
                7, 3, 1, 0, 5, 9, 4, 2, 6, 8; 
                7, 3, 1, 6, 8, 9, 2, 4, 0, 5; 
                7, 3, 9, 5, 0, 1, 2, 4, 8, 6; 
                7, 3, 9, 8, 6, 1, 4, 2, 5, 0; 
                7, 9, 1, 0, 2, 3, 8, 5, 6, 4; 
                7, 9, 1, 6, 4, 3, 5, 8, 0, 2; 
                7, 9, 3, 2, 0, 1, 5, 8, 4, 6; 
                7, 9, 3, 4, 6, 1, 8, 5, 2, 0; 
                8, 2, 6, 1, 7, 9, 3, 0, 4, 5; 
                8, 2, 6, 4, 5, 9, 0, 3, 1, 7; 
                8, 2, 9, 5, 4, 6, 3, 0, 7, 1; 
                8, 2, 9, 7, 1, 6, 0, 3, 5, 4; 
                8, 6, 2, 0, 5, 9, 4, 1, 3, 7; 
                8, 6, 2, 3, 7, 9, 1, 4, 0, 5; 
                8, 6, 9, 5, 0, 2, 1, 4, 7, 3; 
                8, 6, 9, 7, 3, 2, 4, 1, 5, 0; 
                8, 9, 2, 0, 1, 6, 7, 5, 3, 4; 
                8, 9, 2, 3, 4, 6, 5, 7, 0, 1; 
                8, 9, 6, 1, 0, 2, 5, 7, 4, 3; 
                8, 9, 6, 4, 3, 2, 7, 5, 1, 0; 
                9, 5, 7, 1, 6, 8, 4, 0, 3, 2; 
                9, 5, 7, 3, 2, 8, 0, 4, 1, 6; 
                9, 5, 8, 2, 3, 7, 4, 0, 6, 1; 
                9, 5, 8, 6, 1, 7, 0, 4, 2, 3; 
                9, 7, 5, 0, 2, 8, 3, 1, 4, 6; 
                9, 7, 5, 4, 6, 8, 1, 3, 0, 2; 
                9, 7, 8, 2, 0, 5, 1, 3, 6, 4; 
                9, 7, 8, 6, 4, 5, 3, 1, 2, 0; 
                9, 8, 5, 0, 1, 7, 6, 2, 4, 3; 
                9, 8, 5, 4, 3, 7, 2, 6, 0, 1; 
                9, 8, 7, 1, 0, 5, 2, 6, 3, 4; 
                9, 8, 7, 3, 4, 5, 6, 2, 1, 0";

        // Source images are from WolframMathWorld: http://mathworld.wolfram.com/PetersenGraph.html
        private const string EmbeddingsEncoding =
              @"0.51, 0.00;
                0.51, 0.27;
                0.00, 0.37;
                0.25, 0.47;
                0.75, 0.47;
                1.00, 0.37;
                0.35, 0.77;
                0.65, 0.77;
                0.20, 1.00;
                0.81, 1.00#

                0.89, 0.00;
                0.19, 0.00;
                0.67, 0.19;
                0.00, 0.19;
                0.34, 0.51;
                1.00, 0.51;
                0.00, 0.81;
                0.19, 1.00;
                0.67, 0.81;
                0.89, 1.00#

                0.50, 0.50;
                0.50, 0.21;
                0.29, 0.63;
                0.75, 1.00;
                0.25, 1.00;
                0.72, 0.63;
                0.00, 0.50;
                1.00, 0.50;
                0.25, 0.00;
                0.75, 0.00#

                0.50, 0.00;
                0.50, 1.00;
                0.85, 0.15;
                0.25, 0.35;
                0.75, 0.35;
                0.15, 0.15;
                0.85, 0.85;
                0.15, 0.85;
                1.00, 0.50;
                0.00, 0.50#

                0.50, 0.52;
                0.50, 0.00;
                0.06, 0.77;
                0.32, 1.00;
                0.67, 1.00;
                0.94, 0.77;
                0.17, 0.12;
                0.82, 0.12;
                0.00, 0.42;
                1.00, 0.42#

                0.50, 0.55;
                0.17, 0.33;
                0.82, 0.33;
                1.00, 0.67;
                0.82, 1.00;
                0.50, 1.00;
                0.34, 0.0 ;
                0.00, 0.67;
                0.65, 0.00;
                0.17, 1.00#

                0.50, 0.00;
                0.50, 1.00;
                0.00, 0.00;
                0.00, 0.50;
                1.00, 0.50;
                1.00, 0.00;
                1.00, 1.00;
                0.00, 1.00;
                0.25, 0.25;
                0.75, 0.25";

        private const string PathEncoding = "0, 5, 9, 8, 2, 0, 1, 7, 9, 7, 3, 2, 3, 4, 5, 4, 6, 8, 6, 1";

        #endregion // Data
    }
}
