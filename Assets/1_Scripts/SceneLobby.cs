using UnityEngine;

public class SceneLobby : MonoBehaviour
{
    [SerializeField] LockPointDataSO lockPointDataSO;

    void Awake()
    {
        // 최초 unlock
        if (LocalStorage.LoadMaxClearableStage() == new StageCoord(0, 0))
            lockPointDataSO.SaveMaxClearableStage(new StageCoord(0, 0));
    }
}