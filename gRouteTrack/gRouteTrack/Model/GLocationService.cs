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
using System.Collections.Generic;

namespace gRouteTrack.Model
{
    /// <summary>
    /// This class encapsulates the work of the GeoCoordinateWatcher class.
    /// It has method's to start,stop,pause,resume gps sensor.
    /// </summary>
    public class GLocationService
    {
        private GeoPositionAccuracy defGeoPositionAccuracy = GeoPositionAccuracy.High;
        private GeoCoordinateWatcher geoWatcher;
        private gSystemSettings.GLocationServiceStatus _locationServiceStatus;

        private GPoint lastPoint;

        public delegate void NewGPoint(object o, GPointEventArgs e);

        /// <summary>
        /// This event is fired every time when new GPoiint is available.
        /// Note - this event is not fired for every coordiante point that GeoCoordinateWatcher registers as new point.
        /// </summary>
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
            get
            {
                return lastPoint;
            }
            set
            {
                lastPoint = value;
            }
        }
        #endregion

        #region PublicFuntions

        /// <summary>
        /// Tries to start monitoring gps coordinates. It tries to start GPS sensor.
        /// </summary>
        /// <returns>Returns Started if gps sernsor is succesfully started - otherwise return NotStarted</returns>
        public gSystemSettings.GLocationServiceStatus StartGLocationWithEvent()
        {
            _locationServiceStatus = StartGLocation();
            return this.LocationServiceStatus;
        }

        /// <summary>
        /// Tries to stop monitoring gps coordinates. It stop's the GPS sensor.
        /// </summary>
        /// <returns>Returns Stopped</returns>
        public gSystemSettings.GLocationServiceStatus StopGLocation()
        {
            _locationServiceStatus = StopGPSSensor(false);
            return this.LocationServiceStatus;
        }

        /// <summary>
        /// Tries to pause monitoring gps coordinates. It stop's the GPS sensor.
        /// </summary>
        /// <returns>Returns Paused</returns>
        public gSystemSettings.GLocationServiceStatus PauseGLocation()
        {
            _locationServiceStatus = StopGPSSensor(true);
            return this.LocationServiceStatus;
        }

        /// <summary>
        /// Tries to resume monitoring gps coordinates. It tries to start GPS sensor.
        /// </summary>
        /// <returns>Returns Started if gps sernsor is succesfully started - otherwise return NotStarted</returns>
        public gSystemSettings.GLocationServiceStatus ResumeGLocation()
        {
            _locationServiceStatus = StartGLocation();
            return this.LocationServiceStatus;
        }

        /// <summary>
        /// Return the current point that it's storred in GeoWatcher. 
        /// It doesn't mean that it's the current position - it's the last point that phone detected as new point.
        /// </summary>
        /// <returns></returns>
        public GPoint GetCurrentPoint()
        {
            return MakeGPointFromGeoCoordinatePostion(geoWatcher.Position);
        }


        public double CalculateTotalDistanceOnGPoints(List<GPoint> coordinates)
        {
            Double result = 0;

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                result += CalculateDistanceBetweenTwoGPoint(coordinates[i], coordinates[i + 1]);
            }

            return result;
        }

        public double CalculateDistanceBetweenTwoGPoint(GPoint point1, GPoint point2)
        {
            Double result = 0;
            GeoCoordinate p1 = new GeoCoordinate(point1.Latitude, point1.Longitude);
            GeoCoordinate p2 = new GeoCoordinate(point2.Latitude, point2.Longitude);
            result = p1.GetDistanceTo(p2);
            return result;
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
                //try to start with low accuracy)
                geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                haveLocaion = geoWatcher.TryStart(true, TimeSpan.FromMilliseconds(3000));
            }
            if (haveLocaion == false)
            {
                String messageToShow = "Can not start location services";
                if (geoWatcher.Status == GeoPositionStatus.Disabled)
                {
                    messageToShow += " GPS is disabled";
                }
                else
                {
                    if (geoWatcher.Permission != GeoPositionPermission.Granted)
                    {
                        messageToShow += "This application has no acces to GPS";
                    }
                }
                MessageBox.Show(messageToShow);//TO-DO Make Exception Here
                return gSystemSettings.GLocationServiceStatus.NotStarted;
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
                    Double distanceFromLastPoint = this.CalculateDistanceBetweenTwoGPoint(lastPoint, newPoint);
                    if (lastPoint.IsEqual(newPoint, true, gSystemSettings.ComparingPointSeconds))
                    {
                        return;//It's the same location  - this is not a new point
                    }

                    if (distanceFromLastPoint < gSystemSettings.DistanceInMeters)
                    {
                        return; //It's in the same spot
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
                currentPoint.Speed = Double.IsNaN(position.Location.Speed) ? 0 : position.Location.Speed;
                currentPoint.Altitude = Double.IsNaN(position.Location.Altitude) ? 0 : position.Location.Altitude;
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
