using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for picking the active difficulty tier.
/// </summary>
public class difficulty_Manager_Tests
{
    /// <summary>
    /// Creates a manager with the default tiers.
    /// </summary>
    private static difficulty_Manager create_Manager() => new(game_Settings.create_Default().difficulty_Tiers);

    [Theory]
    [InlineData(0, 0)]
    [InlineData(89.9, 0)]
    [InlineData(90, 1)]
    [InlineData(180, 2)]
    [InlineData(269.9, 2)]
    [InlineData(270, 3)]
    [InlineData(540, 6)]
    [InlineData(5000, 6)]
    public void tier_Index_Follows_Ninety_Second_Steps(double elapsed_Seconds, int expected_Index)
    {
        Assert.Equal(expected_Index, create_Manager().get_Tier_Index_At(elapsed_Seconds));
    }

    [Fact]
    public void slow_Motion_Duration_Per_Tier_Matches_Spec()
    {
        var manager = create_Manager();
        var times = new[] { 0d, 90, 180, 270, 360, 450, 540 };

        var durations = times.Select(time => manager.get_Tier_At(time).slow_Motion_Duration_Seconds);

        Assert.Equal(new[] { 1.25, 1.25, 1.25, 1.15, 1.05, 0.95, 0.85 }, durations);
    }
}
