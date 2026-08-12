using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LockPointData", menuName = "ChessPuzzle/LockPointData")]
public class LockPointDataSO : ScriptableObject
{
    [Header("자물쇠로 잠글 스테이지 좌표 (예: b1, c3)")]
    [SerializeField] List<string> lockPointCoords;
    public HashSet<StageCoord> GetLockPoints() => StageLogic.StringToStage(lockPointCoords);
}