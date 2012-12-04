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

        public bool IsFinished
        {
            get
            {
                return !(this.EndTime == gSystemSettings.DefaultDate);
            }
        }

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
        #endregion

        #region PublicFunctions

        public void AddNewGPoint(GPoint newPoint, bool resolvePointNumber)
        {
            if (resolvePointNumber == true)
            {
                newPoint.PointNumber = this.Coordinates.Count;
            }
            this.Coordinates.Add(newPoint);
        }

        public void StopRoute()
        {
            this.EndTime = DateTime.Now;
        }
        #endregion

    }
}
