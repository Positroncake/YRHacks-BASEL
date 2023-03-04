namespace Yrhacks2023.Shared.Data;

public enum ItemType
{
    [UserString("CPU")]
    Cpu = 0,
    [UserString("GPU")]
    Gpu = 1,
    [UserString("RAM")]
    Ram = 2,
    [UserString("Waifu Body Pillow")]
    WaifuBodyPillow = 3,
    [UserString("Motherboard")]
    Motherboard = 4,
    [UserString("Computer Case")]
    Case = 5,
    [UserString("HDD")]
    Hdd = 6,
    [UserString("SSD")]
    Ssd = 7,
    [UserString("Mouse")]
    Mouse = 8,
    [UserString("Keyboard")]
    Keyboard = 9,
    [UserString("Monitor")]
    Monitor = 10,
    [UserString("Cooler")]
    Cooler = 11,
    [UserString("Case Fan")]
    CaseFan = 12,
    [UserString("Phone")]
    Phone = 13
}

public static class ItemTypeExtensions
{
    public static string ToUserString(this ItemType type)
    {
        return UserStringHelpers.LookupUserString(type);
    }
}

