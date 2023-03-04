namespace Yrhacks2023.Shared.Data;

public class BuildListReply
{
    public ulong ListId { set; get; }
    public List<ulong> ProductIds { set; get; }
    public DateTime CreationTime { set; get; }
    public DateTime ModificationTime { set; get; }
    public ulong? OwnerAccount { set; get; }
}