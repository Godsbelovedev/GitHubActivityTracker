namespace GitHubActivity.Application.Interfaces
{
    public interface IGitHubServiceInterface
    {
        void AddGitHubActivity(List<GitHubEvent> gitHubEvents);
        void DeleteEvent(string id);
        void GetGitHubEvent(string id);
        void GetGitHubEventsByType(string searchTerm);
        void GetGitHubEventsByRepoName(string searchTerm);
        void GetGitHubEventsByUserName(string searchTerm);
        Task<List<GitHubEvent>> FetchEventsFromGitHub(string username);
    }
}