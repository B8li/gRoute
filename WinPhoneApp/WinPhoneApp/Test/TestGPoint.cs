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

namespace WinPhoneApp.Test
{
    /// <summary>
    /// Testing class for gPoint class.
    /// </summary>
    public class TestGPoint
    {
        /// <summary>
        /// Test the gPoint.IsEqual() funciton. 
        /// Results of testing are shown in output window in VS.
        /// </summary>
        public static void TestIsEqual()
        {
            String message;
            bool result = false;
            GPoint point1 = new GPoint(40, 40, 0, new DateTimeOffset(DateTime.Now));
            GPoint point2;
            message = "Testing : GPoint.IsEqual - SamePoint without TimeTaken";
            result = point1.IsEqual(point1, false);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == true));

            message = "Testing : GPoint.IsEqual - SamePoint WITH 0 sec to ignore";
            result = point1.IsEqual(point1, true, 0);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == true));

            message = "Testing : GPoint.IsEqual - OtherPoint(0 sec)  WITH 3 sec to ignore";
            point2 = point1.Clone();
            result = point1.IsEqual(point2, true, 3);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == true));

            message = "Testing : GPoint.IsEqual - OtherPoint(+3 sec)  WITH 0 sec to ignore";
            point2 = point1.Clone();
            point2.TimeTaken.AddSeconds(3);
            result = point1.IsEqual(point2, true, 0);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == false));

            message = "Testing : GPoint.IsEqual - OtherPoint(-3 sec)  WITH 0 sec to ignore";
            point2 = point1.Clone();
            point2.TimeTaken = point2.TimeTaken.Subtract(new TimeSpan(0, 0, 3));
            result = point1.IsEqual(point2, true, 0);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == false));

            message = "Testing : GPoint.IsEqual - OtherPoint(-3 sec)  WITH 3 sec to ignore";
            point2 = point1.Clone();
            point2.TimeTaken = point2.TimeTaken.Subtract(new TimeSpan(0, 0, 3));
            result = point1.IsEqual(point2, true, 3);
            System.Diagnostics.Debug.WriteLine(message + "\t\t\t " + (result == true));

        }
    }
}
