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
using gRouteTrack.Model;

namespace gRouteTrack
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = App.gRouteTrackViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            //Always first set SetSpaceBetween if you want to work
            myCircleStart.SetSpaceBetween = 60;
            myCircleStart.SetSize = 200;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.SaveToIsolatedStorage();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        private void gDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid gridData = (Grid)sender;
            //gridData.Width = Application.Current.RootVisual.RenderSize.Width;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            /*
            //TO-DO remove this lines - just for test
            App.gRouteTrackViewModel.CurrentRoute.AddNewGPoint(new Model.GPoint(42.25632, 40.326, 1, new DateTimeOffset(DateTime.Now)),true);
            App.gRouteTrackViewModel.CurrentRoute.AddNewGPoint(new Model.GPoint(43.25632, 41.305, 2, new DateTimeOffset(DateTime.Now)), true);
              */
        }

        private void myCircleStart_Loaded(object sender, RoutedEventArgs e)
        {
            myCircleStart.OnControlClick += new WPStartStopControl.MainControl.ControlClick(myCircleStart_OnControlClick);
            myCircleStart.Status = WPStartStopControl.MainControl.GLocationServiceStatus.NotStarted;
            myCircleStart_OnControlClick(null, WPStartStopControl.MainControl.ControlClicked.Stop);
        }


        void myCircleStart_OnControlClick(object sender, WPStartStopControl.MainControl.ControlClicked e)
        {
            try
            {
                gSystemSettings.GLocationServiceStatus oldStatus = App.gRouteTrackViewModel.LocationServiceStatus;
                if (e == WPStartStopControl.MainControl.ControlClicked.Start)
                {
                    myCircleStart.Status = (WPStartStopControl.MainControl.GLocationServiceStatus)App.gRouteTrackViewModel.ClickedStart();
                }
                else
                {
                    myCircleStart.Status = (WPStartStopControl.MainControl.GLocationServiceStatus)App.gRouteTrackViewModel.ClickedStop();
                }
                ChangeMyControlStatus(oldStatus);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Email_Icon_Click(object sender, EventArgs e)
        {
            Tasks.ComposeEmail("igor.bulovski@gmail.com", "[WP]", GPointConverter.FromGRouteToIsolatedStorageFormat(App.gRouteTrackViewModel.CurrentRoute), "");
        }

        private void Delete_Icon_Click(object sender, EventArgs e)
        {
            gSystemSettings.GLocationServiceStatus oldStatus = App.gRouteTrackViewModel.LocationServiceStatus;
            myCircleStart.Status = (WPStartStopControl.MainControl.GLocationServiceStatus) App.gRouteTrackViewModel.ClickedDelete();
            ChangeMyControlStatus(oldStatus);
        }

        private void ChangeMyControlStatus(gSystemSettings.GLocationServiceStatus oldStatus)
        {
            switch (myCircleStart.Status)
            {
                case WPStartStopControl.MainControl.GLocationServiceStatus.Started:
                    if (oldStatus == gSystemSettings.GLocationServiceStatus.Paused)
                    {
                        animateMyCircle.Resume();
                    }
                    else
                    {
                        animateMyCircle.Begin();
                    }
                    break;
                case WPStartStopControl.MainControl.GLocationServiceStatus.Paused:
                    animateMyCircle.Pause();
                    break;
                case WPStartStopControl.MainControl.GLocationServiceStatus.Stopped:
                case WPStartStopControl.MainControl.GLocationServiceStatus.NotStarted:
                    animateMyCircle.Pause();
                    break;
                default:
                    break;
            }
        }
    }
}