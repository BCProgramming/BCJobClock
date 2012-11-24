using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Windows;

namespace BCJobClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void EventSetter_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("EmitClickSound...");
            SoundPlayer sp = new SoundPlayer("click.wav");
            sp.Load();
            sp.Play();
        }
        
    }
}
