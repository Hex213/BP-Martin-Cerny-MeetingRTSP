using System;

namespace LibUIAcademyFramework.XanderUI
{
    public static class XUIPercentage
    {
        // Methods
        public static int IntToPercent(int number, int total) =>
            Convert.ToInt32(Math.Round((double)(((double)(100 * number)) / ((double)total))));

        public static int PercentToInt(int number, int total) =>
            Convert.ToInt32(Math.Round((double)((total / 100) * number)));
    }



}
