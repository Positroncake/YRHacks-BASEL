namespace Yrhacks2023.Shared.Data;

public class BuildList
{
    public ulong ListId { set; get; }
    public string Name { get; set; }
    public List<ulong> ProductIds { set; get; }
    public DateTime CreationTime { set; get; }
    public DateTime ModificationTime { set; get; }
    public string OwnerAccount { set; get; } = string.Empty;
    public bool IsPublic { get; set; }
}