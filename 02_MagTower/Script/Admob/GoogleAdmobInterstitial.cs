using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// インタースティシャル広告表示クラス
/// </summary>
/// <remarks>Admobのインタースティシャル広告を表示するクラス</remarks>
public class GoogleAdmobInterstitial : MonoBehaviour
{
    /** 変数 */
    private InterstitialAd interstitial;
    private bool showing;

    void Start()
    {
        showing = false;
        // 広告読み込み
        RequestInterstitial();
    }

    void Update()
    {
        // 広告表示
        if (interstitial.IsLoaded() && !showing)
        {
            interstitial.Show();
            showing = true;
        }
    }

    /// <summary>
    /// 広告読み込み処理
    /// </summary>
    private void RequestInterstitial()
    {
        // インタースティシャル広告ID
#if UNITY_ANDROID
        string adUnitId = "【インタースティシャル広告ID(Android)】";
#elif UNITY_IPHONE
        string adUnitId = "【インタースティシャル広告ID(iOS)】";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    /// <summary>
    /// シーン遷移処理
    /// </summary>
    private void LoadNextScene()
    {
        // ゲームシーンへ遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_GAME);
    }

    /// <summary>
    /// オブジェクト破棄
    /// </summary>
    private void OnDestroy()
    {
        // オブジェクトの破棄
        interstitial.Destroy();
    }

    // ---以下、イベントハンドラー
    /// <summary>
    /// 広告の読み込み完了時
    /// </summary>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
    }

    /// <summary>
    /// 広告の読み込み失敗時
    /// </summary>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        // 次のシーンに遷移
        LoadNextScene();
    }

    /// <summary>
    /// 広告がデバイスの画面いっぱいに表示されたとき
    /// </summary>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
    }

    /// <summary>
    /// 広告を閉じたとき
    /// </summary>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        // 次のシーンに遷移
        LoadNextScene();
    }

    /// <summary>
    /// 別のアプリ（Google Play ストアなど）を起動した時
    /// </summary>
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
    }
}
