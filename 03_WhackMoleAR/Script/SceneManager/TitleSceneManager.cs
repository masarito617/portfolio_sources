using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using GooglePlayGames;

/// <summary>
/// タイトルシーン管理クラス
/// </summary>
/// <remarks>タイトルシーンの挙動を管理するクラス</remarks>
public class TitleSceneManager : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;

    /** 定数 */
    private const string UI_CANVAS_MODE = "ModeCanvas";
    private const string UI_IMAGE_MASK = "MaskImage";

    /** UI部品 */
    GameObject modeCanvas;
    GameObject maskImage;

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        modeCanvas = GameObject.Find(UI_CANVAS_MODE);
        maskImage = GameObject.Find(UI_IMAGE_MASK);
        modeCanvas.SetActive(false);
        maskImage.SetActive(false);
    }

    void Update()
    {
        // 処理なし
    }

    // --------------- UIイベントトリガー ---------------
    /// <summary>
    /// スタートボタン押下
    /// </summary>
    public void PushStartButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // MODECANVASの表示
        modeCanvas.SetActive(true);
        // MASKIMAGEの表示
        maskImage.SetActive(true);
    }

    /// <summary>
    /// EASYボタン押下
    /// </summary>
    public void PushModeEasyButton()
    {
        PushModeButton(GameUtil.Const.MODE_EASY);
    }

    /// <summary>
    /// NORMALボタン押下
    /// </summary>
    public void PushModeNormalButton()
    {
        PushModeButton(GameUtil.Const.MODE_NORMAL);
    }

    /// <summary>
    /// HARDボタン押下
    /// </summary>
    public void PushModeHardButton()
    {
        PushModeButton(GameUtil.Const.MODE_HARD);
    }

    /// <summary>
    /// 戻るボタン押下
    /// </summary>
    public void PushBackButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // MODECANVASの非表示
        modeCanvas.SetActive(false);
        // MASKIMAGEの表示
        maskImage.SetActive(false);
    }
    
    /// <summary>
    /// モードボタン共通処理
    /// </summary>
    /// <param name="mode">モード</param>
    public void PushModeButton(string mode)
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // GameSceneへ遷移
        SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_GAME).AsObservable()
            .Subscribe(_ =>
            {
                GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>() as GameSceneManager;
                gameSceneManager.playMode = mode;
            });
    }

    /// <summary>
    /// ランクボタン押下
    /// </summary>
    public void PushRankButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);

#if UNITY_ANDROID
        // Android: リーダーボードの表示
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
#elif UNITY_IPHONE
        // iOS: リーダーボードの表示
        Social.ShowLeaderboardUI();
#endif
    }

    /// <summary>
    /// Appボタン押下
    /// </summary>
    public void PushAppButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // Appsページを開く
        Application.OpenURL("【AppsページURL】");
    }
}
