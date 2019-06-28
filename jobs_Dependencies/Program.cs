using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace jobs_Dependencies
{
    class Program
    {
        static List<string> result = new List<string>();
        static Dictionary<string, string> jobsDictionary = new Dictionary<string, string> { };
        static void Main(string[] args)
        {
            try
            {
                EnterData();
                foreach (var item in jobsDictionary)
                {
                    DependencySearch(item.Key, new List<string>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            Console.WriteLine("the final sequence is :");
            Console.WriteLine(string.Join(", ", result));
        }

        static void DependencySearch(string key, List<string> historyStack)
        {
            // if job exists as dependency but not as a job
            if (!jobsDictionary.ContainsKey(key))
                throw new Exception($"error => error => this job {key} registered as dependency but not as a job");


            // if job exists in result sequence => get its dependance before
            if (result.Contains(key))
                return;

            // if job has no dependencies
            if (string.IsNullOrWhiteSpace(jobsDictionary[key]))
            {
                result.Add(key);
                return;
            }

            // check dependency of himself
            if (jobsDictionary[key] == key)
            {
                throw new Exception($"error => that jobs can’t depend on themselves {key} => {key}");
            }

            // check circular dependencies
            if (historyStack.Contains(key))
            {
                historyStack.Add(key);
                int firstIndex = historyStack.IndexOf(key);
                int LastIndex = historyStack.LastIndexOf(key);

                throw new Exception($"error => that jobs can’t have circular dependencies {string.Join(" => ", historyStack.Skip(firstIndex).Take(LastIndex - firstIndex + 1))}");
            }
            historyStack.Add(key);

            // recursion dependency Search
            DependencySearch(jobsDictionary[key], historyStack);

            result.Add(key);
            return;
        }

        static void EnterData()
        {
            int jobsNumber = 0;
            Console.WriteLine("Enter number of the jobs");
            int.TryParse(Console.ReadLine(), out jobsNumber);
            if (jobsNumber < 1)
                return;

            Console.WriteLine("Enter every job in new line");
            Console.WriteLine("Enter Job as a => c if it has dependancy or a or a => if not");

            for (int i = 0; i < jobsNumber; i++)
            {
                var match = Regex.Match(Console.ReadLine(), @"^([a-zA-Z])\s*(=>\s*([a-zA-Z])?)?");
                if (match.Success)
                {
                    string job = match.Groups[1].Value;
                    string jobDependancy = match.Groups[3].Value;
                    if (jobsDictionary.ContainsKey(job))
                    {
                        throw new Exception($"error => this job {job} is entered before");
                    }
                    jobsDictionary.Add(job, jobDependancy);
                }
            }
            Console.WriteLine("your data :");
            foreach (var item in jobsDictionary)
            {
                Console.WriteLine($" {item.Key} => {item.Value} ");
            }
        }
    }
}
