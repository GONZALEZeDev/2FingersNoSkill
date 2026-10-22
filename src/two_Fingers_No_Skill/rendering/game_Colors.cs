using Microsoft.Xna.Framework;

namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// The color palette of the "simple shapes" look. Change a value here to restyle the whole game.
/// </summary>
public static class game_Colors
{
    /// <summary>Screen background.</summary>
    public static readonly Color background = new(0x1B, 0x1D, 0x24);

    /// <summary>Ground line.</summary>
    public static readonly Color ground = new(0x55, 0x55, 0x55);

    /// <summary>Player body.</summary>
    public static readonly Color player = new(0x3D, 0x8B, 0xFD);

    /// <summary>Player body during the miss lockout.</summary>
    public static readonly Color locked_Out_Player = new(0x7A, 0x7F, 0x8C);

    /// <summary>Enemy body and lives.</summary>
    public static readonly Color enemy = new(0xE5, 0x48, 0x4D);

    /// <summary>All text.</summary>
    public static readonly Color text = new(0xE8, 0xE8, 0xE8);

    /// <summary>Strike zones: ground segments always, dashed outline in debug view.</summary>
    public static readonly Color strike_Zone = new(0xF5, 0xC5, 0x42);

    /// <summary>Gold used for highlighted information (rhythm increases).</summary>
    public static readonly Color highlight = new(0xF5, 0xC5, 0x42);

    /// <summary>Restart flash.</summary>
    public static readonly Color flash = Color.White;
}
