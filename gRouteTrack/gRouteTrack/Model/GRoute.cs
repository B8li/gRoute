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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace gRouteTrack.Model
{
    public class GRoute
    {
        #region PrivateFilds

        private DateTime _startTime;
        private DateTime _endTime;
        private TimeSpan _fullTime;
        private ObservableCollection<GPoint> _coordinates;
        private bool _uploadCheck;
        private double _totalDistance;
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. It's CALLED from other constructors. 
        /// Default values for GRoute fileds are defined here.
        /// </summary>
        public GRoute()
        {
            this._startTime = gSystemSettings.DefaultDate;
            this._endTime = gSystemSettings.DefaultDate;
            this._fullTime = TimeSpan.FromSeconds(0);
            this._coordinates = new ObservableCollection<GPoint>();
            this._uploadCheck = false;
            this._totalDistance = 0;
        }

        /// <summary>
        /// Creates new GRoute object with given list of coordinates.
        /// First call's the default constroctor.
        /// </summary>
        /// <param name="coordinates">List of GPoint elements that represent one route</param>
        public GRoute(List<GPoint> coordinates)
            : this()
        {
            this._coordinates = new ObservableCollection<GPoint>(coordinates);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set route StartTime - when the route is first time started.
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }

        /// <summary>
        /// Get or set route EndTime - when the route is stopped.
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
            }
        }

        /// <summary>
        /// Get or set full elasped time on this route. 
        /// This is different from EndTime - StartTime beacause paused are not iclueded in full time.
        /// </summary>
        public TimeSpan FullTime
        {
            get
            {
                return _fullTime;
            }
            set
            {
                _fullTime = value;
            }
        }

        /// <summary>
        /// Get or set coordinates as List<GPoint>
        /// </summary>
        public List<GPoint> CoordinatesList
        {
            get
            {
                return new List<GPoint>(_coordinates);
            }
            set
            {
                _coordinates = new ObservableCollection<GPoint>(value);
            }
        }


        /// <summary>
        /// Get or set route coordinates as ObservableCollection<GPoint>
        /// </summary>
        public ObservableCollection<GPoint> Coordinates
        {
            get
            {
                return _coordinates;
            }
            set
            {
                _coordinates = value;
            }
        }

        /// <summary>
        /// Returns true if route has valid EndTime value.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return !(this.EndTime == gSystemSettings.DefaultDate);
            }
        }

        /// <summary>
        /// Get the last entered point from route.
        /// </summary>
        public GPoint LastPoint
        {
            get
            {
                GPoint lastPoint;
                if (this.Coordinates.Count > 0)
                    lastPoint = this.Coordinates[this.Coordinates.Count - 1];
                else
                {
                    lastPoint = new GPoint();
                }
                return lastPoint;
            }
        }

        /// <summary>
        /// Get or set UploadCheck flag. True - user once marked that did not want to upload this route at that moment.
        /// </summary>
        public bool UploadCheck
        {
            get
            {
                return this._uploadCheck;
            }
            set
            {
                this._uploadCheck = value;
            }
        }

        /// <summary>
        /// Get or set route total distance
        /// </summary>
        public double TotalDistance
        {
            get 
            {
                return this._totalDistance;
            }
            set
            {
                this._totalDistance = value;
            }
        }
        #endregion

        #region PublicFunctions

        /// <summary>
        /// Add new point to the route.
        /// </summary>
        /// <param name="newPoint">GPoint object representing new point.</param>
        /// <param name="resolvePointNumber">If True then newPoint.PointNumber is resolved from current route.</param>
        public void AddNewGPoint(GPoint newPoint, bool resolvePointNumber)
        {
            if (resolvePointNumber == true)
            {
                newPoint.PointNumber = this.Coordinates.Count;
            }
            this.Coordinates.Add(newPoint);
        }

        /// <summary>
        /// Stop's the current route. EndTime is setted to Now
        /// </summary>
        public void StopRoute()
        {
            this.EndTime = DateTime.Now;
        }
        #endregion

    }
}
