namespace Yrhacks2023.Shared;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public byte[] Password { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
}