namespace Yrhacks2023.Shared.Requests;

public class BuildListModificationRequest
{
    public string Token { set; get; } = string.Empty;
    public ulong ListId { set; get; }
    public ulong ProductId { set; get; }
}