using System.Text.Json;

namespace GitHubActivity.ConsoleHelper
{
    public class ObjectsExtractor
    {
        public static Actor? ActorExtractor(JsonElement jsonElement)
        {
            Actor? actor = null;
            if (jsonElement.TryGetProperty("actor", out var eventActor)
                   && eventActor.TryGetProperty("id", out var actorId)
                   && eventActor.TryGetProperty("login", out var actorName))
            {
                var id = actorId.ValueKind == JsonValueKind.Number ?
                 actorId.GetInt64().ToString() : actorId.GetString();

                var name = actorName.GetString();
                if (name != null || id != null)
                {
                    actor = new Actor(id ?? "Nil", name ?? "Nil");
                }
            }
            if (actor != null)
            {
                return actor;
            }
            return null;
        }
        public static List<Commit> CommitExtractor(string commitInString)
        {
            List<Commit> commits = new List<Commit>();
            var CommitsInJsonElement = JsonSerializer.Deserialize<JsonElement>(commitInString);



            if (CommitsInJsonElement.TryGetProperty("commits", out var arrayOfCommitsInJsonElement))
                foreach (var item in arrayOfCommitsInJsonElement.EnumerateArray())
                {
                    string? message = null;
                    DateTimeOffset? date = null;
                    string? authorName = null;
                    if (item.TryGetProperty("commit", out var commit)
                       && commit.TryGetProperty("message", out var messageInJsonElement)
                       && commit.TryGetProperty("author", out var authorInJson)
                       && authorInJson.TryGetProperty("name", out var authorNameInJsonElement)
                       && authorInJson.TryGetProperty("date", out var dateInJsonElement))
                    {
                        var messageInString = messageInJsonElement.GetString();
                        var authorNameInString = authorNameInJsonElement.GetString();
                        var dateInString = dateInJsonElement.GetString();

                        if (messageInString != null
                            || authorNameInString != null)
                        {
                            message = messageInString;
                            authorName = authorNameInString;
                        }
                        if (dateInString != null)
                        {
                            date = DateTimeOffset.Parse(dateInString);
                        }
                    }
                    var eventCommit = new Commit(authorName ?? "Nil", message ?? "Nil", date ?? default);
                    commits.Add(eventCommit);
                }

            if (commits != null && commits.Count > 0)
            {
                return commits;
            }
            else
            {
                return new List<Commit>();
            }
        }
        public static Repository? RepositoryExtractor(JsonElement jsonElement)
        {
            Repository? repo = null;
            if (jsonElement.TryGetProperty("repo", out var eventRepo)
                   && eventRepo.TryGetProperty("id", out var repoId)
                   && eventRepo.TryGetProperty("name", out var repoName))
            {
                var id = repoId.ValueKind == JsonValueKind.Number ?
                          repoId.GetInt64().ToString() : repoId.GetString();
                var name = repoName.GetString();
                if (name != null || id != null)
                {
                    repo = new Repository(id ?? "Nil", name ?? "Nil");
                }
            }
            if (repo != null)
            {
                return repo;
            }
            return null;
        }
    }
}