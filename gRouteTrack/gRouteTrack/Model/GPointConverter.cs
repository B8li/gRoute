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
using System.Xml;
using gRouteTrack.Model;
using System.Data;
using System.Text.RegularExpressions;
namespace gRouteTrack
{
    /// <summary>
    /// Util class for diferent conversion of GPoing Data.
    /// </summary>
    public class GPointConverter
    {

        #region Private Functions
        private static DateTime FromStringToDateTime(String input)
        {
            try
            {
                String[] split = { ":", "-", " " };
                String[] parts = input.Split(split, StringSplitOptions.RemoveEmptyEntries);
                return new DateTime(Int32.Parse(parts[0]), Int32.Parse(parts[1]), Int32.Parse(parts[2]), Int32.Parse(parts[3]), Int32.Parse(parts[4]), Int32.Parse(parts[5]));
            }
            catch (Exception)
            {
                return gSystemSettings.DefaultDate;
            }
        }

        #endregion

        #region Public Funcions
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

            result += Environment.NewLine + "\t\t";
            result += "<Header version=\"1\" />";

            foreach (GPoint point in points)
            {
                result += Environment.NewLine + "\t\t\t";
                result += "<GpsData longitude=\"" + point.Longitude.ToString().Replace(',', '.') + "\" ";
                result += "\t latitude=\"" + point.Latitude.ToString() + "\" />";
            }

            result += Environment.NewLine + "\t";
            result += "</SensorData>";
            return result;
        }

        public static String FromGRouteToIsolatedStorageFormat(GRoute route)
        {
            String result = "";

            result += "<xml>";

            result += Environment.NewLine + "\t" + "<route> ";
            result += Environment.NewLine + "\t\t" + "<StartTime>" + route.StartTime.ToString("yyyy:MM:dd:HH:mm:ss") + "</StartTime>";
            result += Environment.NewLine + "\t\t" + "<EndTime>" + route.EndTime.ToString("yyyy:MM:dd:HH:mm:ss") + "</EndTime> ";
            result += Environment.NewLine + "\t\t" + "<FullTime>" + route.FullTime.TotalSeconds + "</FullTime> ";
            result += Environment.NewLine + "\t\t" + "<UploadCheck>" + route.UploadCheck.ToString() + "</UploadCheck> ";


            result += Environment.NewLine + "\t\t" + "<coordinates>";
            foreach (GPoint point in route.Coordinates)
            {
                result += Environment.NewLine + "\t\t\t" + "<point>";
                result += Environment.NewLine + "\t\t\t\t" + "<Lon>" + point.Longitude.ToString().Replace(",", ".") + "</Lon>";
                result += Environment.NewLine + "\t\t\t\t" + "<Lat>" + point.Latitude.ToString().Replace(",", ".") + "</Lat>";
                result += Environment.NewLine + "\t\t\t\t" + "<Speed>" + point.Speed.ToString().Replace(",", ".") + "</Speed>";
                result += Environment.NewLine + "\t\t\t\t" + "<Alt>" + point.Altitude.ToString().Replace(",", ".") + "</Alt>";
                result += Environment.NewLine + "\t\t\t\t" + "<Com>" + point.Comment.ToString() + "</Com>";
                result += Environment.NewLine + "\t\t\t\t" + "<TT>" + point.TimeTaken.ToString("yyyy:MM:dd:HH:mm:ss") + "</TT>";
                result += Environment.NewLine + "\t\t\t\t" + "<Num>" + point.PointNumber.ToString() + "</Num>";
                //TO-DO Make picture paths here
                result += Environment.NewLine + "\t\t\t" + "</point>";
            }
            result += Environment.NewLine + "\t\t" + "</coordinates>";

            result += Environment.NewLine + "\t" + "</route>";

            result += Environment.NewLine + "</xml>";

            return result;
        }

        public static bool FromIsolatedStorageFormatToGRoute(String text, out GRoute route)
        {
            route = new GRoute();
            bool succes = false;
            String[] split = { Environment.NewLine };
            String pattern = @">(.*?)<";
            GPoint point = new GPoint();

            String lineValue = "";
            String decimalPointSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            double valueDouble = 0;
            try
            {
                String[] lines = text.Split(split, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    if (line.Contains("<StartTime>"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.StartTime = GPointConverter.FromStringToDateTime(lineValue);
                    }
                    else if (line.Contains("<EndTime>"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.EndTime = GPointConverter.FromStringToDateTime(lineValue);
                    }
                    else if (line.Contains("FullTime"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.FullTime = TimeSpan.FromSeconds(Int32.Parse(lineValue));
                    }
                    else if (line.Contains("UploadCheck"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.UploadCheck = Boolean.Parse(lineValue);
                    }
                    else if (line.Contains("<point>"))
                    {
                        point = new GPoint();
                    }
                    else if (line.Contains("Lon"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        valueDouble = 0;
                        if (Double.TryParse(lineValue, out valueDouble))
                        {
                            point.Longitude = Double.Parse(lineValue);
                        }
                        else
                        {
                            MessageBox.Show("Eror parsing to Double :" + lineValue);
                        }
                    }
                    else if (line.Contains("Lat"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        valueDouble = 0;
                        if (Double.TryParse(lineValue, out valueDouble))
                        {
                            point.Latitude = Double.Parse(lineValue);
                        }
                        else
                        {
                            MessageBox.Show("Eror parsing to Double :" + lineValue);
                        }
                    }
                    else if (line.Contains("Speed"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        point.Speed = Double.Parse(lineValue);
                    }
                    else if (line.Contains("Alt"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        point.Altitude = Double.Parse(lineValue);
                    }
                    else if (line.Contains("Com"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.Comment = lineValue;
                    }
                    else if (line.Contains("TT"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.TimeTaken = new DateTimeOffset(GPointConverter.FromStringToDateTime(lineValue));
                    }
                    else if (line.Contains("Num"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.Altitude = Int32.Parse(lineValue);
                    }
                    else if (line.Contains("/point"))
                    {
                        route.AddNewGPoint(point, false);
                    }
                }
            }
            catch (Exception e)
            {
                succes = false;
                MessageBox.Show(e.Message);
            }

            return succes;

        }

        public static String FromGRoutesToIsolatedStorageFormat(List<GRoute> routes)
        {
            String result = "";

            result += "<xml>";

            foreach (GRoute route in routes)
            {
                result += Environment.NewLine + "\t" + "<route> ";
                result += Environment.NewLine + "\t\t" + "<StartTime>" + route.StartTime.ToString("yyyy:MM:dd:HH:mm:ss") + "</StartTime>";
                result += Environment.NewLine + "\t\t" + "<EndTime>" + route.EndTime.ToString("yyyy:MM:dd:HH:mm:ss") + "</EndTime> ";
                result += Environment.NewLine + "\t\t" + "<FullTime>" + route.FullTime.TotalSeconds + "</FullTime> ";
                result += Environment.NewLine + "\t\t" + "<UploadCheck>" + route.UploadCheck.ToString() + "</UploadCheck> ";


                result += Environment.NewLine + "\t\t" + "<coordinates>";
                foreach (GPoint point in route.Coordinates)
                {
                    result += Environment.NewLine + "\t\t\t" + "<point>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Lon>" + point.Longitude.ToString().Replace(",", ".") + "</Lon>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Lat>" + point.Latitude.ToString().Replace(",", ".") + "</Lat>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Speed>" + point.Speed.ToString().Replace(",", ".") + "</Speed>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Alt>" + point.Altitude.ToString().Replace(",", ".") + "</Alt>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Com>" + point.Comment.ToString() + "</Com>";
                    result += Environment.NewLine + "\t\t\t\t" + "<TT>" + point.TimeTaken.ToString("yyyy:MM:dd:HH:mm:ss") + "</TT>";
                    result += Environment.NewLine + "\t\t\t\t" + "<Num>" + point.PointNumber.ToString() + "</Num>";
                    //TO-DO Make picture paths here
                    result += Environment.NewLine + "\t\t\t" + "</point>";
                }
                result += Environment.NewLine + "\t\t" + "</coordinates>";

                result += Environment.NewLine + "\t" + "</route>";
            }

            result += Environment.NewLine + "</xml>";

            return result;
        }

        public static bool FromIsolatedStorageFormatToGRoutes(String text, out List<GRoute> routes)
        {
            routes = new List<GRoute>();
            bool succes = false;
            String[] split = { Environment.NewLine };
            String pattern = @">(.*?)<";
            GRoute route = new GRoute();
            GPoint point = new GPoint();

            String lineValue = "";
            String decimalPointSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            double valueDouble = 0;
            try
            {
                String[] lines = text.Split(split, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    if (line.Contains("<route>"))
                    {
                        route = new GRoute();
                    }
                    else if (line.Contains("<StartTime>"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.StartTime = GPointConverter.FromStringToDateTime(lineValue);
                    }
                    else if (line.Contains("<EndTime>"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.EndTime = GPointConverter.FromStringToDateTime(lineValue);
                    }
                    else if (line.Contains("FullTime"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.FullTime = TimeSpan.FromSeconds(Int32.Parse(lineValue));
                    }
                    else if (line.Contains("UploadCheck"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        route.UploadCheck = Boolean.Parse(lineValue);
                    }
                    else if (line.Contains("<point>"))
                    {
                        point = new GPoint();
                    }
                    else if (line.Contains("Lon"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        valueDouble = 0;
                        if (Double.TryParse(lineValue, out valueDouble))
                        {
                            point.Longitude = Double.Parse(lineValue);
                        }
                        else
                        {
                            MessageBox.Show("Eror parsing to Double :" + lineValue);
                        }
                    }
                    else if (line.Contains("Lat"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        valueDouble = 0;
                        if (Double.TryParse(lineValue, out valueDouble))
                        {
                            point.Latitude = Double.Parse(lineValue);
                        }
                        else
                        {
                            MessageBox.Show("Eror parsing to Double :" + lineValue);
                        }
                    }
                    else if (line.Contains("Speed"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        point.Speed = Double.Parse(lineValue);
                    }
                    else if (line.Contains("Alt"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        lineValue = lineValue.Replace(",", ".").Replace(".", decimalPointSeparator);
                        point.Altitude = Double.Parse(lineValue);
                    }
                    else if (line.Contains("Com"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.Comment = lineValue;
                    }
                    else if (line.Contains("TT"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.TimeTaken = new DateTimeOffset(GPointConverter.FromStringToDateTime(lineValue));
                    }
                    else if (line.Contains("Num"))
                    {
                        lineValue = Regex.Matches(line, pattern)[0].Groups[1].ToString();
                        point.Altitude = Int32.Parse(lineValue);
                    }
                    else if (line.Contains("/point"))
                    {
                        route.AddNewGPoint(point, false);
                    }
                    else if (line.Contains("/route"))
                    {
                        routes.Add(route);
                    }
                }
            }
            catch (Exception e)
            {
                succes = false;
                MessageBox.Show(e.Message);
            }

            return succes;

        }
        #endregion
    }
}
