using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.entities;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for the full match logic (strikes, contacts, lives, slow motion, tiers).
/// </summary>
public class match_World_Tests
{
    private const double frame_Seconds = 0.016;

    /// <summary>
    /// Creates a match with the default settings and a fixed random seed.
    /// </summary>
    private static match_World create_World() => new(game_Settings.create_Default(), new Random(1));

    /// <summary>
    /// Adds an enemy whose facing edge is <paramref name="gap"/> pixels from the player.
    /// </summary>
    private static enemy add_Enemy_At_Gap(match_World world, facing_Direction side, float gap)
    {
        var player_Edge = side == facing_Direction.left
            ? world.player.center_X - world.player.half_Width - gap - world_Layout.enemy_Half_Width
            : world.player.center_X + world.player.half_Width + gap + world_Layout.enemy_Half_Width;
        var new_Enemy = new enemy(side, player_Edge, world_Layout.enemy_Half_Width, 140);
        world.add_Enemy_For_Tests(new_Enemy);
        return new_Enemy;
    }

    [Fact]
    public void new_Match_Has_Three_Lives_Zero_Score_Player_Facing_Left()
    {
        var world = create_World();

        Assert.Equal(3, world.current_Lives);
        Assert.Equal(0, world.score.current_Score);
        Assert.Equal(facing_Direction.left, world.player.facing);
        Assert.Empty(world.enemies);
        Assert.False(world.is_Game_Over);
    }

    [Fact]
    public void strike_On_Enemy_In_Range_Kills_It_And_Scores()
    {
        var world = create_World();
        add_Enemy_At_Gap(world, facing_Direction.left, 50);

        world.handle_Strike(facing_Direction.left);

        Assert.Empty(world.enemies);
        Assert.Equal(20, world.score.current_Score);
        Assert.Equal(1, world.stats.enemies_Killed);
        Assert.Equal("Ennemi tué (+20)", world.event_Log.entries[^1].text);
        Assert.Equal(facing_Direction.left, world.player.facing);
    }

    [Fact]
    public void strike_With_No_Enemy_In_Range_Is_A_Miss()
    {
        var world = create_World();

        world.handle_Strike(facing_Direction.right);

        Assert.Equal(facing_Direction.right, world.player.facing);
        Assert.Equal(1, world.stats.missed_Hits);
        Assert.Equal(0, world.score.current_Score);
        Assert.True(world.player.is_Locked_Out);
        Assert.Equal("Coup raté (-50)", world.event_Log.entries[^1].text);
    }

    [Fact]
    public void strikes_Are_Ignored_During_Miss_Lockout()
    {
        var world = create_World();
        world.handle_Strike(facing_Direction.right);
        var target = add_Enemy_At_Gap(world, facing_Direction.left, 50);

        world.handle_Strike(facing_Direction.left);

        Assert.Contains(target, world.enemies);
        Assert.Equal(facing_Direction.right, world.player.facing);
    }

    [Fact]
    public void both_Strikes_Same_Frame_Second_Ignored_After_Miss()
    {
        var world = create_World();
        var right_Enemy = add_Enemy_At_Gap(world, facing_Direction.right, 50);

        world.handle_Strike(facing_Direction.left);
        world.handle_Strike(facing_Direction.right);

        Assert.Contains(right_Enemy, world.enemies);
    }

    [Fact]
    public void enemy_Contact_Costs_A_Life_Removes_Enemy_And_Starts_Slow_Motion()
    {
        var world = create_World();
        add_Enemy_At_Gap(world, facing_Direction.left, 0);

        world.update(frame_Seconds);

        Assert.Equal(2, world.current_Lives);
        Assert.Empty(world.enemies);
        Assert.True(world.clock.is_Slow_Motion_Active);
        Assert.Equal("Vie perdue", world.event_Log.entries[^1].text);
    }

    [Fact]
    public void strikes_Are_Ignored_During_Slow_Motion()
    {
        var world = create_World();
        add_Enemy_At_Gap(world, facing_Direction.left, 0);
        world.update(frame_Seconds);
        var target = add_Enemy_At_Gap(world, facing_Direction.right, 50);

        world.handle_Strike(facing_Direction.right);

        Assert.Contains(target, world.enemies);
        Assert.Equal(0, world.stats.missed_Hits);
    }

    [Fact]
    public void two_Contacts_Same_Frame_Cost_Two_Lives()
    {
        var world = create_World();
        add_Enemy_At_Gap(world, facing_Direction.left, 0);
        add_Enemy_At_Gap(world, facing_Direction.right, 0);

        world.update(frame_Seconds);

        Assert.Equal(1, world.current_Lives);
        Assert.Empty(world.enemies);
    }

    [Fact]
    public void last_Life_Lost_Triggers_Game_Over_Without_Slow_Motion()
    {
        var world = create_World();
        add_Enemy_At_Gap(world, facing_Direction.left, 0);
        add_Enemy_At_Gap(world, facing_Direction.right, 0);
        add_Enemy_At_Gap(world, facing_Direction.left, 10);

        world.update(frame_Seconds);

        Assert.Equal(0, world.current_Lives);
        Assert.True(world.is_Game_Over);
        Assert.False(world.clock.is_Slow_Motion_Active);
    }

    [Fact]
    public void update_And_Strikes_Do_Nothing_After_Game_Over()
    {
        var world = create_World();
        for (var index = 0; index < 3; index++)
        {
            add_Enemy_At_Gap(world, facing_Direction.left, 0);
        }
        world.update(frame_Seconds);
        var elapsed_At_Game_Over = world.clock.elapsed_Game_Seconds;
        var frozen_Enemy = add_Enemy_At_Gap(world, facing_Direction.right, 50);
        var frozen_X = frozen_Enemy.center_X;

        world.handle_Strike(facing_Direction.right);
        world.update(1.0);

        Assert.Contains(frozen_Enemy, world.enemies);
        Assert.Equal(frozen_X, frozen_Enemy.center_X);
        Assert.Equal(elapsed_At_Game_Over, world.clock.elapsed_Game_Seconds);
        Assert.Equal(0, world.stats.missed_Hits);
    }

    [Fact]
    public void huge_Delta_Is_Clamped_And_Spawns_At_Most_One_Enemy()
    {
        var settings = game_Settings.create_Default();
        // A tiny interval would allow many spawns in 30 s if the delta or the spawner were not capped.
        settings.difficulty_Tiers[0].spawn_Interval_Seconds = 0.01;
        settings.difficulty_Tiers[0].max_Enemies_On_Screen = 8;
        var world = new match_World(settings, new Random(1));

        world.update(30.0);

        Assert.Equal(match_World.max_Real_Delta_Seconds, world.clock.elapsed_Game_Seconds, 6);
        Assert.Single(world.enemies);
        Assert.Equal(3, world.current_Lives);
    }

    [Fact]
    public void tier_Advances_With_Game_Time()
    {
        var world = create_World();
        var attack_Range = game_Settings.create_Default().player.attack_Range;

        // 905 frames: 900 x 0.1 sums to 89.999... in floating point, just short of the 90 s tier.
        for (var index = 0; index < 905; index++)
        {
            world.update(0.1);
            // Kill anything in range so no enemy ever reaches the player.
            foreach (var side in world.enemies.Where(e => e.get_Gap_To(world.player) <= attack_Range).Select(e => e.spawn_Side).Distinct().ToList())
            {
                world.handle_Strike(side);
            }
        }

        Assert.Equal(3, world.current_Lives);
        Assert.Equal(0, world.stats.missed_Hits);
        Assert.Equal(1, world.current_Tier_Index);
    }
}
