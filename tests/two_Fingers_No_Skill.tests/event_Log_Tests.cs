using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the on-screen event log.
/// </summary>
public class event_Log_Tests
{
    [Theory]
    [InlineData(game_Event_Type.enemy_Killed, 20, "Ennemi tué (+20)")]
    [InlineData(game_Event_Type.missed_Hit, -50, "Coup raté (-50)")]
    [InlineData(game_Event_Type.life_Lost, 0, "Vie perdue")]
    public void build_Text_Uses_French_Label_And_Rule_Points(game_Event_Type event_Type, int points, string expected)
    {
        Assert.Equal(expected, event_Log.build_Text(event_Type, points));
    }

    [Fact]
    public void rhythm_Increase_Shows_The_New_Percent()
    {
        var log = new event_Log(6, game_Settings.create_Default().score_Rules);

        log.handle_Event(new game_Event(game_Event_Type.rhythm_Increased, 90, 111));

        Assert.Equal("Rythme : 111 %", log.entries[0].text);
        Assert.Equal(game_Event_Type.rhythm_Increased, log.entries[0].event_Type);
    }

    [Fact]
    public void log_Keeps_Newest_Last_And_Drops_Oldest_Beyond_Max()
    {
        var log = new event_Log(6, game_Settings.create_Default().score_Rules);

        for (var index = 0; index < 7; index++)
        {
            log.handle_Event(new game_Event(game_Event_Type.enemy_Killed, index));
        }
        log.handle_Event(new game_Event(game_Event_Type.life_Lost, 8));

        Assert.Equal(6, log.entries.Count);
        Assert.Equal(game_Event_Type.life_Lost, log.entries[^1].event_Type);
        Assert.Equal("Vie perdue", log.entries[^1].text);
    }

    [Fact]
    public void log_Shows_Rule_Points_Even_When_Score_Is_Zero()
    {
        var log = new event_Log(6, game_Settings.create_Default().score_Rules);

        log.handle_Event(new game_Event(game_Event_Type.missed_Hit, 0));

        Assert.Equal("Coup raté (-50)", log.entries[0].text);
    }
}
