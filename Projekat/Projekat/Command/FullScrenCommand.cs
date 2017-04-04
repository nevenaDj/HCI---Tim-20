using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekat.Command
{
    public static class FullScrenCommand
    {
        public static readonly RoutedUICommand FullScreen = new RoutedUICommand(
            "FullScreen",
            "Full Screen",
            typeof(FullScrenCommand),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F11, ModifierKeys.None),
                new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift)
            }
            );
    }
}
