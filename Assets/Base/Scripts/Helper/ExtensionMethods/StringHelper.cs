using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Base.Helper
{
    public static class StringHelper
    {
        public static string FormatCurrency(this string amount)
        {
            return String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:N0}", amount);
        }
        
        public static string GetCurrencySymbol(string source)
        {
            return Regex.Replace(source, "[ ,.0123456789]+", "");
        }
    }
}

