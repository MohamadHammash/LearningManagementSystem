using System;

using Itenso.TimePeriod;

namespace Lms.MVC.UI.Utilities
{
    public class TimeTools
    {
        /// <summary>
        /// Checks if start and end is within containerstart and containerend
        /// </summary>
        /// <param name="containerStart"></param>
        /// <param name="containerEnd"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsInside(DateTime containerStart, DateTime containerEnd, DateTime start, DateTime end)
        {
            TimeRange Container = new TimeRange(containerStart, containerEnd);
            return Container.HasInside(new TimeRange(start, end));
        }
    }
}