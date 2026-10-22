namespace two_Fingers_No_Skill.game_States;

/// <summary>
/// Every French text shown on screen (except event log lines), gathered in one place.
/// </summary>
public static class ui_Texts
{
    /// <summary>Game title.</summary>
    public const string game_Title = "2 FINGERS NO SKILL";

    /// <summary>Title screen prompt.</summary>
    public const string start_Prompt = "Appuyez sur Bouton M1 souris pour commencer !";

    /// <summary>HUD score, {0} = score.</summary>
    public const string score_Format = "SCORE : {0}";

    /// <summary>Game over heading.</summary>
    public const string game_Over_Title = "GAME OVER";

    /// <summary>Game over score line, {0} = score.</summary>
    public const string final_Score_Format = "Score : {0}";

    /// <summary>Game over kill count line, {0} = count.</summary>
    public const string enemies_Killed_Format = "Ennemis tués : {0}";

    /// <summary>Game over missed hits line, {0} = count.</summary>
    public const string missed_Hits_Format = "Coups ratés : {0}";

    /// <summary>Game over prompt.</summary>
    public const string restart_Prompt = "Appuyez sur Entrée pour rejouer";
}
