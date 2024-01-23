namespace AutoResultAmendment.Models;

public class AppSettings
{
    public string[] SiteDbServer { get; set; } = { };
    public string[] EmailAddress { get; set; }
    public string EnvironmentName { get; set; }
}