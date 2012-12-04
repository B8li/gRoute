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
using System.Device.Location;

namespace gRouteTrack.Model
{
    public class GLocationService
    {
        private GeoPositionAccuracy defGeoPositionAccuracy = GeoPositionAccuracy.High;
        private GeoCoordinateWatcher geoWatcher;
        private gSystemSettings.GLocationServiceStatus _locationServiceStatus;

        private GPoint lastPoint;

        public delegate void NewGPoint(object o, GPointEventArgs e);

        public event NewGPoint OnNewGPoint;

        #region Constrcotrs

        public GLocationService()
        {
            geoWatcher = new GeoCoordinateWatcher(defGeoPositionAccuracy);
            _locationServiceStatus = gSystemSettings.GLocationServiceStatus.NotStarted;
        }
        public GLocationService(GeoPositionAccuracy geoPositionAccuracy)
        {
            geoWatcher = new GeoCoordinateWatcher(geoPositionAccuracy);
            _locationServiceStatus = gSystemSettings.GLocationServiceStatus.NotStarted;
        }
        #endregion

        #region Properties

        public gSystemSettings.GLocationServiceStatus LocationServiceStatus
        {
            get
            {
                return this._locationServiceStatus;
            }
            set
            {
                this._locationServiceStatus = value;
            }
        }

        public GPoint LastPoint
        {
            get { return lastPoint; }
            set { lastPoint = value; }
        }
        #endregion

        #region PublicFuntions

        public gSystemSettings.GLocationServiceStatus StartGLocationWithEvent()
        {
            _locationServiceStatus = StartGLocation();
            return this.LocationServiceStatus;
        }

        public gSystemSettings.GLocationServiceStatus StopGLocation()
        {
            _locationServiceStatus = StopGPSSensor(false);
            return this.LocationServiceStatus;
        }

        public gSystemSettings.GLocationServiceStatus PauseGLocation()
        {
            _locationServiceStatus = StopGPSSensor(true);
            return this.LocationServiceStatus;
        }

        public gSystemSettings.GLocationServiceStatus ResumeGLocation()
        {
            _locationServiceStatus = StartGLocation();
            return this.LocationServiceStatus;
        }

        public GPoint GetCurrentPoint()
        {
            return MakeGPointFromGeoCoordinatePostion(geoWatcher.Position);
        }

        #endregion

        #region PrivateFuntions

        private gSystemSettings.GLocationServiceStatus StartGLocation()
        {
            if (geoWatcher == null)
            {
                geoWatcher = new GeoCoordinateWatcher(defGeoPositionAccuracy);
            }
            geoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_PositionChanged);

            bool haveLocaion = geoWatcher.TryStart(true, TimeSpan.FromMilliseconds(3000));
            if (haveLocaion == false)
            {
                MessageBox.Show("Can not start location services");//TO-DO Make Exception Here
                return gSystemSettings.GLocationServiceStatus.NotStarted;
            }
            else
            {
                if (geoWatcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("GPS is disabled");//TO-DO Make Exception Here
                    return gSystemSettings.GLocationServiceStatus.NotStarted;
                }
                else
                {
                    if (geoWatcher.Permission != GeoPositionPermission.Granted)
                    {
                        MessageBox.Show("This application has no acces to GPS");//TO-DO Make Exception Here
                        return gSystemSettings.GLocationServiceStatus.NotStarted;
                    }
                }
            }

            return gSystemSettings.GLocationServiceStatus.Started;
        }

        private gSystemSettings.GLocationServiceStatus StopGPSSensor(bool isPaused)
        {
            if (geoWatcher != null)
            {
                geoWatcher.Stop();
                geoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_PositionChanged);
            }

            if (isPaused == true)
            {
                return gSystemSettings.GLocationServiceStatus.Paused;
            }
            else
            {
                return gSystemSettings.GLocationServiceStatus.Stopped;
            }
        }
        private void geoWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown == true)
            {
                //OnNewGPoint(this, new GPointEventArgs(new GPoint(0, 0, 0)));
            }
            else
            {
                GPoint newPoint = MakeGPointFromGeoCoordinatePostion(e.Position);
                if (lastPoint != null)
                {
                    if (lastPoint.IsEqual(newPoint, true, gSystemSettings.ComparingPointSeconds))
                    {
                        return;//It's the same location  - this is not a new point
                    }
                }
                lastPoint = newPoint;
                OnNewGPoint(this, new GPointEventArgs(newPoint));
            }
            System.Diagnostics.Debug.WriteLine(e.Position.Location.Longitude + " " + e.Position.Location.Latitude + " H:" + e.Position.Location.HorizontalAccuracy.ToString() + " V:" + e.Position.Location.VerticalAccuracy.ToString());

        }

        private GPoint MakeGPointFromGeoCoordinatePostion(GeoPosition<GeoCoordinate> position)
        {
            GPoint currentPoint = new GPoint();

            if (position.Location.IsUnknown == false)
            {
                currentPoint.Longitude = position.Location.Longitude;
                currentPoint.Latitude = position.Location.Latitude;
                currentPoint.Speed = position.Location.Speed;
                currentPoint.Altitude = position.Location.Altitude;
                currentPoint.TimeTaken = position.Timestamp;
            }
            else
            {
                currentPoint.Longitude = 0;
                currentPoint.Latitude = 0;
            }

            System.Diagnostics.Debug.WriteLine(position.Timestamp.ToString());
            return currentPoint;
        }

        #endregion



    }
}
