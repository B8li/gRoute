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
using gRouteTrack.Model;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
namespace gRouteTrack.ViewModels
{
    public class gSystemViewModel : BaseNotify
    {
        #region PrivateFields

        private GLocationService _locationService;
        private GRoute _currentRoute;
        private gSystemSettings.GLocationServiceStatus _cuurentStatus;

        private DispatcherTimer _elasptedTimeTimer;
        private ObservableCollection<GRoute> _routesOnPhone;

        #endregion

        #region Constructors

        public gSystemViewModel()
        {
            this._elasptedTimeTimer = new DispatcherTimer();
            this._elasptedTimeTimer.Interval = TimeSpan.FromSeconds(1);
            this._elasptedTimeTimer.Tick += new EventHandler(_elasptedTimeTimer_Tick);

            this._locationService = new GLocationService();
            this._currentRoute = new GRoute();
            this._locationService.OnNewGPoint += new GLocationService.NewGPoint(_locationService_OnNewGPoint);
            this._cuurentStatus = this._locationService.LocationServiceStatus;
            this._routesOnPhone = new ObservableCollection<GRoute>();
        }


        #endregion

        #region Properties

        public ObservableCollection<GPoint> PointItems
        {
            get
            {
                return this._currentRoute.Coordinates;
            }
        }

        public ObservableCollection<GRoute> RoutesOnPhone
        {
            get
            {
                return this._routesOnPhone;
            }
        }
        public TimeSpan FullTime
        {
            get
            {
                return this._currentRoute.FullTime;
            }
        }

        public TimeSpan ElaspedTime
        {
            get
            {
                if (this._currentRoute.StartTime != gSystemSettings.DefaultDate)
                {
                    return (DateTime.Now - this._currentRoute.StartTime);
                }
                else
                {
                    return new TimeSpan();
                }
            }
        }

        public GPoint CurrentPoint
        {
            get
            {
                return this._currentRoute.LastPoint;
            }
        }

        public gSystemSettings.GLocationServiceStatus LocationServiceStatus
        {
            get
            {
                return this._cuurentStatus;
            }
            set
            {
                if (value != this._cuurentStatus)
                {
                    this._cuurentStatus = value;
                    RaisePropertyChanged("LocationServiceStatus");
                }
            }
        }

        public GRoute CurrentRoute
        {
            get
            {
                return this._currentRoute;
            }
            set
            {
                this._currentRoute = value;
            }
        }

        public double Latitude
        {
            get
            {
                return this.CurrentPoint.Latitude;
            }
        }

        public double Longitude
        {
            get
            {
                return this.CurrentPoint.Longitude;
            }
        }

        public double Speed
        {
            get
            {
                return this.CurrentPoint.Speed;
            }
        }

        public double Altitude
        {
            get
            {
                return this.CurrentPoint.Altitude;
            }
        }
        #endregion

        #region PublicFunctions

        public void Start()
        {
            if (this.LocationServiceStatus == gSystemSettings.GLocationServiceStatus.NotStarted)
            {
                this._currentRoute.StartTime = DateTime.Now;
            }
            if (this._locationService != null)
            {
                this.LocationServiceStatus = this._locationService.StartGLocationWithEvent();
            }
            if (this.LocationServiceStatus == gSystemSettings.GLocationServiceStatus.Started)
            {
                this._elasptedTimeTimer.Start();
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        public void Stop()
        {
            if (this._locationService != null)
            {
                this.LocationServiceStatus = this._locationService.StopGLocation();
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            }
        }

        public void Pause()
        {
            if (this._locationService != null)
            {
                this.LocationServiceStatus = this._locationService.PauseGLocation();
            }
        }

        public void Resume()
        {
            if (this._locationService != null)
            {
                this.LocationServiceStatus = this._locationService.StartGLocationWithEvent();
            }
        }

        public gSystemSettings.GLocationServiceStatus ClickedStart()
        {
            if (this.CurrentRoute.IsFinished)
            {
                ResolveFinifshedRoute();
                return gSystemSettings.GLocationServiceStatus.NotStarted;
            }
            gSystemSettings.GLocationServiceStatus newStatus = gSystemSettings.GLocationServiceStatus.Stopped;
            switch (this.LocationServiceStatus)
            {
                case gSystemSettings.GLocationServiceStatus.NotStarted:
                    this.Start();
                    newStatus = gSystemSettings.GLocationServiceStatus.Started;
                    break;
                case gSystemSettings.GLocationServiceStatus.Started:
                    this.Pause();
                    newStatus = gSystemSettings.GLocationServiceStatus.Paused;
                    break;
                case gSystemSettings.GLocationServiceStatus.Paused:
                    this.Start();
                    newStatus = gSystemSettings.GLocationServiceStatus.Started;
                    break;
                default:
                    newStatus = gSystemSettings.GLocationServiceStatus.Stopped;
                    break;
            }

            //Just for test 
            //newStatus = gSystemSettings.GLocationServiceStatus.NotStarted;
            return newStatus;
        }

        public gSystemSettings.GLocationServiceStatus ClickedStop()
        {
            gSystemSettings.GLocationServiceStatus newStatus = gSystemSettings.GLocationServiceStatus.Stopped;
            switch (this.LocationServiceStatus)
            {
                case gSystemSettings.GLocationServiceStatus.NotStarted:
                    newStatus = gSystemSettings.GLocationServiceStatus.NotStarted;
                    break;
                case gSystemSettings.GLocationServiceStatus.Started:
                    this.Pause();
                    newStatus = gSystemSettings.GLocationServiceStatus.Paused;
                    break;
                case gSystemSettings.GLocationServiceStatus.Paused:
                    this.Stop();
                    this.CurrentRoute.StopRoute();
                    ResolveFinifshedRoute();
                    newStatus = gSystemSettings.GLocationServiceStatus.Stopped;
                    break;
                default:
                    newStatus = gSystemSettings.GLocationServiceStatus.Stopped;
                    break;
            }

            return newStatus;
        }

        public gSystemSettings.GLocationServiceStatus ClickedDelete()
        {
            switch (LocationServiceStatus)
            {
                case gSystemSettings.GLocationServiceStatus.NotStarted:
                case gSystemSettings.GLocationServiceStatus.Stopped:
                    if (MessageBox.Show("Do you want to delete current route?", "Route is stopped.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteCurrentRoute();
                    }
                    break;
                case gSystemSettings.GLocationServiceStatus.Paused:
                case gSystemSettings.GLocationServiceStatus.Started:
                    if (MessageBox.Show("Do you want to stop and delete current route?", "Route is active.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteCurrentRoute();
                    }
                    break;
                default:
                    break;
            }

            return this.LocationServiceStatus;
        }
        public void ResolveFinifshedRoute()
        {
            MessageBoxResult result = MessageBoxResult.Cancel;
            if (this.CurrentRoute.UploadCheck == false)
            {
                result = MessageBox.Show("Do you want to upload current route?", "Route is finished.", MessageBoxButton.OKCancel);
                this.CurrentRoute.UploadCheck = true;
            }
            else
            {
                MessageBox.Show("", "Route is finished.", MessageBoxButton.OK);
                result = MessageBoxResult.Cancel;
            }

            if (result == MessageBoxResult.OK)
            {
                //TO-DO Upload route here
            }
            else
            {
                SaveCurrentRoute();
            }

            DeleteCurrentRoute();
        }
        #endregion

        #region PrivateFunctions
        private void RefreshRouteProperties()
        {
            RaisePropertyChanged("Latitude");
            RaisePropertyChanged("Longitude");
            RaisePropertyChanged("Speed");
            RaisePropertyChanged("SpeedKM");
            RaisePropertyChanged("Altitude");
        }
        private void _locationService_OnNewGPoint(object o, GPointEventArgs e)
        {
            if (e.Point.IsValid)
            {
                this.CurrentRoute.AddNewGPoint(e.Point, true);
                RefreshRouteProperties();
            }
        }

        private void _elasptedTimeTimer_Tick(object sender, EventArgs e)
        {
            if (this.LocationServiceStatus == gSystemSettings.GLocationServiceStatus.Started)
            {
                this._currentRoute.FullTime = this._currentRoute.FullTime.Add(TimeSpan.FromSeconds(1));
                RaisePropertyChanged("FullTime");
            }
            else
            {
                this._elasptedTimeTimer.Stop();
            }
        }

        private void DeleteCurrentRoute()
        {
            this.Stop();// Just to be sure that GPS Sensor  is stopped
            this._currentRoute = new GRoute();
            this.LocationServiceStatus = gSystemSettings.GLocationServiceStatus.NotStarted;
            RefreshRouteProperties();
            RaisePropertyChanged("PointItems"); //Used to refresh data grid
        }

        private void SaveCurrentRoute()
        {
            this.Stop(); // Just to be sure that GPS Sensor  is stopped
            this.RoutesOnPhone.Add(this.CurrentRoute);
            RaisePropertyChanged("RoutesOnPhone");
        }
        #endregion
    }
}
