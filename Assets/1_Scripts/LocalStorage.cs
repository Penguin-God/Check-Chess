using UnityEngine;

public static class LocalStorage
{
    const string KEY_MAX_CLEARED_LEVEL = "MaxClearedLevel";
    const string KEY_UNLOCKED_STAGES = "UnlockedStageLevels";

    public static StageCoord LoadMaxClearedStage() => StageCoord.FromAbsoluteLevel(PlayerPrefs.GetInt(KEY_MAX_CLEARED_LEVEL, 0));

    public static void SaveMaxClearedStage(StageCoord stageCoord)
    {
        PlayerPrefs.SetInt(KEY_MAX_CLEARED_LEVEL, stageCoord.ToAbsoluteLevel());
        PlayerPrefs.Save(); // 기기에 확실히 기록되도록 Save() 호출
    }

    public static string LoadUnlockedStages() => PlayerPrefs.GetString(KEY_UNLOCKED_STAGES, "");

    public static void SaveUnlockedStages(string data)
    {
        PlayerPrefs.SetString(KEY_UNLOCKED_STAGES, data);
        PlayerPrefs.Save();
    }
}