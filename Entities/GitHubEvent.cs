using System.Text.Json.Serialization;

namespace GitHubActivity
{
    public class GitHubEvent
    {
        public string EventId { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public Repository? Repo { get; set; }
        public Actor? Actor { get; set; }
         public List<Commit> Commits { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public GitHubEvent(string eventId, string eventType, Repository? repo, 
                         Actor? actor, List<Commit> commits, DateTimeOffset createdAt)
        {
            EventId = eventId;
            EventType = eventType;
            Repo = repo;
            Actor = actor;
            Commits = commits;
            CreatedAt = createdAt;
        }
    }
}