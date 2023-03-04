namespace Yrhacks2023.Shared.Requests;

public class RemoveBuildListRequest
{
    public ulong Account { set; get; }
    public ulong ListId { set; get; }
    public ulong ProductId { set; get; }
}