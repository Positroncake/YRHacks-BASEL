namespace Yrhacks2023.Shared.Data;

public class BuildListEntry
{
    public ulong ListId { set; get; }
    public ulong? ProductId { set; get; }
    public DateTime CreationTime { set; get; }
    public DateTime ModificationTime { set; get; }
    public ulong? OwnerAccount { set; get; }
}