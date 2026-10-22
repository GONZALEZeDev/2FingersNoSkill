using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the match clock and its slow motion.
/// </summary>
public class game_Clock_Tests
{
    [Fact]
    public void advance_Without_Slow_Motion_Returns_Real_Delta()
    {
        var clock = new game_Clock(0.05f);

        Assert.Equal(1.0, clock.advance(1.0), 6);
        Assert.Equal(1.0, clock.elapsed_Game_Seconds, 6);
    }

    [Fact]
    public void slow_Motion_Scales_Delta_And_Expires_In_Real_Time()
    {
        var clock = new game_Clock(0.05f);
        clock.start_Slow_Motion(1.25);

        Assert.Equal(0.05, clock.advance(1.0), 6);
        Assert.True(clock.is_Slow_Motion_Active);
        Assert.Equal(0.015, clock.advance(0.3), 6);
        Assert.False(clock.is_Slow_Motion_Active);
        Assert.Equal(1.0, clock.advance(1.0), 6);
        Assert.Equal(1.065, clock.elapsed_Game_Seconds, 6);
    }

    [Fact]
    public void restarting_Slow_Motion_Resets_Remaining_Duration()
    {
        var clock = new game_Clock(0.05f);
        clock.start_Slow_Motion(1.0);
        clock.advance(0.8);

        clock.start_Slow_Motion(1.0);
        clock.advance(0.5);

        Assert.True(clock.is_Slow_Motion_Active);
    }
}
