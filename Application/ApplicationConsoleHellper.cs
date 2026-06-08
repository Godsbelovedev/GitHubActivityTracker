using GitHubActivity.Infrastructure.Implementation;
using GitHubActivity.Infrastructure.Interfaces;

namespace GitHubActivity.Application
{
    public class ApplicationConsoleHellper
    {
        public static void SaveEvents(List<GitHubEvent> events)
        {
            bool allStored = false;
            var rep = new GitEventRepository();
            foreach (var ev in events)
            {
                rep.AddGitHubActivity(ev);
                allStored = true;
            }
            if (allStored)
            {
                Console.WriteLine("saved successful");
            }
            else
            {
                Console.WriteLine("save fail");
            }
        }
        public static int VerifyOption(int min, int max)
        {
            if (min == max)
            {
                int option;

                Console.WriteLine("input option");
                bool verifyOption = int.TryParse(Console.ReadLine(), out option);
                while (!verifyOption && option != max)
                {
                    Console.WriteLine($"option cant be above or below {max}");
                    verifyOption = int.TryParse(Console.ReadLine(), out option);
                }
                return option;
            }
            else //if (min != max)
            {
                int option;

                Console.WriteLine($"input from option {min} - {max}");
                bool verifyOption = int.TryParse(Console.ReadLine(), out option);
                while (!verifyOption && option > max && option < min)
                {
                    Console.WriteLine($"only options from {min} to {max} are allowed");
                    verifyOption = int.TryParse(Console.ReadLine(), out option);
                }
                return option;
            }

        }
    }
}