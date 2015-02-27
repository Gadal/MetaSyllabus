using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MetaSyllabus.Animations
{
    public class PointCollectionAnimation : PointCollectionAnimationBase
    {
        #region Private Members
        private PointCollection PresentCollection = new PointCollection();
        #endregion // Private Members

        #region Constructors

        public PointCollectionAnimation()
        {
            From = new PointCollection();
            To   = new PointCollection();
        }
        #endregion // Constructors

        #region Properties

        #region FromProperty
        public static DependencyProperty FromProperty = DependencyProperty.Register(
            "From",
            typeof(PointCollection),
            typeof(PointCollectionAnimation),
            new PropertyMetadata(null));

        public PointCollection From
        {
            get { return (PointCollection)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        #endregion // FromProperty

        #region ToProperty
        public static DependencyProperty ToProperty = DependencyProperty.Register(
            "To",
            typeof(PointCollection),
            typeof(PointCollectionAnimation),
            new PropertyMetadata(null));

        public PointCollection To
        {
            get { return (PointCollection)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
        #endregion // ToProperty

        #region ByProperty
        public static DependencyProperty ByProperty = DependencyProperty.Register(
            "By",
            typeof(PointCollection),
            typeof(PointCollectionAnimation),
            new PropertyMetadata(null));

        public PointCollection By
        {
            get { return (PointCollection)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }
        #endregion // ByProperty

        #region EasingFunction
        public static DependencyProperty EasingFunctionProperty = DependencyProperty.Register(
            "EasingFunction",
            typeof(EasingFunctionBase),
            typeof(PointCollectionAnimation),
            new PropertyMetadata(null));

        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }
        #endregion // EasingFunction

        // MSDN: "Indicates whether the target property's current value should be 
        //        added to this animation's starting value."
        public bool IsAdditive
        {
            get { return (bool)GetValue(IsAdditiveProperty); }
            set { SetValue(IsAdditiveProperty, value); }
        }

        // MSDN: "When this property is set to true, the animation's output values only 
        //        accumulate when the animation's RepeatBehavior property causes it to repeat"
        public bool IsCumulative
        {
            get { return (bool)GetValue(IsCumulativeProperty); }
            set { SetValue(IsCumulativeProperty, value); }
        }

        #endregion // Properties

        #region Implementing PointCollectionAnimationBase

        protected override Freezable CreateInstanceCore()
        {
            return new PointCollectionAnimation();
        }

        /// <summary>
        /// Returns a point collection that is the component-wise interpolation between two PointCollections
        /// based on the elapsed time and EasingFunction
        /// </summary>
        /// <param name="animationClock">Provides information about the animation's progress</param>
        /// <returns></returns>
        protected override PointCollection GetCurrentValueCore(PointCollection defaultOriginValue,
                                                               PointCollection defaultDestinationValue,
                                                               AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
                return null; // The animationClock really oughtn't be null...

            double progress = EasingFunction == null 
                                  ? animationClock.CurrentProgress.Value
                                  : EasingFunction.Ease(animationClock.CurrentProgress.Value);

            PointCollection fromCollection = From ?? defaultOriginValue;
            PointCollection toCollection = new PointCollection();

            if (To != null)
            {
                foreach (Point point in To)
                    toCollection.Add(point);
            }
            else if (By != null)
            {
                for (int i = 0; 
                         i < Math.Min(fromCollection.Count, By.Count); 
                         i++)
                    toCollection.Add(new Point(fromCollection[i].X + By[i].X,
                                               fromCollection[i].Y + By[i].Y));
            }
            else
            {
                foreach (Point point in defaultDestinationValue)
                    toCollection.Add(point);
            }

            // Generate a new "frame" of the animation
            PointCollection swapCollection = new PointCollection();

            for (int i = 0;
                     i < Math.Min(fromCollection.Count, toCollection.Count);
                     i++)
            {
                double x = (1 - progress) * fromCollection[i].X + progress * toCollection[i].X;
                double y = (1 - progress) * fromCollection[i].Y + progress * toCollection[i].Y;

                swapCollection.Add(new Point(x, y));
            }

            if (IsAdditive &&
                From != null &&
                (To != null || By != null))
            {
                for (int i = 0;
                         i < Math.Min(swapCollection.Count, defaultOriginValue.Count);
                         i++)
                {
                    swapCollection[i] = new Point(swapCollection[i].X + defaultOriginValue[i].X,
                                                  swapCollection[i].Y + defaultOriginValue[i].Y);
                }
            }

            if (IsCumulative && animationClock.CurrentIteration != null)
            {
                int iter = animationClock.CurrentIteration.Value;
                for (int i = 0; i < swapCollection.Count; i++)
                {
                    swapCollection[i] = new Point(swapCollection[i].X + (iter - 1) * (toCollection[i].X - fromCollection[i].X),
                                                  swapCollection[i].Y + (iter - 1) * (toCollection[i].Y - fromCollection[i].Y));
                }
            }

            // Swap the new "frame" with the old
            PresentCollection = swapCollection;
            return PresentCollection;
        }

        #endregion // Implementing PointCollectionAnimationBase
    }
}
