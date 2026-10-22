using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the end-of-match statistics.
/// </summary>
public class match_Stats_Tests
{
    [Fact]
    public void stats_Count_Each_Event_Type()
    {
        var stats = new match_Stats();

        stats.handle_Event(new game_Event(game_Event_Type.enemy_Killed, 0));
        stats.handle_Event(new game_Event(game_Event_Type.enemy_Killed, 0));
        stats.handle_Event(new game_Event(game_Event_Type.missed_Hit, 0));
        stats.handle_Event(new game_Event(game_Event_Type.life_Lost, 0));

        Assert.Equal(2, stats.enemies_Killed);
        Assert.Equal(1, stats.missed_Hits);
        Assert.Equal(1, stats.lives_Lost);
    }
}
