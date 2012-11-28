using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WinPhoneApp
{
    /// <summary>
    /// Util class for diferent conversion of GPoing Data.
    /// </summary>
    public class GPointConverter
    {
        /// <summary>
        /// Return's string made from list of coordinates(GPoing) that that can be imported in Windows Phone Simulator.
        /// </summary>
        /// <param name="points">List of GPoing elements that need's to be imported in WP Simulator.</param>
        /// <returns>String that can be imported in Windows Phone Simulator.</returns>
        public static String FromGpointToWP7Simulator(List<GPoint> points)
        {
            String result = "";
            result += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            
            result += Environment.NewLine;
            result += "<WindowsPhoneEmulator xmlns=\"http://schemas.microsoft.com/WindowsPhoneEmulator/2009/08/SensorData\">";
            
            result += Environment.NewLine + "\t";
            result += "<SensorData>";
            
            result +=Environment.NewLine + "\t\t";
            result += "<Header version=\"1\" />";

            foreach (GPoint point in points)
	        {
                result +=Environment.NewLine + "\t\t\t";
                result +="<GpsData longitude=\""+ point.Longitude.ToString().Replace(',','.')+"\" ";
                result +="\t latitude=\""+point.Latitude.ToString()+"\" />";
            }

            result += Environment.NewLine + "\t";
            result += "</SensorData>";
            return result;
        }
    }
}
