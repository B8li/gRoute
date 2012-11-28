using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace WinPhoneApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        GLocation gLocationSystem;
        ObservableCollection<GPoint> gPoints;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            /*
            Adder add = new Adder();
            ContentPanel.DataContext = add;
            */

            gPoints = new ObservableCollection<GPoint>();
            gLocationSystem = new GLocation(System.Device.Location.GeoPositionAccuracy.High);
            gLocationSystem.OnNewGPoint += new GLocation.NewGPoint(gLocationSystem_OnNewGPoint);

            //PointsListBox.ItemsSource = gPoints;
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            GPoint currPoint = gLocationSystem.GetCurrentPoint();
            if (currPoint.IsValid)
            {
                gPoints.Add(new GPoint(currPoint.Longitude, currPoint.Latitude, gPoints.Count + 1));
            }
           
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartButton.Tag == null || (StartButton.Tag != null && (bool)StartButton.Tag == false))
            {
                gLocationSystem.StartGLocationWithEvent();
                StartButton.Tag = true;
                StartButton.Content = "Stop";
            }
            else
            {
                gLocationSystem.StopGLocation();
                StartButton.Tag = false;
                StartButton.Content = "Start";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //Just to invoke test
            Test.TestGPoint.TestIsEqual();


            gPoints.Clear();
            PointsConsole.Text = "";
            CurrentPointTextBlock.Text = "";
        }
        private void CurrentPointButton_Click(object sender, RoutedEventArgs e)
        {
            GPoint currPoint = gLocationSystem.GetCurrentPoint();
            if (currPoint.IsValid)
            {
                TopValueTextBox.Text = currPoint.Longitude.ToString();
                BottomValueTextBox.Text = currPoint.Latitude.ToString();

                CurrentPointTextBlock.Text = currPoint.ToString();
            }
           
        }
        void gLocationSystem_OnNewGPoint(object o, GPointEventArgs e)
        {
            if (e.Point.IsValid)
            {
                gPoints.Add(new GPoint(e.Point.Longitude, e.Point.Latitude, gPoints.Count + 1));

                PointsConsole.Text += Environment.NewLine + e.Point.ToString();
                CurrentPointTextBlock.Text = new GPoint(e.Point.Longitude, e.Point.Latitude, 0).ToString();
                //ApplicationTitle.Text = e.Point.
            }
        }

        private void ContentPanel_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            TextBox temp = (TextBox)sender;
            if (e.Action == ValidationErrorEventAction.Added)
            {
                temp.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                temp.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GPointConverter.FromGpointToWP7Simulator(gPoints.ToList<GPoint>()));
            Tasks.ComposeEmail("igor.bulovski@gmail.com", "[WP]Data " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), PointsConsole.Text, ""); 
        }
    }
}