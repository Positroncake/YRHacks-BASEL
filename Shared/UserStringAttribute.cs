namespace Yrhacks2023.Shared;

[AttributeUsage(AttributeTargets.Field)]
public class UserStringAttribute : Attribute
{
    public UserStringAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; }
}

public static class UserStringHelpers
{
    public static string LookupUserString<T>(T x) where T : Enum
    {
        var f = typeof(T).GetField(x.ToString());
        foreach (var attr in f!.GetCustomAttributes(typeof(UserStringAttribute), false))
        {
            return ((UserStringAttribute) attr).Name;
        }

        throw new ArgumentException("Invalid enum value", nameof(x));
    }
}