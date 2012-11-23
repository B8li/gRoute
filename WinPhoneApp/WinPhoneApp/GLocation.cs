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

namespace WinPhoneApp
{
    public class GLocation
    {
        GeoPositionAccuracy defGeoPositionAccuracy = GeoPositionAccuracy.High;
        GeoCoordinateWatcher geoWatcher;

        GPoint lastPoint;

        public delegate void NewGPoint(object o, GPointEventArgs e);

        public event NewGPoint OnNewGPoint;

        public GLocation()
        {
            geoWatcher = new GeoCoordinateWatcher(defGeoPositionAccuracy);
        }
        public GLocation(GeoPositionAccuracy geoPositionAccuracy)
        {
            geoWatcher = new GeoCoordinateWatcher(geoPositionAccuracy);
        }

        private void StartGLocation()
        {
            if (geoWatcher == null)
            {
                geoWatcher = new GeoCoordinateWatcher(defGeoPositionAccuracy);
            }
            geoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_PositionChanged);
            geoWatcher.MovementThreshold = 0;
            bool haveLocaion = geoWatcher.TryStart(true, TimeSpan.FromMilliseconds(3000));
            if (haveLocaion == false)
            {
                MessageBox.Show("Can not start location services");//TO-DO Make Exception Here
            }

            if (geoWatcher.Status == GeoPositionStatus.Disabled)
            {
                MessageBox.Show("GPS is disabled");//TO-DO Make Exception Here
            }
            else
            {
                if (geoWatcher.Permission != GeoPositionPermission.Granted)
                {
                    MessageBox.Show("This application has no acces to GPS");//TO-DO Make Exception Here
                }
            }
        }

        public void StartGLocationWithEvent()
        {
            StartGLocation();
        }

        public void StopGLocation()
        {
            if (geoWatcher != null)
            {
                geoWatcher.Stop();
            }
        }
        void geoWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown == true)
            {
                OnNewGPoint(this, new GPointEventArgs(new GPoint(-999, -999, -999)));
            }
            else
            {
                GPoint newPoint = MakeGPointFromGeoCoordinate(e);
                if (lastPoint != null)
                {
                    if (lastPoint.IsEqual(newPoint))
                    {
                        return;//It's the same location  - this is not a new point
                    }
                }
                lastPoint = newPoint;
                OnNewGPoint(this, new GPointEventArgs(newPoint));
            }
            System.Diagnostics.Debug.WriteLine(e.Position.Location.Longitude + " " + e.Position.Location.Latitude + " H:" + e.Position.Location.HorizontalAccuracy.ToString() + " V:" + e.Position.Location.VerticalAccuracy.ToString());

        }


        public GPoint GetCurrentPoint()
        {
            GPoint currentPoint = new GPoint();

            if (geoWatcher.Position.Location.IsUnknown == false)
            {
                currentPoint.Long = geoWatcher.Position.Location.Longitude;
                currentPoint.Lat = geoWatcher.Position.Location.Latitude;
            }else{
                currentPoint.Long = -999;
                currentPoint.Lat = -999;
            }
            return currentPoint;
        }

        private GPoint MakeGPointFromGeoCoordinate(GeoPositionChangedEventArgs<GeoCoordinate> coordinate)
        {
            GPoint currentPoint = new GPoint(coordinate.Position.Location.Longitude,coordinate.Position.Location.Latitude);

            return currentPoint;
        }
    }
}
