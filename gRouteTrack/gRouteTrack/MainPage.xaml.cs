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
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace gRouteTrack
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraTask ;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = App.gRouteTrackViewModel;

            //Always first set SetSpaceBetween if you want to work size 
            myCircleStart.SetSpaceBetween = 60;
            myCircleStart.SetSize = 200;
            cameraTask = new CameraCaptureTask();
            cameraTask.Completed += new EventHandler<PhotoResult>(cameraTask_Completed);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.SaveToIsolatedStorage();
            App.SaveSettings();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        private void gDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid gridData = (Grid)sender;
            gridData.Width = Application.Current.RootVisual.RenderSize.Width;
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
            try
            {
                WriteableBitmap bi = App.ReadImageFromIsolatedStorage("gRouteTemp.jpg");
                if (bi != null)
                {
                    this.TestImage.Source = bi;
                }
                Tasks.ComposeEmail("igor.bulovski@gmail.com", "[WP]", GPointConverter.FromGRouteToIsolatedStorageFormat(App.gRouteTrackViewModel.CurrentRoute), "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Delete_Icon_Click(object sender, EventArgs e)
        {
            gSystemSettings.GLocationServiceStatus oldStatus = App.gRouteTrackViewModel.LocationServiceStatus;
            myCircleStart.Status = (WPStartStopControl.MainControl.GLocationServiceStatus)App.gRouteTrackViewModel.ClickedDelete();
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(UserNameTextBox.Text.Trim()) == false && String.IsNullOrWhiteSpace(PasswordBox.Password) == false)
            {
                App.configurationViewModel.UserName = UserNameTextBox.Text.Trim();
                App.configurationViewModel.Password = PasswordBox.Password.Trim();
            }
            else
            {
                if (String.IsNullOrWhiteSpace(UserNameTextBox.Text.Trim()))
                {
                    UserNameTextBox.Focus();
                }
                else
                {
                    PasswordBox.Focus();
                }

            }
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            ((PivotItem)sender).DataContext = App.configurationViewModel;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPivot.SelectedIndex == 0)
            {
                //Settings Tabs
                ((PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content).ApplicationBar.IsVisible = false;
            }
            else
            {
                ((PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content).ApplicationBar.IsVisible = true;
            }
            

            //Folllowing settings are for optimising performance of the application
            if (MainPivot.SelectedIndex == 2)
            {
                //Charts
                speedChart.DataSource  = App.gRouteTrackViewModel.PointItems;
                altitudeChart.DataSource = App.gRouteTrackViewModel.PointItems;
            }
            else
            {
                //Deactive charts
                speedChart.DataSource = null;
                altitudeChart.DataSource = null;
            }

            if (MainPivot.SelectedIndex == 3)
            {
                //Route Data
                pointsListBox.ItemsSource = App.gRouteTrackViewModel.PointItems;
            }
            else
            {
                //Deactive coordinates data
                pointsListBox.ItemsSource = null;
            }

            if (MainPivot.SelectedIndex == 4)
            {
                RoutesListBox.ItemsSource = App.gRouteTrackViewModel.RoutesOnPhone;
            }
            else
            {
                RoutesListBox.ItemsSource = null;
            }
        }

        private void TakePicture_Click(object sender, EventArgs e)
        {
            cameraTask.Show();
        }

        private void cameraTask_Completed(object sender, PhotoResult e)
        {
            try
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    //String tempJPEG = "gRouteTrack-" + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + ".jpg";
                    String tempJPEG = "gRouteTemp.jpg";
                    using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fileStream = isf.CreateFile(tempJPEG))
                        {
                            StreamResourceInfo sri = null;
                            Uri uri = new Uri(e.OriginalFileName, UriKind.Relative);
                            sri = Application.GetResourceStream(uri);

                            BitmapImage bitmap = new BitmapImage();
                            bitmap.SetSource(sri.Stream);
                            WriteableBitmap wb = new WriteableBitmap(bitmap);


                            Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}