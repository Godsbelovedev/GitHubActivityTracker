using System.Text.Json;
using GitHubActivity.Application.Interfaces;
using GitHubActivity.ConsoleHelper;
using GitHubActivity.Infrastructure.Implementation;
using GitHubActivity.Infrastructure.Interfaces;

namespace GitHubActivity.Application.Implementation
{
    public class GitHubService : IGitHubServiceInterface
    {
        private readonly IGitEventRepositoryInterface _gitHub;
        private readonly HttpClient _httpClient;
        public GitHubService()
        {
            _gitHub = new GitEventRepository();
            _httpClient = new HttpClient();
        }
        public void AddGitHubActivity(List<GitHubEvent> gitHubEvents)
        {
            bool isExistingEvent = false;
            var allEvents = _gitHub.GetAllStoredGitHubEvents();
            for (int i = 0; i < allEvents.Count; i++)
            {
                if (allEvents[i].EventId == gitHubEvents[i].EventId)
                {
                    allEvents[i] = gitHubEvents[i];
                    isExistingEvent = true;
                    continue;
                }
                if (isExistingEvent == false)
                {
                    _gitHub.AddGitHubActivity(gitHubEvents[i]);
                }
            }

        }

        public void DeleteEvent(string id)
        {
            var eventToDelete = _gitHub.DeleteEvent(id);
            if (eventToDelete == false)
            {
                Console.WriteLine("delete fail"); return;
            }
            Console.WriteLine("Delete Successful");
            return;
        }

        public async Task<List<GitHubEvent>> FetchEventsFromGitHub(string username)
        {
            var events = await _gitHub.FetchEventsFromGitHub(username);

            var eventInJsonElement = JsonSerializer.Deserialize<JsonElement>(events);
            var root = eventInJsonElement.EnumerateArray();

            string? head = null;
            string? before = null;
            string? eventId = null;
            string? eventType = null;
            DateTimeOffset? createdAt = null;
            var listOfGitHubEvents = new List<GitHubEvent>();

            foreach (var item in root)
            {
                var repo = ObjectsExtractor.RepositoryExtractor(item);
                if (repo == null)
                {
                    Console.WriteLine("repository is not found");
                    continue;
                }

                var actor = ObjectsExtractor.ActorExtractor(item);
                if (actor == null)
                {
                    Console.WriteLine("actor is not found");
                    continue;
                }

                if (item.TryGetProperty("type", out var jsonEventType))
                {
                    var type = jsonEventType.GetString();
                    if (type != null)
                    {
                        eventType = type;
                    }
                }
                else
                {
                    Console.WriteLine("Type unknown");
                }

                if (item.TryGetProperty("id", out var jsonId))
                {
                    var id = jsonId.GetString();
                    if (id != null)
                    {
                        eventId = id ?? "Nil";
                    }
                }
                else
                {
                    Console.WriteLine("unknown Id");
                }

                if (item.TryGetProperty("created_at", out var jsonCreatedAt))
                {
                    var timeCreated = jsonCreatedAt.GetString();
                    if (timeCreated != null)
                    {
                        createdAt = DateTimeOffset.Parse(timeCreated);
                    }
                }
                else
                {
                    Console.WriteLine("date created unknown");
                }

                if (item.TryGetProperty("payload", out var jsonPayload)
                   && jsonPayload.TryGetProperty("head", out var payloadHeadInJson)
                   && jsonPayload.TryGetProperty("before", out var payloadbeforeInJson))
                {
                    var payloadHead = payloadHeadInJson.GetString();
                    var payloadbefore = payloadbeforeInJson.GetString();
                    if (payloadHead != null && payloadbefore != null)
                    {
                        head = payloadHead;
                        before = payloadbefore;
                    }
                    else
                    {
                        Console.WriteLine("'head' and or 'before' from the payload not found");
                    }
                }

                if (string.Equals("PushEvent", eventType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var commits = new List<Commit>();
                    var commiturl = $"https://api.github.com/repos/{repo?.RepositoryName}/compare/{before}...{head}";
                    try
                    {
                        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyGitHubActivityApp");
                        var commitData = await _httpClient.GetAsync(commiturl);

                        if (commitData.IsSuccessStatusCode)
                        {
                            var commitInString = await commitData.Content.ReadAsStringAsync();

                            commits = ObjectsExtractor.CommitExtractor(commitInString);
                        }

                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Notice] Failed to fetch commits for push event. Status: {commitData.StatusCode}");
                            Console.ResetColor();
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Network Warning] Error fetching commit payload: {ex.Message}");
                        Console.ResetColor();
                    }
                    listOfGitHubEvents.Add(new GitHubEvent
                    (eventId ?? "nil", eventType, repo, actor, commits, createdAt ?? default));

                }
                else
                {
                    var commits = new List<Commit>();
                    listOfGitHubEvents.Add(new GitHubEvent
                    (eventId ?? "nil", eventType, repo, actor, commits, createdAt ?? default));

                }

            }
            return listOfGitHubEvents;
        }

        public void GetGitHubEvent(string id)
        {

            var eventToView = _gitHub.GetGitHubEvent(id);
            if (eventToView == null)
            {
                Console.WriteLine("no event found");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=======================================================");
            Console.WriteLine($"📊   TRACKER REPORT: FOUND {eventToView.Actor?.Login ?? "NO"} GITHUB EVENTS");
            Console.WriteLine("=======================================================");
            Console.ResetColor();

            if (eventToView == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" No recent events found for this user.");
                Console.ResetColor();
                return;
            }

            string localTimeStr = eventToView.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n📌 [EVENT ID: {eventToView.EventId}] -> {eventToView.EventType.ToUpper()}");
            Console.ResetColor();

            Console.WriteLine($"   📅 Time (Local): {localTimeStr}");

            if (eventToView.Actor != null)
            {
                Console.WriteLine($"   👤 Actor: {eventToView.Actor.Login} (ID: {eventToView.Actor.Id})");
            }
            else
            {
                Console.WriteLine("   👤 Actor: Anonymous / Nil");
            }

            if (eventToView.Repo != null)
            {
                Console.WriteLine($"   📁 Repo:  {eventToView.Repo.RepositoryName} (ID: {eventToView.Repo.Id})");
            }
            else
            {
                Console.WriteLine("   📁 Repo:  Unknown Repository / Nil");
            }


            if (eventToView.Commits != null && eventToView.Commits.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("   📝 Commits Attached:");
                Console.ResetColor();

                int count = 1;
                foreach (var commit in eventToView.Commits)
                {
                    string commitTime = commit.CommitDate.ToLocalTime().ToString("HH:mm:ss");

                    Console.WriteLine($" {count}. ⚡ [{commitTime ?? "nil"}] {commit.CommitAuthor ?? "nil"}: {commit.CommitMessage ?? "nil"}");
                    count = +1;
                }
            }
            else if (eventToView.EventType.Equals("PushEvent", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("   📝 Commits: No commit messages available in this payload.");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', 55));
            Console.ResetColor();

        }

        public void GetGitHubEventsByRepoName(string searchTerm)
        {
            var events = _gitHub.GetAllGitHubEventsByRepoName(searchTerm);
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

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
                Console.WriteLine(Environment.NewLine);
            }


        }

        public void GetGitHubEventsByType(string searchTerm)
        {
            var events = _gitHub.GetAllGitHubEventsByType(searchTerm);
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

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
                Console.WriteLine(Environment.NewLine);
            }


        }

        public void GetGitHubEventsByUserName(string searchTerm)
        {
            var events = _gitHub.GetAllGitHubEventsByUserName(searchTerm);
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

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
                Console.WriteLine(Environment.NewLine);
            }
        }

    }
}