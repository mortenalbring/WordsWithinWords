using System;
using System.Diagnostics;

namespace WordsWithinWords
{
    internal class Progress
    {
        public static void OutputTimeRemaining(int index, int max, Stopwatch sw, string message = null)
        {
            var stepsRemaining = max - index;
            var percentageDone = Math.Round((float)index / max * 100, 2);

            var timePerStep = (float)sw.ElapsedMilliseconds / index;

            var timeRemaining = stepsRemaining * timePerStep;
            var timeRemainingMins = timeRemaining / 1000 / 60;

            if (index % 1000 == 0)
            {
                Console.WriteLine($"{percentageDone} %  - {timeRemainingMins} mins remaining \t {message}");
            }
        }
    }
}