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
	public partial class StartSpriteControl : UserControl
	{
		public StartSpriteControl()
		{
			// Required to initialize variables
			InitializeComponent();
			this.SizeChanged+=new System.Windows.SizeChangedEventHandler(StartSpriteControl_SizeChanged);
		}

        public Brush Fill
        {
            get
            {
                return this.Sprite1.Fill;
            }
        }

        private void StartSpriteControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            TransformGroup sprite1Transform = new TransformGroup();
            DrawUtils.DrawSprite(Sprite1, 0.15, 0.28, e.NewSize.Width, 35, sprite1Transform);


            TranslateTransform translate = new TranslateTransform();
            translate.X = (Sprite1.ActualWidth - Sprite1.Margin.Left - Sprite1.Margin.Right) /1.17;
            sprite1Transform.Children.Add(translate);

            Sprite1.RenderTransform = sprite1Transform;


            TransformGroup sprite2Transform = new TransformGroup();
            DrawUtils.DrawSprite(Sprite2, 0.14, 0.3, e.NewSize.Width, 125, sprite2Transform);
            TranslateTransform translate2 = new TranslateTransform();
            translate2.X = -(Sprite2.ActualWidth - Sprite2.Margin.Left - Sprite2.Margin.Right) / 1.1;
            translate2.Y =(Sprite2.ActualHeight - Sprite2.Margin.Top - Sprite2.Margin.Bottom) / 2.8;
            sprite2Transform.Children.Add(translate2);

            Sprite2.RenderTransform = sprite2Transform;
            //Sprite2.Visibility = System.Windows.Visibility.Collapsed;
        }
	}
}