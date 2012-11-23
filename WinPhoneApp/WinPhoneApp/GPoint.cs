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
    public class GPoint
    {
        private double _long;
        private double _lat;

        private int _pointNumber;
        private String formatStr = "0.########";
        public GPoint() { }

        public GPoint(int pointNumber)
        {
            this._long = 0;
            this._lat = 0;
            this._pointNumber = pointNumber;
        }
        public GPoint(double longi, double lat)
        {
            this._long = longi;
            this._lat = lat;
            this._pointNumber = 0;
        }
        
        public GPoint(double longi, double lat,int pointNumber)
        {
            this._long = longi;
            this._lat = lat;
            this._pointNumber = pointNumber;
        }
        public double Long
        {
            get
            {
                return this._long;
            }
            set
            {
                this._long = value;
            }
        }

        public double Lat
        {
            get
            {
                return this._lat;
            }
            set
            {
                this._lat = value;
            }
        }

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

        public bool IsValid
        {
            get
            {
                return this._lat != 0 || this._long != 0;
            }
        }

        public String GetStringRepresenatation
        {
            get
            {
                return this.ToString();
            }
        }

        public override string ToString()
        {
            String result = "";
            if (this._pointNumber != 0)
            {
                result = this._pointNumber + ":";
            }
            return result + this._lat.ToString(formatStr) + " " + this._long.ToString(formatStr);
        }

        public bool IsEqual(GPoint point)
        {
            return this.Long == point.Long && this.Lat == this.Lat;
        }
    }
}
