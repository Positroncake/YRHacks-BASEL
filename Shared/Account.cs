namespace Yrhacks2023.Shared;

public class Account
{
    public ulong Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>(); // 128 Bytes (1024-bit)
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Joined { get; set; }
}