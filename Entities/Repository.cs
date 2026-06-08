namespace GitHubActivity
{
    public class Repository
    {
        public string Id { get; set; } = default!;
        public string RepositoryName { get; set; }  = default!;
        public Repository(string id, string repositoryName)
        {
            Id = id;
            RepositoryName = repositoryName;
        }
    }
}