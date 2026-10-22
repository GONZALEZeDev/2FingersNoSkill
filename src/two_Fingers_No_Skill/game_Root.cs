using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using two_Fingers_No_Skill.config;
using two_Fingers_No_Skill.core;
using two_Fingers_No_Skill.game_States;
using two_Fingers_No_Skill.input;
using two_Fingers_No_Skill.rendering;

namespace two_Fingers_No_Skill;

/// <summary>
/// Root MonoGame class: owns the window, loads resources, and runs the main loop
/// by delegating each frame to the active game state.
/// </summary>
public class game_Root : Game
{
    private readonly GraphicsDeviceManager graphics_Manager;
    private readonly input_Handler input_Reader = new();
    private readonly focus_Input_Guard focus_Guard = new();
    private readonly game_State_Machine state_Machine = new();
    private game_Services? services;
    private render_Context? render_Ctx;

    /// <summary>
    /// Configures the 1280x720 window.
    /// </summary>
    public game_Root()
    {
        graphics_Manager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = world_Layout.screen_Width,
            PreferredBackBufferHeight = world_Layout.screen_Height,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = ui_Texts.game_Title;
    }

    /// <summary>
    /// Loads settings, fonts and drawing tools, then opens the title screen. (MonoGame-imposed method name.)
    /// </summary>
    protected override void LoadContent()
    {
        var settings_Path = Path.Combine(AppContext.BaseDirectory, settings_Loader.default_File_Name);
        var settings = settings_Loader.load_From_File(settings_Path, Console.Error.WriteLine);

        render_Ctx = new render_Context(
            new SpriteBatch(GraphicsDevice),
            new shape_Renderer(GraphicsDevice),
            Content.Load<SpriteFont>("fonts/hud_Font"),
            Content.Load<SpriteFont>("fonts/title_Font"));

        services = new game_Services(settings, state_Machine, new Random());
        state_Machine.change_State(new title_State(services));
    }

    /// <summary>
    /// Reads input and updates the active screen. (MonoGame-imposed method name.)
    /// </summary>
    protected override void Update(GameTime game_Time)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // Always read, to keep "just pressed" detection right, then let the guard drop the click
        // that focuses the window so it never counts as a strike.
        var mouse_State = Mouse.GetState();
        var is_Any_Mouse_Button_Held = mouse_State.LeftButton == ButtonState.Pressed || mouse_State.RightButton == ButtonState.Pressed;
        var input = focus_Guard.filter(input_Reader.read(), IsActive, is_Any_Mouse_Button_Held);

        if (input.debug_Toggle_Pressed && services is not null)
        {
            services.is_Debug_View_Enabled = !services.is_Debug_View_Enabled;
        }

        state_Machine.update(game_Time.ElapsedGameTime.TotalSeconds, input);
        base.Update(game_Time);
    }

    /// <summary>
    /// Clears the screen and draws the active screen. (MonoGame-imposed method name.)
    /// </summary>
    protected override void Draw(GameTime game_Time)
    {
        GraphicsDevice.Clear(game_Colors.background);

        if (render_Ctx is not null)
        {
            render_Ctx.batch.Begin(samplerState: SamplerState.PointClamp);
            state_Machine.draw(render_Ctx);
            render_Ctx.batch.End();
        }

        base.Draw(game_Time);
    }
}
