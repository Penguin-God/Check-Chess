using UnityEngine;

public class Util : MonoBehaviour
{
    [SerializeField] string MaxPlayable;
    [ContextMenu("플레이 가능한 최대 부분 저장")]
    void SaveClear() => LocalStorage.SaveMaxPlayableStage(StageCoord.FromBoardCoord(BoardCoord.FromChessSquare(MaxPlayable)));
}
