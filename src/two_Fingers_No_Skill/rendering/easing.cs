namespace two_Fingers_No_Skill.rendering;

/// <summary>
/// Animation curves turning a linear progress (0 to 1) into a smoother one.
/// </summary>
public static class easing
{
    /// <summary>
    /// Fast start, soft landing. Input is clamped to [0, 1].
    /// </summary>
    public static float ease_Out_Cubic(float t)
    {
        var clamped = Math.Clamp(t, 0f, 1f);
        var inverse = 1f - clamped;
        return 1f - inverse * inverse * inverse;
    }
}
