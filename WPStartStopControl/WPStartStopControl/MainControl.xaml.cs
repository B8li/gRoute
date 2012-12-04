using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPStartStopControl
{
    public partial class MainControl : UserControl
    {
        private double _spaceBeetweenSprites;
        private GLocationServiceStatus _status;

        public enum GLocationServiceStatus
        {
            NotStarted,
            Started,
            Paused,
            Stopped
        }

        public enum ControlClicked
        {
            None,
            Start,
            Stop
        }
        public delegate void ControlClick(object o, ControlClicked e);
        public event ControlClick OnControlClick;

        

        public MainControl()
        {
            InitializeComponent();
            this._status = GLocationServiceStatus.NotStarted;
            this._spaceBeetweenSprites = 10;
        }

        #region Properties

        public GLocationServiceStatus Status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
                ChangeStatus();  
            }
        }

        /// <summary>
        /// Set size(Width) of each Sprite control. 
        /// Actual size of the control would be (SetSize * 2) + SetSpaceBetween
        /// </summary>
        public double SetSize
        {
            set
            {
                double scaleRactor = this.Width / value;
                this.Width = value + this._spaceBeetweenSprites;
                this.Height = value / 2;
                this.LayoutRoot.Width = value + this._spaceBeetweenSprites;
                this.LayoutRoot.Height = value / 2;
                this.animatedCircle.ArcThickness = (value / 10) / 2;

                this.LayoutRoot.ColumnDefinitions[0].Width = new GridLength(value /2, GridUnitType.Pixel);
                this.LayoutRoot.ColumnDefinitions[1].Width = new GridLength(_spaceBeetweenSprites, GridUnitType.Pixel);
                this.LayoutRoot.ColumnDefinitions[2].Width = new GridLength(value/2, GridUnitType.Pixel);
                this.animatedCircleStop.ArcThickness = (value / 10) / 2;
            }
        }

        public double MyArcThickness
        {
            set
            {
                this.animatedCircle.ArcThickness = value;
            }
        }

        public static readonly DependencyProperty SetLineDependencyProperty = DependencyProperty.Register(
            "SetLine",
              typeof(double),
              typeof(MainControl),
              new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnSetLinePropertyChanged))
            );

        /// <summary>
        /// Sets EndAngle of the arc control(circle). It us used for animating the circle.
        /// </summary>
        public double SetLine
        {
            get
            {
                return this.animatedCircle.EndAngle;
            }
            set
            {
                this.animatedCircle.EndAngle = value;
                this.animatedCircle.StartAngle = value + 10;
            }
        }

        public static readonly DependencyProperty SetSpaceBetweenDependencyProperty = DependencyProperty.Register(
            "SetSpaceBetween",
            typeof(double),
            typeof(MainControl),
            new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnSetSpaceBetweenPropertyChanged))
            );

        /// <summary>
        /// Sets the space beetween start and stop sprite.MUST user this before SetSize otherwise it will not work good.
        /// </summary>
        public double SetSpaceBetween
        {
            get
            {
                return this._spaceBeetweenSprites;
            }
            set
            {
                this._spaceBeetweenSprites = value;
            }
        }

        #endregion

        #region Private Functions

        private void ChangeControlState(ISprite control, Microsoft.Expression.Shapes.Arc circle, Brush brush)
        {
            circle.Fill = brush;
            control.Fill = brush;
        }
        private void ShowStop(bool gray)
        {
            StopSprite.Visibility = System.Windows.Visibility.Visible;
            StartSprite.Visibility = System.Windows.Visibility.Collapsed;
            if (gray)
            {
                animatedCircle.Fill = DrawUtils.GetGrayBrush();
                StopSprite.Fill = DrawUtils.GetGrayBrush();
            }
            else
            {
                animatedCircle.Fill = DrawUtils.StopBrush;
                StopSprite.Fill = DrawUtils.StopBrush;
            }
        }

        
        private static void OnSetLinePropertyChanged(DependencyObject dependencyObject,DependencyPropertyChangedEventArgs e)
        {
            MainControl myUserControl = dependencyObject as MainControl;
            myUserControl.OnSetLinePropertyChanged(e);
        }
        private void OnSetLinePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.SetLine = (double)e.NewValue;
        }

        private static void OnSetSpaceBetweenPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            MainControl myUserControl = dependencyObject as MainControl;
            myUserControl.OnSetLinePropertyChanged(e);
        }
        private void OnSetSpaceBetweenPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.SetSpaceBetween = (double)e.NewValue;
        }

        private void ChangeStatus()
        {
            switch (this.Status)
            {
                case GLocationServiceStatus.NotStarted:
                    ChangeControlState(StartSprite, animatedCircle, DrawUtils.GetGrayBrush());
                    ChangeControlState(StopSprite, animatedCircleStop, DrawUtils.GetGrayBrush());
                    break;
                case GLocationServiceStatus.Started:
                    ChangeControlState(StartSprite, animatedCircle, DrawUtils.StartBrush);
                    ChangeControlState(StopSprite, animatedCircleStop, DrawUtils.StopBrush);
                    break;
                case GLocationServiceStatus.Paused:
                    ChangeControlState(StartSprite, animatedCircle, DrawUtils.GetGrayBrush());
                    ChangeControlState(StopSprite, animatedCircleStop, DrawUtils.StopBrush);
                    break;
                case GLocationServiceStatus.Stopped:
                    ChangeControlState(StartSprite, animatedCircle, DrawUtils.GetGrayBrush());
                    ChangeControlState(StopSprite, animatedCircleStop, DrawUtils.GetGrayBrush());
                    break;
                default:
                    break;
            }
        }

        private void Start_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnControlClick(sender,ControlClicked.Start);
        }

        private void Stop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnControlClick(sender, ControlClicked.Stop);
        }

        #endregion



    }
}