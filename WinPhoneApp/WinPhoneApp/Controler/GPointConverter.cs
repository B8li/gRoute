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
using System.Collections.Generic;

namespace WinPhoneApp
{
    public class GPointConverter
    {
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
                result +="<GpsData longitude=\""+ point.Long.ToString().Replace(',','.')+"\" ";
                result +="\t latitude=\""+point.Lat.ToString()+"\" />";
            }

            result += Environment.NewLine + "\t";
            result += "</SensorData>";
            return result;
        }
    }
}
