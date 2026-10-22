using two_Fingers_No_Skill.input;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests that the click used to focus the game window never counts as a strike.
/// </summary>
public class focus_Input_Guard_Tests
{
    private static readonly input_Snapshot left_Click = new(true, false, false, false);
    private static readonly input_Snapshot enter_Press = new(false, false, true, false);
    private static readonly input_Snapshot nothing = input_Snapshot.empty;

    [Fact]
    public void inactive_Window_Gets_No_Input()
    {
        var guard = new focus_Input_Guard();

        Assert.Equal(input_Snapshot.empty, guard.filter(left_Click, is_Window_Active: false, is_Any_Mouse_Button_Held: true));
    }

    [Fact]
    public void click_That_Focuses_The_Window_Is_Ignored()
    {
        var guard = new focus_Input_Guard();
        guard.filter(nothing, is_Window_Active: false, is_Any_Mouse_Button_Held: false);

        var result = guard.filter(left_Click, is_Window_Active: true, is_Any_Mouse_Button_Held: true);

        Assert.False(result.left_Strike_Pressed);
    }

    [Fact]
    public void next_Click_After_Release_Counts()
    {
        var guard = new focus_Input_Guard();
        guard.filter(nothing, is_Window_Active: false, is_Any_Mouse_Button_Held: false);
        guard.filter(left_Click, is_Window_Active: true, is_Any_Mouse_Button_Held: true);
        guard.filter(nothing, is_Window_Active: true, is_Any_Mouse_Button_Held: false);

        var result = guard.filter(left_Click, is_Window_Active: true, is_Any_Mouse_Button_Held: true);

        Assert.True(result.left_Strike_Pressed);
    }

    [Fact]
    public void keyboard_Still_Works_While_Mouse_Is_Blocked()
    {
        var guard = new focus_Input_Guard();
        guard.filter(nothing, is_Window_Active: false, is_Any_Mouse_Button_Held: false);

        var result = guard.filter(enter_Press, is_Window_Active: true, is_Any_Mouse_Button_Held: true);

        Assert.True(result.confirm_Pressed);
    }
}
