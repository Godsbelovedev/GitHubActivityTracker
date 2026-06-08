using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using GitHubActivity.Infrastructure.Interfaces;

namespace GitHubActivity.Infrastructure.Implementation
{
    public class GitEventRepository : IGitEventRepositoryInterface
    {
        private readonly string _basePath;
        private readonly string _eventsFolderPath;
        private readonly string _eventFilePath;
        private readonly HttpClient _httpClient;
        public GitEventRepository()
        {
            _basePath = Path.Combine(Environment.GetFolderPath
            (Environment.SpecialFolder.LocalApplicationData), "GitHubActivity");

            _eventsFolderPath = Path.Combine(_basePath, "GitEvents");

            _eventFilePath = Path.Combine(_eventsFolderPath, "events.json");
            _httpClient = new HttpClient();
        }

        private void WriteEventsToFile(List<GitHubEvent> fetchedEvents)
        {
            try
            {
                if (!Directory.Exists(_eventsFolderPath))
                {
                    Directory.CreateDirectory(_eventsFolderPath);
                }
                var eventsInJson = JsonSerializer.Serialize(fetchedEvents, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_eventFilePath, eventsInJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"write to file failed: {ex.Message}");
            }

        }
        private List<GitHubEvent> ReadEventsFromFile()
        {
            if (!File.Exists(_eventFilePath)) return new List<GitHubEvent>();

            var jsonData = File.ReadAllText(_eventFilePath);
            if (string.IsNullOrEmpty(jsonData))
            {
                return new List<GitHubEvent>();
            }

            var response = JsonSerializer.Deserialize<List<GitHubEvent>>(jsonData);
            if (response == null || !response.Any())
            {
                return new List<GitHubEvent>();
            }
            return response;
        }
        public bool AddGitHubActivity(GitHubEvent gitHubEvent)
        {
            try
            {
                var listOfEvents = ReadEventsFromFile();
                listOfEvents.Add(gitHubEvent);
                WriteEventsToFile(listOfEvents);
                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool DeleteEvent(string id)
        {

            try
            {

                var listOfEvents = ReadEventsFromFile();
                if (listOfEvents == null || !listOfEvents.Any())
                {
                    return false;
                }
                for (int i = 0; i < listOfEvents.Count; i++)
                {
                    if (listOfEvents[i].EventId == id)
                    {
                        listOfEvents.RemoveAt(i);
                        WriteEventsToFile(listOfEvents);
                        return true;
                    }

                }

                return false;
            }
            catch
            {
                return false;
            }
        }



        public GitHubEvent? GetGitHubEvent(string id)
        {
            try
            {
                var listOfEvents = ReadEventsFromFile();
                if (listOfEvents == null || !listOfEvents.Any())
                {
                    return null;
                }
                foreach (var gitEvent in listOfEvents)
                {
                    if (gitEvent.EventId == id)
                    {
                        return gitEvent;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<GitHubEvent> GetAllGitHubEventsByRepoName(string searchTerm)
        {
            var listOfEvents = ReadEventsFromFile();
            var searchedEvents = new List<GitHubEvent>();
            try
            {
                if (listOfEvents == null || !listOfEvents.Any())
                {
                    return searchedEvents;
                }
                foreach (var gitEvent in listOfEvents)
                {
                    var gitEventRepoName = gitEvent.Repo?.RepositoryName;
                    if (gitEventRepoName != null && gitEventRepoName.Contains(searchTerm))
                    {
                        searchedEvents.Add(gitEvent);
                    }
                }
                return searchedEvents;
            }
            catch
            {
                return searchedEvents;
            }
        }

        public List<GitHubEvent> GetAllGitHubEventsByType(string searchTerm)
        {
            var listOfEvents = ReadEventsFromFile();
            var searchedEvents = new List<GitHubEvent>();
            try
            {
                if (listOfEvents == null || !listOfEvents.Any())
                {
                    return searchedEvents;
                }
                foreach (var gitEvent in listOfEvents)
                {
                    if (gitEvent.EventType.Contains(searchTerm))
                    {
                        searchedEvents.Add(gitEvent);
                    }
                }
                return searchedEvents;
            }
            catch
            {
                return searchedEvents;
            }
        }

        public List<GitHubEvent> GetAllGitHubEventsByUserName(string searchTerm)
        {
            var listOfEvents = ReadEventsFromFile();
            var searchedEvents = new List<GitHubEvent>();
            try
            {
                if (listOfEvents == null || !listOfEvents.Any())
                {
                    return searchedEvents;
                }
                foreach (var gitEvent in listOfEvents)
                {
                    var gitEventActorName = gitEvent.Actor?.Login;
                    if (gitEventActorName != null && gitEventActorName.Contains(searchTerm))
                    {
                        searchedEvents.Add(gitEvent);
                    }
                }
                return searchedEvents;
            }
            catch
            {
                return searchedEvents;
            }
        }

        public async Task<string> FetchEventsFromGitHub(string username)
        {
            var url = $"https://api.github.com/users/{username}/events/public";
            try
            {
                
                if(!_httpClient.DefaultRequestHeaders.UserAgent.Any())
                {
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyGitHubActivityApp");
                }
                
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Username does not exixt or API issue encountered.");
                    return "[]";
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch(HttpRequestException )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[NETWORK ERROR]. Could not connect to Git Hub, please verify your internet connections");
                Console.ResetColor();
                return "[]";
            }


        }
        public List<GitHubEvent> GetAllStoredGitHubEvents()
        {
            var allEvents = ReadEventsFromFile();
            if (allEvents == null || !allEvents.Any())
            {
                return new List<GitHubEvent>();
            }
            return allEvents;
        }



    }
}