using two_Fingers_No_Skill.entities;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the player entity.
/// </summary>
public class player_Tests
{
    [Fact]
    public void player_Faces_Left_By_Default()
    {
        Assert.Equal(facing_Direction.left, new player(640, 20).facing);
    }

    [Fact]
    public void lockout_Expires_After_Duration()
    {
        var hero = new player(640, 20);
        hero.start_Lockout(0.3);

        hero.update(0.2);
        Assert.True(hero.is_Locked_Out);

        hero.update(0.1);
        Assert.False(hero.is_Locked_Out);
    }
}
