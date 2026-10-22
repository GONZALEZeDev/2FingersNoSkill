using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// White flash between game over and the title screen: fade to white, swap screens at the peak, fade back.
/// </summary>
public sealed class restart_Flash_State : i_Game_State
{
    /// <summary>Duration of the fade to white.</summary>
    public const double fade_In_Seconds = 0.2;

    /// <summary>Duration of the fade from white.</summary>
    public const double fade_Out_Seconds = 0.25;

    private readonly game_Services services;
    private readonly i_Game_State fading_Out_State;
    private title_State? next_Title_State;
    private double timer_Seconds;

    /// <summary>
    /// Starts the flash on top of the screen being left.
    /// </summary>
    public restart_Flash_State(game_Services services, i_Game_State fading_Out_State)
    {
        this.services = services;
        this.fading_Out_State = fading_Out_State;
    }

    /// <summary>
    /// Advances the flash. Input is ignored. Builds the title screen at the peak and switches to it at the end.
    /// </summary>
    public void update(double real_Delta_Seconds, input_Snapshot input)
    {
        timer_Seconds += real_Delta_Seconds;
        if (timer_Seconds >= fade_In_Seconds)
        {
            // The finished match is dropped here; the next M1 on the title screen builds a fresh one.
            next_Title_State ??= new title_State(services);
        }
        if (timer_Seconds >= fade_In_Seconds + fade_Out_Seconds)
        {
            services.state_Machine.change_State(next_Title_State!);
        }
    }

    /// <summary>
    /// Draws the underlying screen (game over, then title) under a white layer whose opacity rises then falls.
    /// </summary>
    public void draw(render_Context ctx)
    {
        float white_Opacity;
        if (next_Title_State is null)
        {
            fading_Out_State.draw(ctx);
            white_Opacity = (float)(timer_Seconds / fade_In_Seconds);
        }
        else
        {
            next_Title_State.draw(ctx);
            white_Opacity = 1f - (float)((timer_Seconds - fade_In_Seconds) / fade_Out_Seconds);
        }

        ctx.fill_Screen(game_Colors.flash * Math.Clamp(white_Opacity, 0f, 1f), world_Layout.screen_Width, world_Layout.screen_Height);
    }
}
