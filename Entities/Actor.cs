namespace GitHubActivity
{
    public class Actor
    {
        public Actor(string id, string login)
        {
           Id = id;
           Login = login;
        }
        public string Id { get; set; } = default!;
        public string Login { get; set; } = default!;
    }
}