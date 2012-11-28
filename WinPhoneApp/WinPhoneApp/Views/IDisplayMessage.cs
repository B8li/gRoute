using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneApp.Views
{
    public interface IDisplayMessage : IView
    {
        String ErrorMessage { get; set; }
        String InfoMessage { get; set; }
        String SuccesfulMassage { get; set; }
    }
}
