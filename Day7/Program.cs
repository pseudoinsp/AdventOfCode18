using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            var steps = ParseInput(linesReadingTask.Result);

            // Task1
            var orderSteps = OrderOfExecution(steps);
            Console.Write($"Order of execution: ");

            foreach (char c in orderSteps)
            {
                Console.Write(c);
            }

            var stepsCopy = ParseInput(linesReadingTask.Result);
            // Task 2
            var timeOfExecution = TimeOfExecution(stepsCopy, 5);

            Console.WriteLine();
            // -1 since the second incrementation happens before checking the exit condition
            Console.WriteLine($"Time of construction: {timeOfExecution - 1}");

            Console.ReadLine();
        }

        public static int TimeOfExecution(List<Step> steps, int numberOfWorkers)
        {
            var workers = new Dictionary<int, Step>();

            for (int i = 0; i < numberOfWorkers; i++)
            {
                workers.Add(i, null);
            }

            int secondOfWork = 0;

            while (steps.Any())
            {
                SimulateSecond(secondOfWork, steps, workers);
                secondOfWork++;
            }

            return secondOfWork;
        }

        static void SimulateSecond(int second, List<Step> steps, IDictionary<int, Step> workers)
        {
            IList<Step> finishedStepsInThisSecond = steps.Where(s => s.InProgress && s.StartingTime + s.Duration == second).OrderBy(s => s.Name).ToList();
            IList<int> justFinishedWorkers = workers.Where(w => finishedStepsInThisSecond.Contains(w.Value)).Select(w => w.Key).ToList();

            foreach (var finishedStep in finishedStepsInThisSecond)
            {
                foreach (var step in steps)
                {
                    step.Prerequisites.Remove(finishedStep);
                }
                steps.Remove(finishedStep);
            }

            foreach (int i in justFinishedWorkers)
            {
                workers[i] = null;
            }

            IList<Step> stepsCanBeStarted = steps.Where(s => !s.InProgress && s.Prerequisites.Count == 0).ToList();
            IList<int> availableWorkers = workers.Where(kv => kv.Value == null).Select(w => w.Key).ToList();

            foreach (var step in stepsCanBeStarted.OrderBy(s => s.Name))
            {
                if (!availableWorkers.Any())
                {
                    return;
                }

                int workerToScheduleWork = availableWorkers.First();
                workers[workerToScheduleWork] = step;
                availableWorkers.Remove(workerToScheduleWork);
                step.InProgress = true;
                step.StartingTime = second;
            }
        }
        
        static List<Step> ParseInput(string[] input)
        {
            var steps = new List<Step>();

            foreach (var stepString in input)
            {
                char currentStepChar = stepString[36];
                char prerequisiteStepChar = stepString[5];

                var currentStep = steps.FirstOrDefault(s => s.Name == currentStepChar);
                var prerequisiteStep = steps.FirstOrDefault(s => s.Name == prerequisiteStepChar);

                if (currentStep == null)
                {
                    steps.Add(new Step(currentStepChar));
                    currentStep = steps.First(s => s.Name == currentStepChar);
                }
                if (prerequisiteStep == null)
                {
                    steps.Add(new Step(prerequisiteStepChar));
                    prerequisiteStep = steps.First(s => s.Name == prerequisiteStepChar);
                }

                currentStep.Prerequisites.Add(prerequisiteStep);
            }

            return steps;
        }

        public static List<char> OrderOfExecution(List<Step> steps)
        {
            var orderOfExecution = new List<char>();

            while (steps.Any())
            {
                var stepToExecute = steps.OrderBy(s => s.Prerequisites.Count).ThenBy(s => s.Name).First();

                orderOfExecution.Add(stepToExecute.Name);
                steps.Remove(stepToExecute);
                foreach (var step in steps)
                {
                    step.Prerequisites.Remove(stepToExecute);
                }
            }

            return orderOfExecution;
        }
    }

    [DebuggerDisplay("{Name} , Prerequisites: {Prerequisites.Count}")]
    class Step
    {
        public Step(char name)
        {
            Name = name;
            Prerequisites = new List<Step>();
        }

        public char Name { get; }

        public bool InProgress { get; set; }

        public int StartingTime { get; set; }

        public int Duration => 60 + (Name - 64);

        public List<Step> Prerequisites { get; }
    }
}
