using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JobClockAdmin
{
    public static class StringExtensions
    {
        /// <summary>
        /// this string is the regular expression, sregex is the string to test.
        /// </summary>
        /// <param name="test">Regular Expression</param>
        /// <param name="sregex">String to match</param>
        /// <returns></returns>
        public static bool TestRegex(this string test, String sregex)
        {
            // System.Text.RegularExpressions.Regex re = new Regex(test,RegexOptions.IgnoreCase);
            return System.Text.RegularExpressions.Regex.Match(test,sregex, RegexOptions.IgnoreCase).Success;




        }
    }
}
