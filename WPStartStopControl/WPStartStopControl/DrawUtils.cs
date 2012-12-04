using System;
using System.Net;
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
    public class DrawUtils
    {
        public static void DrawSprite(Rectangle sprite,double sizeWPercent,double sizeHPercent, double newSize, double rotateAngel,TransformGroup groupTransform)
        {
            Double leftMargin = (newSize - (newSize * sizeWPercent)) / 2;
            Double topMargin = newSize * sizeHPercent;

            sprite.Margin = new Thickness(leftMargin, topMargin, leftMargin, topMargin);

            if (rotateAngel != 0)
            {
                RotateTransform rotate = new RotateTransform();
                rotate.Angle = rotateAngel;
                rotate.CenterX = (sprite.ActualWidth - sprite.Margin.Left - sprite.Margin.Right) / 2;
                rotate.CenterY = (sprite.ActualHeight - sprite.Margin.Top - sprite.Margin.Bottom) / 2;

                if (groupTransform == null)
                {
                    groupTransform = new TransformGroup();
                }
                groupTransform.Children.Add(rotate);
            }
        }
        public static Brush GetGrayBrush()
        {
           return new SolidColorBrush(Colors.LightGray);
           
        }

        public static Brush StartBrush;      
        public static Brush StopBrush;
    }
}
