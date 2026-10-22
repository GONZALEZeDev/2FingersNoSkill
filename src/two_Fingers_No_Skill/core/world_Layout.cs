namespace two_Fingers_No_Skill.core;

/// <summary>
/// Fixed screen and entity dimensions, in pixels.
/// </summary>
public static class world_Layout
{
    /// <summary>Window width.</summary>
    public const int screen_Width = 1280;

    /// <summary>Window height.</summary>
    public const int screen_Height = 720;

    /// <summary>Vertical position of the ground line; characters stand on it.</summary>
    public const float ground_Y = 520f;

    /// <summary>Horizontal center of the player (middle of the screen).</summary>
    public const float player_Center_X = screen_Width / 2f;

    /// <summary>Half of the player body width.</summary>
    public const float player_Half_Width = 20f;

    /// <summary>Player body height.</summary>
    public const float player_Height = 140f;

    /// <summary>Half of an enemy body width.</summary>
    public const float enemy_Half_Width = 18f;

    /// <summary>Enemy body height.</summary>
    public const float enemy_Height = 125f;
}
