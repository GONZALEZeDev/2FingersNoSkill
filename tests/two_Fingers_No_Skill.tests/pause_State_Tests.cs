using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.game_States;
using two_Fingers_No_Skill.input;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for pausing and resuming a match with the Space bar.
/// </summary>
public class pause_State_Tests
{
    private static readonly input_Snapshot space_Press = new(false, false, false, false, pause_Pressed: true);
    private static readonly input_Snapshot left_Click = new(true, false, false, false);

    /// <summary>
    /// Starts a match and returns the shared services and the playing screen.
    /// </summary>
    private static (game_Services services, playing_State playing) start_Match()
    {
        var services = new game_Services(game_Settings.create_Default(), new game_State_Machine(), new Random(1));
        var playing = new playing_State(services);
        services.state_Machine.change_State(playing);
        return (services, playing);
    }

    [Fact]
    public void space_During_A_Match_Opens_The_Pause_Screen()
    {
        var (services, playing) = start_Match();

        playing.update(0.016, space_Press);

        Assert.IsType<pause_State>(services.state_Machine.current_State);
    }

    [Fact]
    public void match_Is_Frozen_And_Strikes_Ignored_While_Paused()
    {
        var (services, playing) = start_Match();
        playing.update(0.016, space_Press);
        var elapsed_Before = playing.world.clock.elapsed_Game_Seconds;

        for (var index = 0; index < 100; index++)
        {
            services.state_Machine.update(0.1, left_Click);
        }

        Assert.Equal(elapsed_Before, playing.world.clock.elapsed_Game_Seconds);
        Assert.Equal(0, playing.world.stats.missed_Hits);
    }

    [Fact]
    public void space_While_Paused_Resumes_The_Same_Match()
    {
        var (services, playing) = start_Match();
        playing.update(0.016, space_Press);

        services.state_Machine.update(0.016, space_Press);
        for (var index = 0; index < 5; index++)
        {
            services.state_Machine.update(0.1, input_Snapshot.empty);
        }

        Assert.Same(playing, services.state_Machine.current_State);
        Assert.True(playing.world.clock.elapsed_Game_Seconds > 0.4);
    }
}
