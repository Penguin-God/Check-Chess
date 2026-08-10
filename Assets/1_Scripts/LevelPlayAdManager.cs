using UnityEngine;
using Unity.Services.LevelPlay;
using System;

public class LevelPlayAdManager : MonoBehaviour
{
    public static LevelPlayAdManager Instance { get; private set; }

    [Header("LevelPlay Settings")]
    public string appKey = "277cdb17d";
    public string adUnitId = "ufblnzbx0r5k6upi";

    LevelPlayRewardedAd rewardedAd;
    Action onAdSuccessCallback; // UI에 결과를 알려줄 델리게이트

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        LevelPlay.Init(appKey);
    }

    void OnInitSuccess(LevelPlayConfiguration config)
    {
        rewardedAd = new LevelPlayRewardedAd(adUnitId);
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
        rewardedAd.LoadAd();
    }

    void OnInitFailed(LevelPlayInitError error) { Debug.Log("초기화 실패: " + error.ErrorMessage); }

    // UI에서 광고를 호출할 때 성공 시 실행할 함수(Action)를 같이 넘겨받습니다.
    public void ShowRewardedAd(Action onSuccess)
    {
        onAdSuccessCallback = onSuccess;

        if (rewardedAd != null && rewardedAd.IsAdReady())
            rewardedAd.ShowAd();
        else
            Debug.Log("광고가 아직 준비되지 않았습니다.");
    }

    void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log("광고 시청 완료!");
        onAdSuccessCallback?.Invoke(); // 저장해둔 콜백 실행!
        onAdSuccessCallback = null;
        rewardedAd.LoadAd();
    }

    void OnAdLoadFailed(LevelPlayAdError error) { Debug.Log("광고 로드 실패: " + error.ErrorMessage); }
}