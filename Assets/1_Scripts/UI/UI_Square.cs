using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public record SquareModel(Color BgColor, Sprite Sprite);

public class UI_Square : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public Image background;
    public Image pieceImage;

    // 초기화 및 클릭 이벤트 연결을 도와주는 헬퍼 함수
    public void Init(UnityAction onClickAction)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClickAction);
    }

    public void UpdateVisuals(Color bgColor, Sprite sprite, Color spriteColor)
    {
        background.color = bgColor;
        pieceImage.sprite = sprite;
        pieceImage.color = spriteColor;
    }

    public void UpdateVisuals(SquareModel squareModel)
    {
        background.color = squareModel.BgColor;
        pieceImage.sprite = squareModel.Sprite;
        pieceImage.color = squareModel.Sprite == null ? Color.clear : Color.white;
    }
}
