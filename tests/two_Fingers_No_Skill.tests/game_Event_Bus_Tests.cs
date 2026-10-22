using two_Fingers_No_Skill.core;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the gameplay event bus.
/// </summary>
public class game_Event_Bus_Tests
{
    [Fact]
    public void publish_Notifies_All_Subscribers()
    {
        var bus = new game_Event_Bus();
        var received = new List<game_Event>();
        bus.event_Published += received.Add;
        bus.event_Published += received.Add;
        var published = new game_Event(game_Event_Type.enemy_Killed, 12.5);

        bus.publish(published);

        Assert.Equal(new[] { published, published }, received);
    }
}
