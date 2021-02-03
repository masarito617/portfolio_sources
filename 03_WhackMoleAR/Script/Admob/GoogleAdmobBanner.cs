using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

/// <summary>
/// バナー広告表示クラス
/// </summary>
/// <remarks>Admobのバナー広告を表示するクラス</remarks>
public class GoogleAdmobBanner : MonoBehaviour
{
    BannerView bannerView;
    bool dispd = false;

    void Update()
    {
        // dispBannerがTRUEになったら表示
        if (!GameSystemManager.dispBanner || dispd)
            return;
        dispd = true;

        // アプリIDを設定
        #if UNITY_ANDROID
                string appId = GameUtil.Const.ADMOB_ANDROID_APP_ID;
        #elif UNITY_IPHONE
                string appId = GameUtil.Const.ADMOB_IOS_APP_ID;
        #else
                string appId = "unexpected_platform";
        #endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        RequestBanner();
    }

    /// <summary>
    /// 広告読み込み処理
    /// </summary>
    private void RequestBanner()
    {
        // バナー広告IDを設定
        #if UNITY_ANDROID
                string adUnitId = GameUtil.Const.ADMOB_ANDROID_BANNER_ID;
        #elif UNITY_IPHONE
                string adUnitId = GameUtil.Const.ADMOB_IOS_BANNER_ID;
        #else
                string adUnitId = "unexpected_platform";
        #endif
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    /// <summary>
    /// オブジェクト破棄
    /// </summary>
    private void OnDestroy()
    {
        bannerView.Destroy();
    }
}