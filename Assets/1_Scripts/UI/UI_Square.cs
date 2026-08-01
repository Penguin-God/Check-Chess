using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Square : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public Image background;
    public Image pieceImage;

    // 초기화 및 클릭 이벤트 연결을 도와주는 헬퍼 함수
    public void Init(UnityAction onClickAction)
    {
        button.onClick.RemoveAllListeners(); // 중복 등록 방지
        button.onClick.AddListener(onClickAction);
    }

    // 시각적 업데이트를 담당하는 함수를 만들어두면 GameBoardUI가 더 깔끔해집니다.
    public void UpdateVisuals(Color bgColor, Sprite sprite, Color spriteColor)
    {
        background.color = bgColor;
        pieceImage.sprite = sprite;
        pieceImage.color = spriteColor;
    }
}
