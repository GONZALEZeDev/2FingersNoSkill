using System.Runtime.CompilerServices;
using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.entities;
using two_Fingers_No_Skill.systems;

[assembly: InternalsVisibleTo("two_Fingers_No_Skill.tests")]

namespace two_Fingers_No_Skill.core;

/// <summary>
/// One match: owns the player, the enemies and every gameplay system, and applies the rules each frame.
/// Contains no drawing code, so it can be fully unit-tested.
/// </summary>
public sealed class match_World
{
    /// <summary>Longest real frame duration taken into account, so a long freeze cannot skip gameplay.</summary>
    public const double max_Real_Delta_Seconds = 0.1;

    private readonly game_Settings settings;
    private readonly game_Event_Bus event_Bus = new();
    private readonly difficulty_Manager difficulty;
    private readonly enemy_Spawner spawner;
    private readonly List<enemy> enemy_List = new();

    /// <summary>
    /// Creates a fresh match: full lives, score at 0, no enemy.
    /// </summary>
    /// <param name="settings">Gameplay settings.</param>
    /// <param name="random">Random source used for spawn sides.</param>
    public match_World(game_Settings settings, Random random)
    {
        this.settings = settings;
        current_Lives = settings.starting_Lives;
        player = new player(world_Layout.player_Center_X, world_Layout.player_Half_Width);
        clock = new game_Clock(settings.slow_Motion_Time_Scale);
        difficulty = new difficulty_Manager(settings.difficulty_Tiers);
        spawner = new enemy_Spawner(random);
        score = new score_Manager(settings.score_Rules, settings.minimum_Score);
        stats = new match_Stats();
        event_Log = new event_Log(settings.event_Log.max_Visible_Events, settings.score_Rules);

        // Systems react to gameplay events without the match knowing what they do with them.
        event_Bus.event_Published += score.handle_Event;
        event_Bus.event_Published += stats.handle_Event;
        event_Bus.event_Published += event_Log.handle_Event;
    }

    /// <summary>The player character.</summary>
    public player player { get; }

    /// <summary>Enemies currently alive.</summary>
    public IReadOnlyList<enemy> enemies => enemy_List;

    /// <summary>Remaining lives.</summary>
    public int current_Lives { get; private set; }

    /// <summary>True once the last life is lost; the match is then frozen.</summary>
    public bool is_Game_Over { get; private set; }

    /// <summary>Match clock (elapsed time and slow motion).</summary>
    public game_Clock clock { get; }

    /// <summary>Index of the active difficulty tier.</summary>
    public int current_Tier_Index => difficulty.get_Tier_Index_At(clock.elapsed_Game_Seconds);

    /// <summary>Match score.</summary>
    public score_Manager score { get; }

    /// <summary>Counters shown on the game over screen.</summary>
    public match_Stats stats { get; }

    /// <summary>Latest events, for the on-screen log.</summary>
    public event_Log event_Log { get; }

    /// <summary>
    /// Runs one frame of gameplay: time, spawns, movement and contacts.
    /// </summary>
    /// <param name="real_Delta_Seconds">Real time elapsed since the previous frame.</param>
    public void update(double real_Delta_Seconds)
    {
        if (is_Game_Over)
        {
            return;
        }

        var game_Delta = clock.advance(Math.Min(real_Delta_Seconds, max_Real_Delta_Seconds));
        player.update(game_Delta);
        var tier = difficulty.get_Tier_At(clock.elapsed_Game_Seconds);

        if (spawner.try_Spawn(game_Delta, tier, enemy_List.Count, out var spawn_Side))
        {
            spawn_Enemy(spawn_Side, tier.enemy_Speed);
        }

        foreach (var walking_Enemy in enemy_List)
        {
            walking_Enemy.move_Toward(player, game_Delta);
        }

        resolve_Contacts(tier);
    }

    /// <summary>
    /// Handles a strike toward a side: kills the nearest enemy in range, or counts a miss.
    /// Ignored during game over, slow motion and miss lockout.
    /// </summary>
    public void handle_Strike(facing_Direction side)
    {
        if (is_Game_Over || clock.is_Slow_Motion_Active || player.is_Locked_Out)
        {
            return;
        }

        player.face(side);
        var target = combat_System.find_Strike_Target(player, side, enemy_List, settings.player.attack_Range);
        if (target is not null)
        {
            enemy_List.Remove(target);
            publish(game_Event_Type.enemy_Killed);
        }
        else
        {
            publish(game_Event_Type.missed_Hit);
            player.start_Lockout(settings.player.miss_Lockout_Seconds);
        }
    }

    /// <summary>
    /// Adds an enemy directly. Test helper only.
    /// </summary>
    internal void add_Enemy_For_Tests(enemy new_Enemy) => enemy_List.Add(new_Enemy);

    /// <summary>
    /// Creates an enemy just outside the screen on the given side.
    /// </summary>
    private void spawn_Enemy(facing_Direction side, float speed)
    {
        var start_X = side == facing_Direction.left
            ? -world_Layout.enemy_Half_Width
            : world_Layout.screen_Width + world_Layout.enemy_Half_Width;
        enemy_List.Add(new enemy(side, start_X, world_Layout.enemy_Half_Width, speed));
    }

    /// <summary>
    /// Every enemy in contact hits the player: one life each. Ends the match at 0 lives,
    /// otherwise starts the slow motion of the active tier.
    /// </summary>
    private void resolve_Contacts(difficulty_Tier tier)
    {
        var attackers = enemy_List
            .Where(candidate => combat_System.has_Reached(candidate, player, settings.player.contact_Distance))
            .ToList();
        if (attackers.Count == 0)
        {
            return;
        }

        foreach (var attacker in attackers)
        {
            enemy_List.Remove(attacker);
            current_Lives--;
            publish(game_Event_Type.life_Lost);
            if (current_Lives <= 0)
            {
                // No slow motion on the final hit: the game over screen takes over immediately.
                is_Game_Over = true;
                return;
            }
        }

        clock.start_Slow_Motion(tier.slow_Motion_Duration_Seconds);
    }

    /// <summary>
    /// Publishes an event stamped with the current match time.
    /// </summary>
    private void publish(game_Event_Type event_Type) =>
        event_Bus.publish(new game_Event(event_Type, clock.elapsed_Game_Seconds));
}
