namespace Yrhacks2023.Shared.Data;

public enum ItemCondition
{  
    [UserString("Good")]
    Good = 1,
    [UserString("Fair")]
    Fair = 0,
    [UserString("Poor")]
    Poor = -1,
}

public static class ItemConditionExtensions
{
    public static string ToUserString(this ItemCondition condition)
    {
        return UserStringHelpers.LookupUserString(condition);
    }
}
