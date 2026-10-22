using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.entities;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the enemy spawn timing.
/// </summary>
public class enemy_Spawner_Tests
{
    /// <summary>
    /// Builds a tier with only the values the spawner reads.
    /// </summary>
    private static difficulty_Tier create_Tier(double interval, int max_Enemies) => new()
    {
        spawn_Interval_Seconds = interval,
        max_Enemies_On_Screen = max_Enemies,
        enemy_Speed = 100,
    };

    [Fact]
    public void spawner_Waits_One_Interval_Before_First_Spawn()
    {
        var spawner = new enemy_Spawner(new Random(1));
        var tier = create_Tier(1.6, 3);

        Assert.False(spawner.try_Spawn(1.5, tier, 0, out _));
        Assert.True(spawner.try_Spawn(0.1, tier, 0, out _));
    }

    [Fact]
    public void spawner_Holds_When_Max_Enemies_Reached_Then_Spawns_As_Soon_As_A_Slot_Frees()
    {
        var spawner = new enemy_Spawner(new Random(1));
        var tier = create_Tier(1.6, 3);

        Assert.False(spawner.try_Spawn(5.0, tier, 3, out _));
        Assert.True(spawner.try_Spawn(0.0, tier, 2, out _));
    }

    [Fact]
    public void spawner_Spawns_At_Most_One_Enemy_Per_Call()
    {
        var spawner = new enemy_Spawner(new Random(1));
        var tier = create_Tier(0.45, 8);

        Assert.True(spawner.try_Spawn(10.0, tier, 0, out _));
        Assert.False(spawner.try_Spawn(0.0, tier, 1, out _));
    }

    [Fact]
    public void spawn_Side_Uses_Both_Sides()
    {
        var spawner = new enemy_Spawner(new Random(42));
        var tier = create_Tier(1.0, 8);
        var sides = new HashSet<facing_Direction>();

        for (var index = 0; index < 200; index++)
        {
            if (spawner.try_Spawn(1.0, tier, 0, out var side))
            {
                sides.Add(side);
            }
        }

        Assert.Equal(2, sides.Count);
    }
}
