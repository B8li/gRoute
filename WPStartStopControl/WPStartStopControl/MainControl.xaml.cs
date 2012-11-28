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
        public MainControl()
        {
            // Required to initialize variables
            InitializeComponent();
            Started = false;
        }

        public bool Started { get; set; }
        public double SetSize
        {
            set
            {
				double scaleRactor = this.Width / value;
                this.Width = value;
                this.Height = value;
                this.LayoutRoot.Width = value;
                this.LayoutRoot.Height = value;

                this.animatedCircle.ArcThickness = value / 10;

                //this.StopSprite.Margin = new Thickness(scaleRactor);
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

        private static void OnSetLinePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MainControl myUserControl = dependencyObject as MainControl;
            myUserControl.OnSetLinePropertyChanged(e);
        }
        private void OnSetLinePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.SetLine = (double)e.NewValue;
        }
        public double SetLine
        {
            set
            {
                this.animatedCircle.EndAngle = value;
                this.animatedCircle.StartAngle = value + 10;
            }

            get
            {
                return this.animatedCircle.EndAngle;
            }
        }

        public void ShowStart()
        {
            StopSprite.Visibility = System.Windows.Visibility.Collapsed;
            StartSprite.Visibility = System.Windows.Visibility.Visible;
            animatedCircle.Fill = StartSprite.Fill;
            this.Started = true;
        }
        public void ShowStop()
        {
            StopSprite.Visibility = System.Windows.Visibility.Visible;
            StartSprite.Visibility = System.Windows.Visibility.Collapsed;
            animatedCircle.Fill = StopSprite.Fill;
            this.Started = false;
        }
    }
}