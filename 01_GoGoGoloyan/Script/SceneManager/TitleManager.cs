using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using TMPro;

/// <summary>
/// タイトルシーン管理クラス
/// </summary>
public class TitleManager : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;

    /** UIオブジェクト */
    private const string START_BUTTON_NAME = "StartButton";
    private const string TIMEATTACK_BUTTON_NAME = "TimeAttackButton";
    private const string TUTORIAL_DISP_BUTTON = "TutorialDispButton";
    private const string MODE_CANVAS_NAME = "ModeCanvas";
    private const string TIMEATTACK_CANVAS_NAME = "TimeAttackCanvas";
    private const string TUTORIAL_CANVAS_NAME = "TutorialCanvas";
    private const string EPISODE1_BUTTON_NAME = "Episode1Button";
    private const string EPISODE2_BUTTON_NAME = "Episode2Button";
    private const string EPISODE3_BUTTON_NAME = "Episode3Button";
    private const string EPISODE4_BUTTON_NAME = "Episode4Button";
    private const string EPISODE5_BUTTON_NAME = "Episode5Button";
    private const string TIMEATTACK_STAGE1_BUTTON_NAME = "TimeAttackStage1Button";
    private const string TIMEATTACK_STAGE2_BUTTON_NAME = "TimeAttackStage2Button";
    private const string TIMEATTACK_STAGE3_BUTTON_NAME = "TimeAttackStage3Button";
    private const string TIMEATTACK_STAGE4_BUTTON_NAME = "TimeAttackStage4Button";
    private const string TIMEATTACK_STAGE5_BUTTON_NAME = "TimeAttackStage5Button";
    private const string CLEAR_BEST_TIME_STAGE1_TEXT_NAME = "ClearBestTimeStage1";
    private const string CLEAR_BEST_TIME_STAGE2_TEXT_NAME = "ClearBestTimeStage2";
    private const string CLEAR_BEST_TIME_STAGE3_TEXT_NAME = "ClearBestTimeStage3";
    private const string CLEAR_BEST_TIME_STAGE4_TEXT_NAME = "ClearBestTimeStage4";
    private const string CLEAR_BEST_TIME_STAGE5_TEXT_NAME = "ClearBestTimeStage5";
    private const string BACKGROUND_IMAGE_NAME = "background";
    private const string TITLE_IMAGE_NAME = "title";
    private const string GOROYAN_IMAGE_NAME = "title_goroyan";
    GameObject startButton;
    GameObject timeAttackButton;
    GameObject tutorialDispButton;
    GameObject modeCanvas;
    GameObject timeAttackCanvas;
    GameObject tutorialCanvas;
    GameObject episode1Button;
    GameObject episode2Button;
    GameObject episode3Button;
    GameObject episode4Button;
    GameObject episode5Button;
    GameObject timeAttackStage1Button;
    GameObject timeAttackStage2Button;
    GameObject timeAttackStage3Button;
    GameObject timeAttackStage4Button;
    GameObject timeAttackStage5Button;
    TextMeshProUGUI clearBestTimeStage1Text;
    TextMeshProUGUI clearBestTimeStage2Text;
    TextMeshProUGUI clearBestTimeStage3Text;
    TextMeshProUGUI clearBestTimeStage4Text;
    TextMeshProUGUI clearBestTimeStage5Text;
    Material backGroundMaterial;
    Material titleMaterial;
    Material goroyanMaterial;

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        // UI部品取得
        startButton = GameObject.Find(START_BUTTON_NAME);
        timeAttackButton = GameObject.Find(TIMEATTACK_BUTTON_NAME);
        tutorialDispButton = GameObject.Find(TUTORIAL_DISP_BUTTON);
        modeCanvas = GameObject.Find(MODE_CANVAS_NAME);
        timeAttackCanvas = GameObject.Find(TIMEATTACK_CANVAS_NAME);
        tutorialCanvas = GameObject.Find(TUTORIAL_CANVAS_NAME);
        episode1Button = GameObject.Find(EPISODE1_BUTTON_NAME);
        episode2Button = GameObject.Find(EPISODE2_BUTTON_NAME);
        episode3Button = GameObject.Find(EPISODE3_BUTTON_NAME);
        episode4Button = GameObject.Find(EPISODE4_BUTTON_NAME);
        episode5Button = GameObject.Find(EPISODE5_BUTTON_NAME);
        timeAttackStage1Button = GameObject.Find(TIMEATTACK_STAGE1_BUTTON_NAME);
        timeAttackStage2Button = GameObject.Find(TIMEATTACK_STAGE2_BUTTON_NAME);
        timeAttackStage3Button = GameObject.Find(TIMEATTACK_STAGE3_BUTTON_NAME);
        timeAttackStage4Button = GameObject.Find(TIMEATTACK_STAGE4_BUTTON_NAME);
        timeAttackStage5Button = GameObject.Find(TIMEATTACK_STAGE5_BUTTON_NAME);
        clearBestTimeStage1Text = GameObject.Find(CLEAR_BEST_TIME_STAGE1_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        clearBestTimeStage2Text = GameObject.Find(CLEAR_BEST_TIME_STAGE2_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        clearBestTimeStage3Text = GameObject.Find(CLEAR_BEST_TIME_STAGE3_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        clearBestTimeStage4Text = GameObject.Find(CLEAR_BEST_TIME_STAGE4_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        clearBestTimeStage5Text = GameObject.Find(CLEAR_BEST_TIME_STAGE5_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        backGroundMaterial = GameObject.Find(BACKGROUND_IMAGE_NAME).GetComponent<Renderer>().material;
        titleMaterial = GameObject.Find(TITLE_IMAGE_NAME).GetComponent<Renderer>().material;
        goroyanMaterial = GameObject.Find(GOROYAN_IMAGE_NAME).GetComponent<Renderer>().material;

        // スタートボタン表示
        SetStartButtonEnable();
        // ゲーム進行によってボタンの活性/非活性を切替
        episode2Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE1);
        episode3Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE2);
        episode4Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE3);
        episode5Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE4);
        timeAttackStage1Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE1);
        timeAttackStage2Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE2);
        timeAttackStage3Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE3);
        timeAttackStage4Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE4);
        timeAttackStage5Button.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE5);
        timeAttackButton.GetComponent<Button>().interactable = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE1);
        // ベストタイムを取得
        clearBestTimeStage1Text.text = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE1).ToString("n2") + " 秒";
        clearBestTimeStage2Text.text = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE2).ToString("n2") + " 秒";
        clearBestTimeStage3Text.text = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE3).ToString("n2") + " 秒";
        clearBestTimeStage4Text.text = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE4).ToString("n2") + " 秒";
        clearBestTimeStage5Text.text = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE5).ToString("n2") + " 秒";
        // 操作説明を非表示にする
        tutorialCanvas.SetActive(false);

        // タイムアタックモードをOFFにする
        GameSystemManager.timeAttackMode = false;
    }

    /// <summary>
    /// スタートボタン活性化
    /// </summary>
    private void SetStartButtonEnable()
    {
        startButton.SetActive(true);
        timeAttackButton.SetActive(true);
        tutorialDispButton.SetActive(true);
        modeCanvas.SetActive(false);
        timeAttackCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);

        // 背景の色を元に戻す
        backGroundMaterial.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
        titleMaterial.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
        goroyanMaterial.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
    }

    /// <summary>
    /// モードボタン活性化
    /// </summary>
    private void SetModeButtonEnable()
    {
        startButton.SetActive(false);
        timeAttackButton.SetActive(false);
        tutorialDispButton.SetActive(false);
        modeCanvas.SetActive(true);

        // 背景の色を暗くする
        backGroundMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        titleMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        goroyanMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
    }

    /// <summary>
    /// タイムアタックボタン活性化
    /// </summary>
    private void SetTimeAttackButtonEnable()
    {
        startButton.SetActive(false);
        timeAttackButton.SetActive(false);
        tutorialDispButton.SetActive(false);
        timeAttackCanvas.SetActive(true);

        // 背景の色を暗くする
        backGroundMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        titleMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        goroyanMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
    }

    /// <summary>
    /// 操作説明表示画面の活性化
    /// </summary>
    private void SetTutorialButtonEnable()
    {
        startButton.SetActive(false);
        timeAttackButton.SetActive(false);
        tutorialDispButton.SetActive(false);
        tutorialCanvas.SetActive(true);

        // 背景の色を暗くする
        backGroundMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        titleMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
        goroyanMaterial.color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
    }

    /// <summary>
    /// エピソードボタン押下時共通処理
    /// </summary>
    /// <param name="episodeButton">エピソードボタン</param>
    /// <param name="eventSceneParam">遷移先イベントパラメータ</param>
    private void EpisodeButtonDown(GameObject episodeButton, string eventSceneParam)
    {
        // ボタンが非活性なら処理終了
        if (!episodeButton.GetComponent<Button>().interactable)
            return;

        // イベントシーンへ遷移
        SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_EVENT).AsObservable()
            .Subscribe(_ =>
            {
                EventManager eventManager = FindObjectOfType<EventManager>() as EventManager;
                eventManager.loadEventParam = eventSceneParam;
            });
    }

    // ---------- ボタン押下時処理 ----------
    /// <summary>
    /// タイムアタックボタン押下共通処理
    /// </summary>
    /// <param name="timeAttackButton">タイムアタックボタンオブジェクト</param>
    /// <param name="sceneName">遷移先シーン名</param>
    private void TimeAttackButtonDown(GameObject timeAttackButton, string sceneName)
    {
        // ボタンが非活性なら処理終了
        if (!timeAttackButton.GetComponent<Button>().interactable)
            return;

        // タイムアタックモードをONにしてステージへ遷移
        GameSystemManager.timeAttackMode = true;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// スタートボタン押下
    /// </summary>
    public void StartButtonDown()
    {
        playClickSound();
        // モードボタン表示
        SetModeButtonEnable();
    }

    /// <summary>
    /// エピソード１ボタン押下
    /// </summary>
    public void Episode1ButtonDown()
    {
        playClickSound();
        EpisodeButtonDown(episode1Button, GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_START);
    }

    /// <summary>
    /// エピソード２ボタン押下
    /// </summary>
    public void Episode2ButtonDown()
    {
        playClickSound();
        EpisodeButtonDown(episode2Button, GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_START);
    }

    /// <summary>
    /// エピソード３ボタン押下
    /// </summary>
    public void Episode3ButtonDown()
    {
        playClickSound();
        EpisodeButtonDown(episode3Button, GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_START);
    }

    /// <summary>
    /// エピソード４ボタン押下
    /// </summary>
    public void Episode4ButtonDown()
    {
        playClickSound();
        EpisodeButtonDown(episode4Button, GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_START);
    }

    /// <summary>
    /// エピソード５ボタン押下
    /// </summary>
    public void Episode5ButtonDown()
    {
        playClickSound();
        EpisodeButtonDown(episode5Button, GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_START);
    }

    /// <summary>
    /// タイムアタックボタン押下
    /// </summary>
    public void TimeAttackStartButtonDown()
    {
        playClickSound();
        // モードボタン表示
        SetTimeAttackButtonEnable();
    }

    /// <summary>
    /// タイムアタックステージ１ボタン押下
    /// </summary>
    public void TimeAttackStage1ButtonDown()
    {
        playClickSound();
        TimeAttackButtonDown(timeAttackStage1Button, GameUtil.Const.SCENE_NAME_STAGE1);
    }

    /// <summary>
    /// タイムアタックステージ２ボタン押下
    /// </summary>
    public void TimeAttackStage2ButtonDown()
    {
        playClickSound();
        TimeAttackButtonDown(timeAttackStage2Button, GameUtil.Const.SCENE_NAME_STAGE2);
    }

    /// <summary>
    /// タイムアタックステージ３ボタン押下
    /// </summary>
    public void TimeAttackStage3ButtonDown()
    {
        playClickSound();
        TimeAttackButtonDown(timeAttackStage3Button, GameUtil.Const.SCENE_NAME_STAGE3);
    }

    /// <summary>
    /// タイムアタックステージ４ボタン押下
    /// </summary>
    public void TimeAttackStage4ButtonDown()
    {
        playClickSound();
        TimeAttackButtonDown(timeAttackStage4Button, GameUtil.Const.SCENE_NAME_STAGE4);
    }

    /// <summary>
    /// タイムアタックステージ５ボタン押下
    /// </summary>
    public void TimeAttackStage5ButtonDown()
    {
        playClickSound();
        TimeAttackButtonDown(timeAttackStage5Button, GameUtil.Const.SCENE_NAME_STAGE5);
    }

    /// <summary>
    /// 遊び方ボタン押下
    /// </summary>
    public void TutorialDispButtonDown()
    {
        playClickSound();
        SetTutorialButtonEnable();
    }

    /// <summary>
    /// 戻るボタン押下時
    /// </summary>
    public void BackButtonDown()
    {
        playClickSound();
        SetStartButtonEnable();
    }

    /// <summary>
    /// クリック音再生
    /// </summary>
    public void playClickSound()
    {
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
    }
}
