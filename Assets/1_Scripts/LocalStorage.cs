using UnityEngine;

public static class LocalStorage
{
    const string KEY_MAX_CLEARED_LEVEL = "MaxClearedLevel";
    const string KEY_MAX_CLEARABLE = "MaxClearableLevel";

    static StageCoord LoadStage(string key, int defaultValue = 0) => StageCoord.FromAbsoluteLevel(PlayerPrefs.GetInt(key, defaultValue));
    static void SaveStage(string key, StageCoord stage)
    {
        PlayerPrefs.SetInt(key, stage.ToAbsoluteLevel());
        PlayerPrefs.Save();
    }

    public static StageCoord LoadMaxClearedStage() => LoadStage(KEY_MAX_CLEARED_LEVEL);
    public static void SaveMaxClearedStage(StageCoord stageCoord) => SaveStage(KEY_MAX_CLEARED_LEVEL, stageCoord);

    public static StageCoord LoadMaxClearableStage() => LoadStage(KEY_MAX_CLEARABLE);
    public static void SaveMaxClearableStage(StageCoord stageCoord) => SaveStage(KEY_MAX_CLEARABLE, stageCoord);

    public static StageCoord LoadMaxPlayable()
    {
        if (PlayerPrefs.HasKey(KEY_MAX_CLEARED_LEVEL) == false) return new StageCoord(0, 0);
        else
        {
            var result = LoadMaxClearableStage();
            result++;
            return result;
        }
    }
}