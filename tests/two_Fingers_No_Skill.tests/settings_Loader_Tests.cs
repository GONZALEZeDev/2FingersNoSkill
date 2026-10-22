using System.Text.Json;
using System.Text.Json.Nodes;
using two_Fingers_No_Skill.config;

namespace two_Fingers_No_Skill.tests;

/// <summary>
/// Tests for loading and validating game_Settings.json.
/// </summary>
public class settings_Loader_Tests
{
    /// <summary>
    /// Returns the default settings serialised as a mutable JSON tree.
    /// </summary>
    private static JsonNode default_Json_Node() =>
        JsonNode.Parse(JsonSerializer.Serialize(game_Settings.create_Default()))!;

    [Fact]
    public void default_Settings_Match_Shipped_Json()
    {
        var from_File = settings_Loader.parse_Json(File.ReadAllText("game_Settings.json"));

        Assert.Equal(
            JsonSerializer.Serialize(game_Settings.create_Default()),
            JsonSerializer.Serialize(from_File));
    }

    [Fact]
    public void defaults_Have_Spec_Values()
    {
        var settings = game_Settings.create_Default();

        Assert.Equal(3, settings.starting_Lives);
        Assert.Equal(20, settings.score_Rules.enemy_Killed);
        Assert.Equal(-50, settings.score_Rules.missed_Hit);
        Assert.Equal(0, settings.minimum_Score);
        Assert.Equal(0.3, settings.player.miss_Lockout_Seconds);
        Assert.Equal(7, settings.difficulty_Tiers.Count);
        Assert.Equal(new[] { 0d, 90, 180, 270, 360, 450, 540 }, settings.difficulty_Tiers.Select(t => t.start_Time_Seconds));
        Assert.Equal(new[] { 1.25, 1.25, 1.25, 1.15, 1.05, 0.95, 0.85 }, settings.difficulty_Tiers.Select(t => t.slow_Motion_Duration_Seconds));
    }

    [Fact]
    public void missing_File_Returns_Defaults_And_Reports_Error()
    {
        string? reported = null;

        var settings = settings_Loader.load_From_File("does_Not_Exist.json", message => reported = message);

        Assert.NotNull(reported);
        Assert.Contains("does_Not_Exist.json", reported);
        Assert.Equal(3, settings.starting_Lives);
    }

    [Fact]
    public void malformed_Json_Returns_Defaults_And_Reports_Error()
    {
        var file_Path = Path.GetTempFileName();
        string? reported = null;

        try
        {
            File.WriteAllText(file_Path, "{ not json");
            var settings = settings_Loader.load_From_File(file_Path, message => reported = message);

            Assert.NotNull(reported);
            Assert.Equal(7, settings.difficulty_Tiers.Count);
        }
        finally
        {
            File.Delete(file_Path);
        }
    }

    [Theory]
    [InlineData("empty_Tiers")]
    [InlineData("first_Tier_Not_At_Zero")]
    [InlineData("non_Positive_Interval")]
    [InlineData("non_Positive_Speed")]
    [InlineData("zero_Max_Enemies")]
    [InlineData("zero_Starting_Lives")]
    public void invalid_Values_Are_Rejected(string case_Name)
    {
        var root = default_Json_Node();
        var tiers = root["difficulty_Tiers"]!.AsArray();
        switch (case_Name)
        {
            case "empty_Tiers": tiers.Clear(); break;
            case "first_Tier_Not_At_Zero": tiers[0]!["start_Time_Seconds"] = 10; break;
            case "non_Positive_Interval": tiers[2]!["spawn_Interval_Seconds"] = 0; break;
            case "non_Positive_Speed": tiers[1]!["enemy_Speed"] = -5; break;
            case "zero_Max_Enemies": tiers[3]!["max_Enemies_On_Screen"] = 0; break;
            case "zero_Starting_Lives": root["starting_Lives"] = 0; break;
        }

        Assert.Throws<settings_Validation_Exception>(() => settings_Loader.parse_Json(root.ToJsonString()));
    }

    [Theory]
    [InlineData("null_Tier_List")]
    [InlineData("null_Tier_Element")]
    [InlineData("null_Player")]
    [InlineData("null_Event_Log")]
    [InlineData("null_Score_Rules")]
    [InlineData("negative_Minimum_Score")]
    [InlineData("misspelled_Key")]
    [InlineData("missing_Key")]
    public void broken_Hand_Edits_Fall_Back_To_Defaults_With_A_Message(string case_Name)
    {
        var root = default_Json_Node();
        switch (case_Name)
        {
            case "null_Tier_List": root["difficulty_Tiers"] = null; break;
            case "null_Tier_Element": root["difficulty_Tiers"]!.AsArray()[1] = null; break;
            case "null_Player": root["player"] = null; break;
            case "null_Event_Log": root["event_Log"] = null; break;
            case "null_Score_Rules": root["score_Rules"] = null; break;
            case "negative_Minimum_Score": root["minimum_Score"] = -100; break;
            case "misspelled_Key":
                root["player"]!.AsObject().Remove("miss_Lockout_Seconds");
                root["player"]!["miss_lockout_Seconds"] = 0.3;
                break;
            case "missing_Key": root.AsObject().Remove("score_Rules"); break;
        }
        var file_Path = Path.GetTempFileName();
        string? reported = null;

        try
        {
            File.WriteAllText(file_Path, root.ToJsonString());
            var settings = settings_Loader.load_From_File(file_Path, message => reported = message);

            Assert.NotNull(reported);
            Assert.Equal(0.3, settings.player.miss_Lockout_Seconds);
            Assert.Equal(20, settings.score_Rules.enemy_Killed);
        }
        finally
        {
            File.Delete(file_Path);
        }
    }

    [Fact]
    public void unsorted_Tiers_Are_Sorted_By_Start_Time()
    {
        var root = default_Json_Node();
        var reversed_Tiers = new JsonArray(root["difficulty_Tiers"]!.AsArray().Reverse().Select(t => t!.DeepClone()).ToArray());
        root["difficulty_Tiers"] = reversed_Tiers;

        var settings = settings_Loader.parse_Json(root.ToJsonString());

        Assert.Equal(new[] { 0d, 90, 180, 270, 360, 450, 540 }, settings.difficulty_Tiers.Select(t => t.start_Time_Seconds));
    }
}
