using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for score computation.
/// </summary>
public class score_Manager_Tests
{
    /// <summary>
    /// Creates a score manager with the default rules.
    /// </summary>
    private static score_Manager create_Score()
    {
        var settings = game_Settings.create_Default();
        return new score_Manager(settings.score_Rules, settings.minimum_Score);
    }

    /// <summary>
    /// Feeds the same event type to the score manager several times.
    /// </summary>
    private static void send(score_Manager score, game_Event_Type event_Type, int count = 1)
    {
        for (var index = 0; index < count; index++)
        {
            score.handle_Event(new game_Event(event_Type, 0));
        }
    }

    [Fact]
    public void enemy_Killed_Adds_Twenty_Points()
    {
        var score = create_Score();

        send(score, game_Event_Type.enemy_Killed);

        Assert.Equal(20, score.current_Score);
    }

    [Fact]
    public void missed_Hit_Removes_Fifty_Points()
    {
        var score = create_Score();

        send(score, game_Event_Type.enemy_Killed, 3);
        send(score, game_Event_Type.missed_Hit);

        Assert.Equal(10, score.current_Score);
    }

    [Fact]
    public void rhythm_Increase_Gives_No_Points()
    {
        var score = create_Score();

        send(score, game_Event_Type.enemy_Killed);
        send(score, game_Event_Type.rhythm_Increased);

        Assert.Equal(20, score.current_Score);
    }

    [Fact]
    public void score_Never_Goes_Below_Minimum()
    {
        var score = create_Score();

        send(score, game_Event_Type.enemy_Killed);
        send(score, game_Event_Type.missed_Hit);
        Assert.Equal(0, score.current_Score);

        send(score, game_Event_Type.missed_Hit);
        Assert.Equal(0, score.current_Score);
    }
}
