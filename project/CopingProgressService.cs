using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public static class CopingProgressService
    {
        public static int CompletedExercises { get; set; } = 0;
        public static int TotalExercises { get; set; } = 4;

        public static string GetProgressText()
        {
            return $"{CompletedExercises}/{TotalExercises} completed";
        }

        public static double GetProgressValue()
        {
            return (double)CompletedExercises / TotalExercises;
        }
    }


}
