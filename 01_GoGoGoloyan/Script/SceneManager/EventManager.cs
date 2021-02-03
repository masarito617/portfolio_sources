using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using UniRx;

/// <summary>
/// 会話イベントシーン管理クラス
/// </summary>
/// <remarks>会話イベントシーンの挙動を管理するクラス</remarks>
public class EventManager : MonoBehaviour
{
    /** 定数値 */
    const string STAGE1_BACK_NAME = "talk_stage1_back"; // ステージ１背景画像名
    const string STAGE2_BACK_NAME = "talk_stage2_back"; // ステージ２背景画像名
    const string STAGE4_BACK_NAME = "talk_stage4_back"; // ステージ４背景画像名
    const string STAGE5_BACK_NAME = "talk_stage5_back"; // ステージ５背景画像名

    /** コンポーネント */
    AssetsManager assetsManager;
    Flowchart flowchart;
    GameObject stage1BackImage;
    GameObject stage2BackImage;
    GameObject stage4BackImage;
    GameObject stage5BackImage;

    /** UI部品 */
    private string SKIP_BUTTON_NAME = "SkipButton";
    GameObject skipButton;

    /** 変数 */
    public string loadEventParam; // 読込イベント名(遷移元で渡されるゴロ)
    bool eventStarted;

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        flowchart = FindObjectOfType<Flowchart>();
        stage1BackImage = GameObject.Find(STAGE1_BACK_NAME);
        stage2BackImage = GameObject.Find(STAGE2_BACK_NAME);
        stage4BackImage = GameObject.Find(STAGE4_BACK_NAME);
        stage5BackImage = GameObject.Find(STAGE5_BACK_NAME);
        skipButton = GameObject.Find(SKIP_BUTTON_NAME);
    }

    void Update()
    {
        // パラメータを受け取るまで処理しない
        if (loadEventParam == null)
            return;

        // イベントシーンスタート
        if (!eventStarted)
        {
            flowchart.SendFungusMessage(loadEventParam);
            eventStarted = true;
            SetInitBackground(loadEventParam);
        }

        // イベント終了時：タイトルへ戻る場合
        if (flowchart.GetVariable<BooleanVariable>(GameUtil.Const.FUNGUS_KEY_BACK_END).Value)
            SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);

        // イベント終了時：ゲームシーンへ遷移する場合
        if (flowchart.GetVariable<BooleanVariable>(GameUtil.Const.FUNGUS_KEY_EVENT_END).Value)
        {
            // 読み込んだイベントで遷移先を指定
            switch (loadEventParam)
            {
                // エピソードN開始 -> ステージN
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_START:
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_STAGE1);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_START:
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_STAGE2);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_START:
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_STAGE3);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_START:
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_STAGE4);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_START:
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_STAGE5);
                    break;
                // エピソードN終了 -> エピソードN+1開始
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_END:
                    LoadEpisode(GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_START);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END:
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END_2:
                    LoadEpisode(GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_START);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_END:
                    LoadEpisode(GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_START);
                    break;
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_END:
                    LoadEpisode(GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_START);
                    break;
                // エンディング
                case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_END:
                    assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_STOP);
                    SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_END);
                    break;
            } 
        }

        // 再生中のオーディオと異なる曲が指定されたら再生
        string playBGMKey = flowchart.GetVariable<StringVariable>(GameUtil.Const.FUNGUS_KEY_PLAY_BGM).Value;
        if (!string.IsNullOrEmpty(playBGMKey))
        {
            if (assetsManager.getPlayingAudio() != assetsManager.getAudioClip(playBGMKey))
            {
                assetsManager.PlayBGM(playBGMKey);
            }
            flowchart.SetStringVariable(GameUtil.Const.FUNGUS_KEY_PLAY_BGM, null);
        }

        // 効果音が指定されたら再生
        string playSEKey = flowchart.GetVariable<StringVariable>(GameUtil.Const.FUNGUS_KEY_PLAY_SE).Value;
        if (!string.IsNullOrEmpty(playSEKey))
        {
            assetsManager.PlayOneShot(playSEKey);
            flowchart.SetStringVariable(GameUtil.Const.FUNGUS_KEY_PLAY_SE, null);
        }

        // 背景の切替処理
        string setBackKey = flowchart.GetVariable<StringVariable>(GameUtil.Const.FUNGUS_KEY_SET_BACKGROUD).Value;
        if (!string.IsNullOrEmpty(setBackKey))
        {
            SwitchBackground(setBackKey);
            flowchart.SetStringVariable(GameUtil.Const.FUNGUS_KEY_SET_BACKGROUD, null);
        }
    }

    /// <summary>
    /// イベント読込処理
    /// </summary>
    /// <param name="eventParam">読込イベントパラメータ</param>
    private void LoadEpisode(string eventParam)
    {
        // ステージ２をクリアしているかつイベントの境目の場合、一定確率で動画広告を表示する
        if (GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE2) && Random.Range(1, 5) == 1)
        {
            // BGM停止
            assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_STOP);
            // 広告シーン遷移
            SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_ADMOB).AsObservable()
                .Subscribe(_ =>
                {
                    GoogleAdmobInterstitial googleAdmobInterstitial = FindObjectOfType<GoogleAdmobInterstitial>() as GoogleAdmobInterstitial;
                    googleAdmobInterstitial.loadEventParam = eventParam;
                });
            return;
        }
        // 読込イベントを設定
        loadEventParam = eventParam;
        eventStarted = false;
        skipButton.SetActive(true);
        assetsManager.PlayFirstBGM();
    }

    /// <summary>
    /// 背景の切替え
    /// </summary>
    /// <param name="eventParam">イベントパラメータ</param>
    void SetInitBackground(string eventParam)
    {
        // イベントによって初期背景を設定
        switch (eventParam)
        {
            // エピソード１：森
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_START:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_END:
                SwitchBackground(STAGE1_BACK_NAME);
                break;
            // エピソード２：海
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_START:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END_2:
                SwitchBackground(STAGE2_BACK_NAME);
                break;
            // エピソード３：海〜宇宙
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_START:
                SwitchBackground(STAGE2_BACK_NAME);
                break;
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_END:
                SwitchBackground(STAGE4_BACK_NAME);
                break;
            // エピソード４：宇宙〜バナナン星
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_START:
                SwitchBackground(STAGE4_BACK_NAME);
                break;
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_END:
                SwitchBackground(STAGE5_BACK_NAME);
                break;
            // エピソード５：バナナン星
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_START:
                SwitchBackground(STAGE5_BACK_NAME);
                break;
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_END:
                SwitchBackground(STAGE5_BACK_NAME);
                break;
        }
    }

    /// <summary>
    /// 背景画像の切替
    /// </summary>
    /// <param name="backName">背景画像名</param>
    void SwitchBackground(string backName)
    {
        // 一旦無効化
        stage1BackImage.SetActive(false);
        stage2BackImage.SetActive(false);
        stage4BackImage.SetActive(false);
        stage5BackImage.SetActive(false);
        // 背景画像を切替え
        switch (backName)
        {
            case STAGE1_BACK_NAME:
                stage1BackImage.SetActive(true);
                break;
            case STAGE2_BACK_NAME:
                stage2BackImage.SetActive(true);
                break;
            case STAGE4_BACK_NAME:
                stage4BackImage.SetActive(true);
                break;
            case STAGE5_BACK_NAME:
                stage5BackImage.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// スキップボタン押下時
    /// </summary>
    public void PushSkipButton()
    {
        // スキップボタン無効化
        skipButton.SetActive(false);
        // 現在のブロックを停止する
        flowchart.StopAllBlocks();
        // 再生中のイベントに応じてスキップ先を選択
        switch (loadEventParam)
        {
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_END:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END_2:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_END:
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_END:
                // コンティニューイベント呼び出し
                flowchart.ExecuteBlock(GameUtil.Const.FUNGUS_CONTINUE_BLOCK);
                break;
            case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_END:
            default:
                // 会話終了
                flowchart.ExecuteBlock(GameUtil.Const.FUNGUS_END_BLOCK);
                break;
        }
    }
}
