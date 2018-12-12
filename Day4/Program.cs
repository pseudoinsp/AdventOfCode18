using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt").Result.ToList();

            lines.Sort(new EntrySorter());

            HashSet<Guard> guards = ParseData(lines);

            (Guard, int) guardWithMostSleepingTime = GuardWithMostAsleepTime(guards);

            int mostFrequentSleepingMinute = MostFrequentSleepingMinute(guardWithMostSleepingTime.Item1).Key;

            // Part1
            Console.WriteLine($"Guard with the most sleep: {guardWithMostSleepingTime.Item1.Id}");
            Console.WriteLine($"Most frequent sleeping minute of this guard: {mostFrequentSleepingMinute}");

            // Part2

            var guardWithMostSleepInAMinute = GuardWithMostFrequentSleepingMinute(guards);

            Console.WriteLine();
            Console.WriteLine($"Guard who is sleeping in a minute most frequently: {guardWithMostSleepInAMinute.Item1.Id}");
            Console.WriteLine($"Most frequent sleeping minute of this guard: {guardWithMostSleepInAMinute.Item2}");

            Console.ReadLine();

        }

        public static (Guard, int) GuardWithMostAsleepTime(IEnumerable<Guard> guards)
        {
            Guard guardWithMostSleepingTime = guards.First();
            int currentMostSleepingTime = guards.First().AsleepTimeSum();

            foreach (var guard in guards)
            {
                if(guard.AsleepTimeSum() > currentMostSleepingTime)
                {
                    guardWithMostSleepingTime = guard;
                    currentMostSleepingTime = guard.AsleepTimeSum();
                }
            }

            return (guardWithMostSleepingTime, currentMostSleepingTime);
        }

        public static (Guard, int) GuardWithMostFrequentSleepingMinute(IEnumerable<Guard> guards)
        {
            Guard guardWithMostSleepInAMinute = guards.First();
            var mostSleepInAMinute = MostFrequentSleepingMinute(guards.First());

            foreach (var guard in guards)
            {
                var mostFrequentSleepingMinute = MostFrequentSleepingMinute(guard);
                if(mostFrequentSleepingMinute.Value > mostSleepInAMinute.Value)
                {
                    mostSleepInAMinute = mostFrequentSleepingMinute;
                    guardWithMostSleepInAMinute = guard;
                }
            }

            return (guardWithMostSleepInAMinute, mostSleepInAMinute.Key);
        }

        // 
        /// <summary>
        /// Which minute, asleep how many times 
        /// </summary>
        public static KeyValuePair<int, int> MostFrequentSleepingMinute(Guard guard)
        {
            var minutesAsleep = new Dictionary<int, int>();

            for (int i = 0; i < 60; i++)
            {
                minutesAsleep[i] = 0;
            }

            foreach (List<Tuple<DateTime, DateTime>> asleepTimesInADuty in guard.Duties.Select(d => d.AsleepTimes))
            {
                foreach (Tuple<DateTime, DateTime> asleepTimeInADuty in asleepTimesInADuty)
                {
                    if(asleepTimeInADuty.Item1.Hour == asleepTimeInADuty.Item2.Hour)
                    {
                        for (int i = asleepTimeInADuty.Item1.Minute; i < asleepTimeInADuty.Item2.Minute; i++)
                        {
                            minutesAsleep[i]++;
                        }
                    }
                    else // sleeping through multiple hours
                    {
                        // TODO handle day difference ....
                        int hourDifference = asleepTimeInADuty.Item2.Hour - asleepTimeInADuty.Item1.Hour;

                        // first hour (not full)
                        for (int i = asleepTimeInADuty.Item1.Minute; i < 60; i++)
                        {
                            minutesAsleep[i]++;
                        }

                        // inbetween hours (full hours between starting and ending hour)
                        if(hourDifference > 2)
                        {
                            for (int i = 0; i < hourDifference - 2; i++)
                            {
                                for (int j = 0; j < 60; j++)
                                {
                                    minutesAsleep[j]++;
                                }
                            }
                        }

                        // last hour (not full)
                        for (int i = 0; i < asleepTimeInADuty.Item2.Minute; i++)
                        {
                            minutesAsleep[i]++;
                        }
                    }
                }
            }

            // return key with max value
            //return minutesAsleep.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            return minutesAsleep.Aggregate((l, r) => l.Value > r.Value ? l : r);
        }

        public static HashSet<Guard> ParseData(List<string> entries)
        {
            var guards = new HashSet<Guard>();

            int currentGuardId = -1;
            Duty currentDuty = new Duty();
            for(int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Contains("begins"))
                {
                    string[] lineParts = entries[i].Split(' ');
                    int guardId = int.Parse(lineParts[3].Substring(1));
                    
                    if (!guards.Select(g => g.Id).Contains(guardId))
                    {
                        guards.Add(new Guard(guardId));
                    }

                    // close the old duty entry
                    // dont add the invalid duty of the first line
                    if (currentDuty.StartTime != default(DateTime))
                    {
                        guards.First(g => g.Id == currentGuardId).Duties.Add(currentDuty);
                    }

                    // start the new duty entry
                    currentGuardId = guardId;
                    currentDuty = new Duty
                    {
                        StartTime = ParseDate(entries[i])
                    };
                }
                
                if (entries[i].Contains("wakes up"))
                {
                    DateTime asleepStart = ParseDate(entries[i-1]);
                    DateTime asleepEnd =  ParseDate(entries[i]);

                    currentDuty.AsleepTimes.Add(Tuple.Create(asleepStart, asleepEnd));
                }
            }

            return guards;
        }

        private static DateTime ParseDate(string date)
        {
            int indexOfDateTimeEnding = date.IndexOf(']');
            string dateExact = date.Substring(1, indexOfDateTimeEnding - 1);

            return DateTime.Parse(dateExact);
        }
    }

    [DebuggerDisplay("{Id}")]
    class Guard
    {
        public Guard(int id)
        {
            Id = id;
            Duties = new List<Duty>();
        }

        public int Id { get; }

        public List<Duty> Duties { get; } 
    }

    [DebuggerDisplay("{StartTime}")]
    class Duty
    {
        public Duty()
        {
            AsleepTimes = new List<Tuple<DateTime, DateTime>>();
        }

        public DateTime StartTime { get; set; }

        public List<Tuple<DateTime, DateTime>> AsleepTimes { get; }
    }

    class EntrySorter : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            DateTime xD = ParseDate(x);
            DateTime yD = ParseDate(y);

            return DateTime.Compare(xD, yD);
        }

        private DateTime ParseDate(string date)
        {
            int indexOfDateTimeEnding = date.IndexOf(']');
            string dateExact = date.Substring(1, indexOfDateTimeEnding - 1);

            return DateTime.Parse(dateExact);
        }
    }

    static class GuardExtensions
    {
        public static int AsleepTimeSum(this Guard guard)
        {
            int minutesAsleep = 0;
            foreach (var duty in guard.Duties)
            {
                foreach (var asleepTime in duty.AsleepTimes)
                {
                    minutesAsleep += (asleepTime.Item2 - asleepTime.Item1).Minutes;
                }
            }

            return minutesAsleep;
        }
    }
}
