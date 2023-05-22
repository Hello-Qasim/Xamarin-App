using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressTracketXamarin
{
    public class Utils
    {
        public static String GetRequiresAssessmentStatus(int requiresAssessmentFlag)
        {
            if (requiresAssessmentFlag == 0)
            {
                return "No";
            }
            else
            {
                return "Yes";
            }
        }

        public static String GetFormattedAmount(int amount)
        {
            return amount + "£";
        }

        public static String GetCurrentDate()
        {
            return new SimpleDateFormat("dd-MM-yyyy", Locale.GetDefault(Locale.Category.Display)).Format(new Date());
        }

    }
}