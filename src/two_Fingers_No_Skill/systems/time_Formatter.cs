using System.Globalization;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Turns a duration into the text shown by the match timer.
/// </summary>
public static class time_Formatter
{
    /// <summary>
    /// Formats seconds as "MM:SS" (minutes keep counting past 59). Partial seconds are dropped.
    /// </summary>
    public static string format_Duration(double seconds)
    {
        var whole_Seconds = (int)Math.Floor(Math.Max(0, seconds));
        return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", whole_Seconds / 60, whole_Seconds % 60);
    }
}
