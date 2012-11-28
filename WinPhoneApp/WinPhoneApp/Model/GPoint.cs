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
namespace WinPhoneApp
{
    /// <summary>
    /// Represent one point with GPS data. 
    /// Extends GeoCoordinate and includes custom fields and lot of function.
    /// </summary>
    public class GPoint : GeoCoordinate 
    {
        #region PrivateMembers

        private DateTimeOffset _pointTimeTaken;
        private int _pointNumber;
        private String formatStr = "0.########";

        private List<String> _isolatedStoragePicturePaths;

        #endregion

        #region Constructors

        public GPoint():base() { }

        public GPoint(int pointNumber):this()
        {
            this.Longitude = 0;
            this.Latitude = 0;
            this._pointNumber = pointNumber;
            this._pointTimeTaken = new DateTimeOffset(DateTime.Now);
            this._isolatedStoragePicturePaths = new List<string>();
        }
        public GPoint(double longi, double lat)
            : this(0)
        {
            this.Longitude = longi;
            this.Latitude = lat;
        }

        public GPoint(double longi, double lat, int pointNumber)
            : this(longi, lat)
        {
            this._pointNumber = pointNumber;
        }

        /// <summary>
        /// Creates new GPoint object with given parameters.
        /// </summary>
        /// <param name="longi">Longitude from -180 to 180 </param>
        /// <param name="lat">Latitude from -180 to 180 </param>
        /// <param name="pointNumber">Positive integer that represent order of point in route-not important</param>
        /// <param name="timeTaken">DateTimeOffset value represents time when was this point taken</param>
        public GPoint(double longi, double lat, int pointNumber,DateTimeOffset timeTaken)
            : this(longi, lat,pointNumber)
        {
            this.TimeTaken = TimeTaken;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the PointNumber from Route where this point belongs.
        /// </summary>
        public int PointNumber
        {
            get
            {
                return this._pointNumber;
            }
            set
            {
                this._pointNumber = value;
            }
        }

        /// <summary>
        /// Returns true point is valid (now working if coordinates are 0,0 ) 
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (this.Latitude != 0 || this.Longitude != 0);
            }
        }

        /// <summary>
        /// Gets basic string representation of the point.
        /// </summary>
        public String GetStringRepresentation
        {
            get
            {
                return this.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the time where this poing was taken.
        /// </summary>
        public DateTimeOffset TimeTaken
        {
            get
            {
                return this._pointTimeTaken;
            }
            set
            {
                this._pointTimeTaken = value;
            }
        }

        /// <summary>
        /// Gets or sets list of String elements containing picutre isolated storage path about pictures connected with this point.
        /// </summary>
        public List<String> IsolatedStoragePicturePaths 
        { 
            get
            {
                return this._isolatedStoragePicturePaths;
            }
            set
            {
                this._isolatedStoragePicturePaths = value;
            }
        }
        #endregion

        #region PublicFunctions

        public override string ToString()
        {
            String result = "";
            if (this._pointNumber != 0)
            {
                result = this._pointNumber + ":";
            }
            return result + this.Latitude.ToString(formatStr) + " " + this.Longitude.ToString(formatStr);
        }

        /// <summary>
        /// Check if both GPoint are equal. See use of checkTimeStamp parameter.
        /// </summary>
        /// <param name="point">Point to compare to</param>
        /// <param name="checkTimeStamp">If false then only compares Longitude and Latitude parameters of two object.
        /// Otherwise compares and TimeTaken property</param>
        /// <param name="secondsIgnore">If specified then compares with +- seconds when comparing TimeTaken property</param>
        /// <returns></returns>
        public bool IsEqual(GPoint point, bool checkTimeStamp, int secondsIgnore = 0)
        {
            bool isEqual = this.Longitude == point.Longitude && this.Latitude == this.Latitude;

            if (isEqual == true && checkTimeStamp == true)
            {
                TimeSpan difference = this.TimeTaken - point.TimeTaken;
                isEqual = Math.Abs(difference.TotalSeconds) <= secondsIgnore;
            }
            return isEqual;
        }

        /// <summary>
        /// Make a new GPoing object from current one.
        /// </summary>
        /// <returns>Clonned GPoint Object</returns>
        public GPoint Clone()
        {
            GPoint newgPoint = new GPoint(this.Longitude, this.Latitude, this.PointNumber, this.TimeTaken);

            newgPoint.IsolatedStoragePicturePaths = new List<String>(this.IsolatedStoragePicturePaths);

            return newgPoint;
        }

        #endregion
    }
}
