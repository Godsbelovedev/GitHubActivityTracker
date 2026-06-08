using System.Threading.Tasks;
using GitHubActivity.Application;
using GitHubActivity.Application.Implementation;
using GitHubActivity.Application.Interfaces;
using GitHubActivity.Infrastructure.Implementation;
using GitHubActivity.Infrastructure.Interfaces;

namespace GitHubActivity.Presentation
{
    public class TaskMenu
    {
        private readonly IGitHubServiceInterface _eventService;
        private readonly IGitEventRepositoryInterface _eventRepo;
        public TaskMenu()
        {
            _eventService = new GitHubService();
            _eventRepo = new GitEventRepository();
        }

        public async Task Start(string? username)
        {
            while (true)
            {
                Console.WriteLine($@"WELCOME, INPUT AN OPTION
                  1. FETCH GITHUB EVENT(S)
                  2. SEARCH STORED EVENT BY USERNAME
                  3. SEARCH STORED EVENT BY EVENT TYPE
                  4. SEARCH STORED EVENT BY REPO NAME
                  0. EXIT");
                Console.WriteLine("SELECT an option between 0 - 4");
                bool isConfirmedOption = int.TryParse(Console.ReadLine(), out int option);
                while (!isConfirmedOption && option < 0 && option > 4)
                {
                    Console.WriteLine("invalid option, option can not be empty, below 0 or above 4");
                    isConfirmedOption = int.TryParse(Console.ReadLine(), out option);
                }

                switch (option)
                {
                    case 1:
                        if (string.IsNullOrWhiteSpace(username))
                        {
                            Console.WriteLine("input username to search events");
                            username = Console.ReadLine();
                            while (string.IsNullOrWhiteSpace(username))
                            {
                                Console.WriteLine("username cant be empty, input a valid username");
                                username = Console.ReadLine();
                            }
                        }
                        await PrintGitHubEvents(username);
                        break;

                    case 2:
                        SearchEventByUserName();
                        break;
                    case 3:
                        SearchEventByEventType();
                        break;
                    case 4:
                        SearchEventByRepoName();
                        break;
                    case 0:
                        Console.WriteLine("Bye-Bye");
                        Environment.Exit(0);
                        Console.WriteLine(Environment.NewLine);
                        break;
                }
            }

        }
        private async Task PrintGitHubEvents(string username)
        {

            var events = await _eventService.FetchEventsFromGitHub(username);

            if (events == null || !events.Any())
            {
                Console.WriteLine("no event found");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=======================================================");
            Console.WriteLine($"📊   TRACKER REPORT: FOUND {events.Count} GITHUB EVENTS");
            Console.WriteLine("=======================================================");
            Console.ResetColor();

            if (events == null || events.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" No recent events found for this user.");
                Console.ResetColor();
                return;
            }

            foreach (var ev in events)
            {
                string localTimeStr = ev.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n📌 [EVENT ID: {ev.EventId}] -> {ev.EventType.ToUpper()}");
                Console.ResetColor();

                Console.WriteLine($"   📅 Time (Local): {localTimeStr}");

                if (ev.Actor != null)
                {
                    Console.WriteLine($"   👤 Actor: {ev.Actor.Login} (ID: {ev.Actor.Id})");
                }
                else
                {
                    Console.WriteLine("   👤 Actor: Anonymous / Nil");
                }

                if (ev.Repo != null)
                {
                    Console.WriteLine($"   📁 Repo:  {ev.Repo.RepositoryName} (ID: {ev.Repo.Id})");
                }
                else
                {
                    Console.WriteLine("   📁 Repo:  Unknown Repository / Nil");
                }


                if (ev.Commits != null && ev.Commits.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("   📝 Commits Attached:");
                    Console.ResetColor();

                    int count = 1;
                    foreach (var commit in ev.Commits)
                    {
                        string commitTime = commit.CommitDate.ToLocalTime().ToString("HH:mm:ss");

                        Console.WriteLine($"{count}. ⚡ [{commitTime}] {commit.CommitAuthor}: {commit.CommitMessage}");
                        count++;
                    }
                }
                else if (ev.EventType.Equals("PushEvent", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("   📝 Commits: No commit messages available in this payload.");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
            }
            Console.WriteLine($@"Do you wish to save these events? SELECT:
                                CLICK '1' To SAVE
                                CLICK '0' To dispose");
            int option = ApplicationConsoleHellper.VerifyOption(0, 1);
            if (option == 1)
            {
                _eventService.AddGitHubActivity(events);
            }
            else
            {
                return;
            }

        }
        private void SearchEventByUserName()
        {
            Console.WriteLine("input username to search stored events");
            var usernameToSearchWith = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(usernameToSearchWith))
            {
                Console.WriteLine("username cant be empty, input a valid username");
                usernameToSearchWith = Console.ReadLine();
            }
            _eventService.GetGitHubEventsByUserName(usernameToSearchWith);
            var events = _eventRepo.GetAllGitHubEventsByUserName(usernameToSearchWith);
            Console.WriteLine($@"Do you wish to VIEW or DELETE an event in full? SELECT:
                                CLICK '1' To VIEW in full
                                CLICK '2' To DELETE
                                CLICK '0' To EXIT");
            int option = ApplicationConsoleHellper.VerifyOption(0, 2);
            if (option == 1)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.GetGitHubEvent(events[viewOption - 1].EventId);
            }
            else if (option == 2)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.DeleteEvent(events[viewOption - 1].EventId);
            }
            else
            {
                return;
            }
        }
        private void SearchEventByRepoName()
        {
            Console.WriteLine("input reponame to search stored events");
            var reponameToSearchWith = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(reponameToSearchWith))
            {
                Console.WriteLine("reponame cant be empty, input a valid username");
                reponameToSearchWith = Console.ReadLine();
            }
            _eventService.GetGitHubEventsByRepoName(reponameToSearchWith);
            var events = _eventRepo.GetAllGitHubEventsByRepoName(reponameToSearchWith);
            Console.WriteLine($@"Do you wish to VIEW or DELETE an event in full? SELECT:
                                CLICK '1' To VIEW in full
                                CLICK '2' To DELETE
                                CLICK '0' To EXIT");
            int option = ApplicationConsoleHellper.VerifyOption(0, 2);
            if (option == 1)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.GetGitHubEvent(events[viewOption - 1].EventId);
            }
            else if (option == 2)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.DeleteEvent(events[viewOption - 1].EventId);
            }
            else
            {
                return;
            }
        }
        private void SearchEventByEventType()
        {
            Console.WriteLine("input reponame to search stored events");
            var eventTypeToSearchWith = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(eventTypeToSearchWith))
            {
                Console.WriteLine("event type cant be empty, input a valid username");
                eventTypeToSearchWith = Console.ReadLine();
            }
            _eventService.GetGitHubEventsByType(eventTypeToSearchWith);
            var events = _eventRepo.GetAllGitHubEventsByType(eventTypeToSearchWith);
            Console.WriteLine($@"Do you wish to VIEW or DELETE an event in full? SELECT:
                                CLICK '1' To VIEW in full
                                CLICK '2' To DELETE
                                CLICK '0' To EXIT");
            int option = ApplicationConsoleHellper.VerifyOption(0, 2);
            if (option == 1)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.GetGitHubEvent(events[viewOption - 1].EventId);
            }
            else if (option == 2)
            {
                Console.WriteLine($"input from 1 to {events.Count} search stored events");
                int viewOption = ApplicationConsoleHellper.VerifyOption(0, events.Count);
                _eventService.DeleteEvent(events[viewOption - 1].EventId);
            }
            else
            {
                return;
            }
        }
    }

}
