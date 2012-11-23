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
using System.ComponentModel;

namespace WinPhoneApp
{
    public class Adder:INotifyPropertyChanged
    {
        private int topValue;
        private int bottomValue;
        public Adder()
        {
        }
        public int TopValue 
        {
            get 
            {
                return this.topValue;
            }

            set
            {
                this.topValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AnswerValue"));
                }
                
            }
        }

        public int BottomValue
        {
            get
            {
                return this.bottomValue;
            }

            set
            {
                this.bottomValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AnswerValue"));
                }
            }
        }

        public int AnswerValue
        {
            get
            {
                return this.topValue + this.bottomValue;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
