using UnityEngine;
using Unity.Services.LevelPlay;

public class LevelPlayAdManager : MonoBehaviour
{
    [Header("LevelPlay Settings")]
    public string appKey = "277cdb17d";
    public string adUnitId = "ufblnzbx0r5k6upi"; // 대시보드에서 만든 보상형 광고의 ID

    // 2. 최신 버전은 '광고 객체'를 직접 생성해서 관리하는 방식으로 변경되었습니다.
    private LevelPlayRewardedAd rewardedAd;

    void Start()
    {
        // 3. 새로운 API 규격으로 초기화 이벤트를 연결하고 SDK를 실행합니다.
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;

        LevelPlay.Init(appKey);
    }

    void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("레벨플레이 SDK 초기화 완료!");

        // 4. 초기화가 무사히 끝나면 광고 객체를 생성하고 이벤트를 연결합니다.
        rewardedAd = new LevelPlayRewardedAd(adUnitId);
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;

        // 유저가 힌트 버튼을 누르기 전에 광고를 미리 다운로드해 둡니다.
        rewardedAd.LoadAd();
    }

    void OnInitFailed(LevelPlayInitError error)
    {
        Debug.Log("초기화 실패: " + error.ErrorMessage);
    }

    // UI 힌트 버튼에 연결할 메서드입니다.
    public void ShowHintAd()
    {
        if (rewardedAd != null && rewardedAd.IsAdReady())
        {
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.Log("광고가 아직 준비되지 않았습니다.");
        }
    }

    // [핵심] 유저가 보상형 광고 시청을 완료했을 때 자동으로 실행되는 콜백
    void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log("광고 시청 완료! 힌트를 제공합니다.");

        // TODO: 여기에 만들어두신 체스 힌트 해금 로직(TryToggleHintCoord 관련)을 호출해 주세요!

        // 시청이 끝났으니 다음 힌트 기능을 위해 광고를 다시 로드해 둡니다.
        rewardedAd.LoadAd();
    }

    void OnAdLoadFailed(LevelPlayAdError error)
    {
        Debug.Log("광고 로드 실패: " + error.ErrorMessage);
    }
}