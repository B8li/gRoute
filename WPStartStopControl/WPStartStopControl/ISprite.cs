using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;

namespace WPStartStopControl
{
    public interface ISprite
    {
        Brush Fill { get; set; }
        System.Windows.Visibility ControlVisibility();
    }
}
