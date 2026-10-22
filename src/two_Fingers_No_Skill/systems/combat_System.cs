using two_Fingers_No_Skill.entities;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Combat rules: who a strike hits, and when an enemy hits the player.
/// </summary>
public static class combat_System
{
    /// <summary>
    /// Finds the enemy hit by a strike: the nearest one on the struck side within range, or null for a miss.
    /// </summary>
    /// <param name="striker">The player.</param>
    /// <param name="side">Side of the strike.</param>
    /// <param name="enemies">Enemies alive.</param>
    /// <param name="attack_Range">Strike zone width, measured from the player edge.</param>
    public static enemy? find_Strike_Target(player striker, facing_Direction side, IReadOnlyList<enemy> enemies, float attack_Range)
    {
        enemy? nearest = null;
        var nearest_Gap = float.MaxValue;
        foreach (var candidate in enemies)
        {
            if (candidate.spawn_Side != side)
            {
                continue;
            }

            var gap = candidate.get_Gap_To(striker);
            if (gap <= attack_Range && gap < nearest_Gap)
            {
                nearest = candidate;
                nearest_Gap = gap;
            }
        }
        return nearest;
    }

    /// <summary>
    /// True when the enemy is close enough to hit the player.
    /// </summary>
    public static bool has_Reached(enemy attacker, player target, float contact_Distance) =>
        attacker.get_Gap_To(target) <= contact_Distance;
}
