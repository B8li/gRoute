﻿using System;
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
    public partial class StopSpriteControl : UserControl,ISprite 
    {
        public StopSpriteControl()
        {
            // Required to initialize variables
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(StopSpriteControl_SizeChanged);
            this.Loaded += new RoutedEventHandler(StopSpriteControl_Loaded);
        }

        void StopSpriteControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawUtils.StopBrush = this.StopSprite1.Fill;
        }

        void StopSpriteControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TransformGroup sprite1Transform = new TransformGroup();
            TransformGroup sprite2Transform = new TransformGroup();
            DrawUtils.DrawSprite(StopSprite1, 0.2,0.2, e.NewSize.Width, 45, sprite1Transform);
            DrawUtils.DrawSprite(StopSprite2, 0.2,0.2, e.NewSize.Width, -45, sprite2Transform);
            StopSprite1.RenderTransform = sprite1Transform;
            StopSprite2.RenderTransform = sprite2Transform;

        }

        public Brush Fill
        {
            get
            {
                return this.StopSprite1.Fill;
            }
            set
            {
                this.StopSprite1.Fill = value;
                this.StopSprite2.Fill = value;
            }
        }


        #region ISprite Members


        public Visibility ControlVisibility()
        {
            return this.Visibility;
        }

        #endregion
    }
}