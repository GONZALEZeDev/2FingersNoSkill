using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the match timer text.
/// </summary>
public class time_Formatter_Tests
{
    [Theory]
    [InlineData(0, "00:00")]
    [InlineData(59.99, "00:59")]
    [InlineData(90, "01:30")]
    [InlineData(599.9, "09:59")]
    [InlineData(3600, "60:00")]
    public void format_Duration_Shows_Minutes_And_Seconds(double seconds, string expected)
    {
        Assert.Equal(expected, time_Formatter.format_Duration(seconds));
    }
}
