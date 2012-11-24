using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BCJobClock
{
    public static class TextBoxAllowable
    {

        public static readonly DependencyProperty AllowedChars = DependencyProperty.RegisterAttached(
            "AllowedChars",
            typeof(string),
            typeof(TextBoxAllowable),
            new UIPropertyMetadata("", OnAllowableCharsChanged));


        public static String GetAllowedChars(DependencyObject d)
        {
            return (String)d.GetValue(AllowedChars);

            
        }
        public static void SetAllowedChars(DependencyObject d, String value)
        {
            d.SetValue(AllowedChars, value);

        }
        //code that handles changes to the AllowedChars property

        private static void OnAllowableCharsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string allowedchars = (string)e.NewValue;

            TextBox textbox = (TextBox)d;

            if (allowedchars != "")
            {
                textbox.PreviewTextInput += AllowChars;
                textbox.PreviewKeyDown += ReviewKeyDown;


            }
            else
            {
                textbox.PreviewTextInput -= AllowChars;
                textbox.PreviewKeyDown -= ReviewKeyDown;
            }





        }
        public static void AllowChars(object sender, TextCompositionEventArgs e)
        {

            var d = (DependencyObject)sender;
            String allowed = (String)d.GetValue(AllowedChars);

            foreach (char ch in e.Text)
            {
                if (!allowed.Contains(ch))
                    e.Handled = true;


            }

        }
        //disallow space if it isn't in the AllowedChars Property
        private static void ReviewKeyDown(object sender, KeyEventArgs e)
        {
            var d = (DependencyObject)sender;
            String allowed = (String)d.GetValue(AllowedChars);
            if (e.Key == Key.Space)
            {
                //Disallow if there are no spaces in allowed...
                e.Handled = (allowed.Contains(" "));


            }


        }

    }
}
