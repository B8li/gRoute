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
using Microsoft.Phone.Tasks;

namespace WinPhoneApp
{
    public static class Tasks
    {
        /// <summary>
        /// Composes an email using the specified arguments
        /// </summary>
        /// <param name="to">The recepient(s) of the email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email contents</param>
        /// <param name="cc">The recipient(s) on the cc line of the email</param>
        public static void ComposeEmail(string to, string subject, string body,
                                         string cc = "")
        {
            var task = new EmailComposeTask()
            {
                To = to,
                Subject = subject,
                Body = body,
                Cc = cc,
            };
            task.Show();
        }
    }

}
