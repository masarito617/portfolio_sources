using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

/// <summary>
/// タイトルシーン管理クラス
/// </summary>
/// <remarks>タイトルシーンの挙動を管理するクラス</remarks>
public class TitleSceneManager : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;
    GameObject howToCanvas;
    GameObject masterImage;

    /** 定数 */
    private const string CANVAS_HOWTO_NAME = "HowToCanvas";
    private const string UI_IMAGE_MASTER = "MasterImage";

    private void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        howToCanvas = GameObject.Find(CANVAS_HOWTO_NAME);
        howToCanvas.SetActive(false);
        // マスター証の表示
        masterImage = GameObject.Find(UI_IMAGE_MASTER);
        masterImage.SetActive(GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_BOOL_MASTER));
        // 重力を設定する
        Physics.gravity = new Vector3(0, -2.5f, 0);
    }

    // ---------- ボタン押下イベント ----------
    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void PushDownStartButton()
    {
        // HOWTO画面表示時は対象外
        if (howToCanvas.activeSelf)
            return;

        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // ゲームシーンへ遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_GAME);
    }

    /// <summary>
    /// ランキングボタン押下時
    /// </summary>
    public void PushDownRankButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);

#if UNITY_ANDROID
        // Android: リーダーボードの表示
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GameUtil.Const.LEADER_BOARD_ID_ANDROID);
#elif UNITY_IPHONE
        // iOS: リーダーボードの表示
        Social.ShowLeaderboardUI();
#endif
    }

    /// <summary>
    /// HOWTOボタン押下時
    /// </summary>
    public void PushDownHowToButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // HOWTO画面表示
        howToCanvas.SetActive(true);
    }

    /// <summary>
    /// HOWTO閉じるボタン押下時
    /// </summary>
    public void PushDownHowToCloseButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // HOWTO画面非表示
        howToCanvas.SetActive(false);
    }
}
