using UnityEngine;

public static class LocalStorage
{
    const string KEY_MAX_PLAYABLE = "MaxPlayableLevel";
    const string KEY_MAX_CLEARABLE = "MaxClearableLevel";

    static StageCoord LoadStage(string key, int defaultValue = 0) => StageCoord.FromAbsoluteLevel(PlayerPrefs.GetInt(key, defaultValue));
    static void SaveStage(string key, StageCoord stage)
    {
        PlayerPrefs.SetInt(key, stage.ToAbsoluteLevel());
        PlayerPrefs.Save();
    }

    public static StageCoord LoadMaxPlayableStage() => LoadStage(KEY_MAX_PLAYABLE);
    public static void SaveMaxPlayableStage(StageCoord stageCoord) => SaveStage(KEY_MAX_PLAYABLE, stageCoord);

    public static StageCoord LoadMaxClearableStage() => LoadStage(KEY_MAX_CLEARABLE);
    public static void SaveMaxClearableStage(StageCoord stageCoord) => SaveStage(KEY_MAX_CLEARABLE, stageCoord);
}