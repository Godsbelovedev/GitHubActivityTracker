namespace GitHubActivity.Infrastructure.Interfaces
{
    public interface IGitEventRepositoryInterface
    {
         bool AddGitHubActivity(GitHubEvent gitHubEvent);
         bool DeleteEvent(string id);
         GitHubEvent? GetGitHubEvent(string id);
         List<GitHubEvent> GetAllGitHubEventsByType(string searchTerm);
         List<GitHubEvent> GetAllStoredGitHubEvents();
         List<GitHubEvent> GetAllGitHubEventsByRepoName(string searchTerm);
         List<GitHubEvent> GetAllGitHubEventsByUserName(string searchTerm);
         Task<string> FetchEventsFromGitHub(string username);
    }
}