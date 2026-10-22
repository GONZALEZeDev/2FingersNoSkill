using two_Fingers_No_Skill.entities;
using two_Fingers_No_Skill.systems;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for enemy distances, movement and strike resolution.
/// </summary>
public class combat_System_Tests
{
    private const float enemy_Half_Width = 18f;

    private static readonly player hero = new(640, 20);

    /// <summary>
    /// Creates an enemy on the given side whose facing edge is <paramref name="gap"/> pixels from the player.
    /// </summary>
    private static enemy create_Enemy_At_Gap(facing_Direction side, float gap, float speed = 100)
    {
        var center_X = side == facing_Direction.left
            ? 620 - gap - enemy_Half_Width
            : 660 + gap + enemy_Half_Width;
        return new enemy(side, center_X, enemy_Half_Width, speed);
    }

    [Fact]
    public void gap_Is_Measured_From_Player_Edge()
    {
        var left_Enemy = new enemy(facing_Direction.left, 500, enemy_Half_Width, 100);

        Assert.Equal(102f, left_Enemy.get_Gap_To(hero), 3);
    }

    [Fact]
    public void enemy_Stops_At_Contact_Instead_Of_Overlapping()
    {
        var left_Enemy = create_Enemy_At_Gap(facing_Direction.left, 5, speed: 200);
        var right_Enemy = create_Enemy_At_Gap(facing_Direction.right, 5, speed: 200);

        left_Enemy.move_Toward(hero, 1.0);
        right_Enemy.move_Toward(hero, 1.0);

        Assert.Equal(0f, left_Enemy.get_Gap_To(hero), 3);
        Assert.Equal(0f, right_Enemy.get_Gap_To(hero), 3);
    }

    [Fact]
    public void strike_Hits_Nearest_Enemy_On_Struck_Side_Within_Range()
    {
        var far_Left = create_Enemy_At_Gap(facing_Direction.left, 120);
        var near_Left = create_Enemy_At_Gap(facing_Direction.left, 40);
        var near_Right = create_Enemy_At_Gap(facing_Direction.right, 10);

        var target = combat_System.find_Strike_Target(hero, facing_Direction.left, new[] { far_Left, near_Left, near_Right }, 140);

        Assert.Same(near_Left, target);
    }

    [Fact]
    public void strike_Ignores_Enemies_Out_Of_Range_Or_On_Other_Side()
    {
        var out_Of_Range_Left = create_Enemy_At_Gap(facing_Direction.left, 141);
        var near_Right = create_Enemy_At_Gap(facing_Direction.right, 10);

        var target = combat_System.find_Strike_Target(hero, facing_Direction.left, new[] { out_Of_Range_Left, near_Right }, 140);

        Assert.Null(target);
    }

    [Fact]
    public void enemy_Reaches_Player_At_Contact_Distance()
    {
        Assert.True(combat_System.has_Reached(create_Enemy_At_Gap(facing_Direction.right, 30), hero, 30));
        Assert.False(combat_System.has_Reached(create_Enemy_At_Gap(facing_Direction.right, 31), hero, 30));
    }
}
