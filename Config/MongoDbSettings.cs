namespace bugtracker.Config
{
  public class MongoDbSettings{
    public string Env { get; set; }
    public string Host { get; set; }
    public string Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string ConnectionString { 
      get {
        return $"{Env}://{User}:{Password}@{Host}{Port}";
      } 
    }
  }
}