using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

/// <summary>
/// ランキング認証クラス
/// </summary>
/// <remarks>GPGS、GameCenterの認証クラス</remarks>
public class AuthManager : MonoBehaviour
{
    void Awake()
    {
        // ゲーム内に一つだけ保持
        if (FindObjectsOfType<AuthManager>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
        // 初期化
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        // サインイン実行
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                Debug.Log("Authentication Successed!!");
            }
            else
            {
                Debug.Log("Authentication Failed!!");
            }

            // バナー表示フラグをTRUE
            GameSystemManager.dispBanner = true;
        });
#elif UNITY_IPHONE
        // iOS: GameCenter認証初期化
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                Debug.Log("Authentication Successed!!");
            }
            else
            {
                Debug.Log("Authentication Failed!!");
            }

            // バナー表示フラグをTRUE
            GameSystemManager.dispBanner = true;
        });
#endif
    }
}
