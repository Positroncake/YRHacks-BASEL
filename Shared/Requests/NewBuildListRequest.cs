namespace Yrhacks2023.Shared.Requests;

public class NewBuildListRequest
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;
}