using System.Text.Json.Serialization;

namespace GitHubActivity
{
    public class Commit
    {
        public string CommitAuthor { get; set; } = default!;
        public string CommitMessage { get; set; } = default!;
        public DateTimeOffset CommitDate { get; set;}
        public Commit(string commitAuthor, string commitMessage, DateTimeOffset commitDate)
        {
            CommitAuthor = commitAuthor;
            CommitMessage = commitMessage;
            CommitDate = commitDate;
        }
    }
}