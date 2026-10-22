using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.entities;

namespace two_Fingers_No_Skill.systems;

/// <summary>
/// Decides when a new enemy appears and on which side.
/// </summary>
public sealed class enemy_Spawner
{
    private readonly Random random;
    private double spawn_Timer_Seconds;

    /// <summary>
    /// Creates the spawner. The first enemy appears after one full spawn interval.
    /// </summary>
    /// <param name="random">Random source, injectable so that tests are deterministic.</param>
    public enemy_Spawner(Random random)
    {
        this.random = random;
    }

    /// <summary>
    /// Advances the spawn timer and tells whether an enemy must appear now. At most one enemy per call.
    /// </summary>
    /// <param name="game_Delta_Seconds">Match time elapsed since the previous call.</param>
    /// <param name="tier">Active difficulty tier.</param>
    /// <param name="alive_Enemy_Count">Enemies currently alive.</param>
    /// <param name="spawn_Side">Side of the new enemy, when one spawns.</param>
    public bool try_Spawn(double game_Delta_Seconds, difficulty_Tier tier, int alive_Enemy_Count, out facing_Direction spawn_Side)
    {
        spawn_Side = facing_Direction.left;
        spawn_Timer_Seconds += game_Delta_Seconds;
        if (spawn_Timer_Seconds < tier.spawn_Interval_Seconds)
        {
            return false;
        }

        if (alive_Enemy_Count >= tier.max_Enemies_On_Screen)
        {
            // The screen is full: keep the timer "ready" so the next enemy comes as soon as a slot frees.
            spawn_Timer_Seconds = tier.spawn_Interval_Seconds;
            return false;
        }

        spawn_Timer_Seconds = 0;
        spawn_Side = random.Next(2) == 0 ? facing_Direction.left : facing_Direction.right;
        return true;
    }
}
