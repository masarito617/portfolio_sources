using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UniRx;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
/// <remarks>ゲームシーンの挙動を管理するクラス</remarks>
[RequireComponent(typeof(ARRaycastManager))]
public class GameSceneManager : MonoBehaviour
{
    /** コンポーネント */
    ARPlaneManager aRPlaneManager;
    AssetsManager assetsManager;
    ResultMoleController resultMoleController;

    /** 設定値 */
    public GameObject mole_PlacedPrefab;
    public GameObject molegold_PlacedPrefab;
    public GameObject majiro_PlacedPrefab;
    public ARSession session;
    public GameObject sessionPrefab;
    public string playMode;

    /** 定数 */
    private const string UI_CANVAS_READY = "ReadyCanvas";
    private const string UI_BUTTON_START = "GameStartButton";
    private const string UI_IMAGE_AR_SEARCH = "ArSearchImage";
    private const string UI_BUTTON_AR_RELOAD = "ArReloadButton";

    private const string UI_CANVAS_GAME = "GameCanvas";
    private const string UI_TEXT_SCORE = "ScoreText";
    private const string UI_TEXT_COUNT = "CountText";
    private const string UI_TEXT_READY = "ReadyText";
    private const string UI_SLIDER_TIMER = "TimerSlider";

    private const string UI_CANVAS_RESULT = "ResultCanvas";
    private const string UI_TEXT_RESULT_SCORE = "ResultScoreText";

    /** UI部品 */
    GameObject readyCanvas;
    GameObject gameStartButton;
    GameObject arSearchImage;
    GameObject arReloadButton;

    GameObject gameCanvas;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI countText;
    TextMeshProUGUI readyText;
    Slider timerSlider;

    GameObject resultCanvas;
    TextMeshProUGUI resultScoreText;

    /** 変数 */
    private float prevFingerId = -1;
    private int moleScore = 100;       // モグラのスコア
    private int molegoldScore = 500;   // モグラゴールドのスコア
    private int majiroScore = -300;    // マジロのスコア
    private int score = 0;             // 合計スコア
    private float countBaseTime = 45;  // ゲーム時間
    private float countTime;           // ゲーム残り時間
    private bool dispResult;           // 結果画面表示フラグ
    private bool endCountDown;         // カウントダウン終了フラグ
    
    private TrackableCollection<ARPlane> playTrackableCollection;   // 検出したTrackableCollection
    private List<ARPlane> playArPlaneList;                          // ARPlaneリスト
    private Dictionary<string, List<AppearInfoBean>> appearInfoMap; // モグラ出現MAP

    // ステータス
    public enum State
    {
        INIT, // 初期状態
        GAME, // ゲーム状態
        END   // 終了状態
    };
    public State state = State.INIT;
    public State nextState = State.INIT;

    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// ステータス変更処理
    /// </summary>
    /// <param name="nextState">変更先ステータス</param>
    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    private void Awake()
    {
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        assetsManager = FindObjectOfType<AssetsManager>();
        resultMoleController = FindObjectOfType<ResultMoleController>();

        // UI部品設定
        readyCanvas = GameObject.Find(UI_CANVAS_READY);
        gameStartButton = GameObject.Find(UI_BUTTON_START);
        arSearchImage = GameObject.Find(UI_IMAGE_AR_SEARCH);
        arReloadButton = GameObject.Find(UI_BUTTON_AR_RELOAD);
        gameStartButton.GetComponent<Button>().interactable = false;
        arReloadButton.GetComponent<Button>().interactable = false;

        gameCanvas = GameObject.Find(UI_CANVAS_GAME);
        scoreText = GameObject.Find(UI_TEXT_SCORE).GetComponent<TextMeshProUGUI>();
        countText = GameObject.Find(UI_TEXT_COUNT).GetComponent<TextMeshProUGUI>();
        readyText = GameObject.Find(UI_TEXT_READY).GetComponent<TextMeshProUGUI>();
        timerSlider = GameObject.Find(UI_SLIDER_TIMER).GetComponent<Slider>();
        scoreText.text = score.ToString();
        countText.text = countBaseTime.ToString("F0");
        readyText.text = "";
        timerSlider.value = countBaseTime;

        resultCanvas = GameObject.Find(UI_CANVAS_RESULT);
        resultScoreText = GameObject.Find(UI_TEXT_RESULT_SCORE).GetComponent<TextMeshProUGUI>();

        // ModeCanvas表示
        DispCanvas(readyCanvas);

        // プレハブの大きさを決める
        mole_PlacedPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        molegold_PlacedPrefab.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        majiro_PlacedPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    
    private void Update()
    {
        // 遷移パラメタを受け取るまでは処理しない
        if (playMode == null)
            return;
        // モードに応じてもぐら出現情報MAPを設定
        if (appearInfoMap == null)
            InitAppearInfoMap(playMode);

        // ステートに応じた処理
        switch (state)
        {
            case State.INIT:
                InitUpdate();
                break;
            case State.GAME:
                GameUpdate();
                break;
            case State.END:
                EndUpdate();
                break;
        }
        // ステート遷移処理
        if (state != nextState)
        {
            state = nextState;
            switch (state)
            {
                case State.INIT:
                    InitStart();
                    break;
                case State.GAME:
                    GameStart();
                    break;
                case State.END:
                    EndStart();
                    break;
            }
        }
    }

    /// <summary>
    /// タッチ判定処理
    /// </summary>
    /// <param name="touchPosition">タッチされた位置</param>
    /// <returns>タッチフラグ(ON：タッチされている OFF：タッチされていない)</returns>
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        // スマホ：タッチされていて前回のfingerIDと異なる場合
        if (Input.touchCount > 0 && prevFingerId != Input.GetTouch(0).fingerId)
        {
            prevFingerId = Input.GetTouch(0).fingerId;
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        // エディター確認用：マウスクリック時
        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
            return true;
        }
        touchPosition = default;
        return false;
    }

    /// <summary>
    /// もぐら出現情報MAP更新処理
    /// </summary>
    /// <param name="key">キー(出現させる秒数)</param>
    /// <param name="bean">出現情報</param>
    private void SetAppearInfoMap(string key, AppearInfoBean bean)
    {
        List<AppearInfoBean> list;
        if (appearInfoMap.ContainsKey(key))
        {
            list = appearInfoMap[key];
            list.Add(bean);
            appearInfoMap[key] = list;
        }
        else
        {
            list = new List<AppearInfoBean>();
            list.Add(bean);
            appearInfoMap.Add(key, list);
        }
    }

    /// <summary>
    /// もぐら出現情報MAP初期化処理
    /// </summary>
    /// <param name="mode">モード</param>
    private void InitAppearInfoMap(string mode)
    {
        // MAPを初期化
        appearInfoMap = new Dictionary<string, List<AppearInfoBean>>();

        // *** ゲームデザインシートより作成 ***
        switch (mode)
        {
            case GameUtil.Const.MODE_EASY:
                SetAppearInfoMap("45", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 5.0f, 1.0f));
                SetAppearInfoMap("41", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 5.0f, 1.0f));
                SetAppearInfoMap("36", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 5.0f, 1.0f));
                SetAppearInfoMap("35", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("32", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 5.0f, 2.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 3.0f, 1.0f));
                SetAppearInfoMap("29", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("28", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 5.0f, 1.0f));
                SetAppearInfoMap("25", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("20", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 5.0f, 1.0f));
                SetAppearInfoMap("19", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 5.0f, 1.0f));
                SetAppearInfoMap("18", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("13", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 5.0f, 1.0f));
                SetAppearInfoMap("12", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("11", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 1.0f));
                SetAppearInfoMap("6", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 5.0f, 1.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("2", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                break;
            case GameUtil.Const.MODE_NORMAL:
                SetAppearInfoMap("43", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("43", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 1.0f));
                SetAppearInfoMap("42", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 2.0f));
                SetAppearInfoMap("39", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 3.0f, 1.0f));
                SetAppearInfoMap("37", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 2.0f));
                SetAppearInfoMap("35", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                SetAppearInfoMap("35", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("34", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 1.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 2.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("27", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("27", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 5.0f, 1.0f));
                SetAppearInfoMap("25", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 2.0f));
                SetAppearInfoMap("25", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                SetAppearInfoMap("24", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                SetAppearInfoMap("22", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("22", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 2.0f));
                SetAppearInfoMap("22", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("20", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 2.0f));
                SetAppearInfoMap("17", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 3.0f, 1.0f));
                SetAppearInfoMap("17", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                SetAppearInfoMap("17", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("15", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 2.0f));
                SetAppearInfoMap("15", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 2.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 1.0f));
                SetAppearInfoMap("7", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 2.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("3", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 1.0f, 1.0f));
                break;
            case GameUtil.Const.MODE_HARD:
                SetAppearInfoMap("45", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("45", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("45", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("45", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("42", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("42", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("40", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 0.5f, 2.0f));
                SetAppearInfoMap("40", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("40", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 2.0f));
                SetAppearInfoMap("40", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 3.0f, 1.0f));
                SetAppearInfoMap("39", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 1.0f));
                SetAppearInfoMap("38", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("37", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("37", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 5.0f, 1.0f));
                SetAppearInfoMap("36", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("35", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 2.0f));
                SetAppearInfoMap("35", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("33", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("32", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 0.5f, 1.0f));
                SetAppearInfoMap("30", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("29", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("29", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 3, 3.0f, 1.0f));
                SetAppearInfoMap("27", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("27", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("26", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 3.0f, 2.0f));
                SetAppearInfoMap("25", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("25", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("23", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 0.5f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 3.0f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 5.0f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 3.0f, 1.0f));
                SetAppearInfoMap("21", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("20", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 2.0f));
                SetAppearInfoMap("18", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("18", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("18", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 2.0f));
                SetAppearInfoMap("15", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 0.5f, 2.0f));
                SetAppearInfoMap("15", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("13", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 1.0f, 1.0f));
                SetAppearInfoMap("13", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 2.0f));
                SetAppearInfoMap("13", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 3, 5.0f, 1.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 1.0f, 2.0f));
                SetAppearInfoMap("10", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("9", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 3.0f, 1.0f));
                SetAppearInfoMap("9", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 1.0f, 1.0f));
                SetAppearInfoMap("9", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 1, 0.5f, 2.0f));
                SetAppearInfoMap("9", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("7", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 2, 3.0f, 1.0f));
                SetAppearInfoMap("6", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 0.5f, 2.0f));
                SetAppearInfoMap("6", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 3, 5.0f, 1.0f));
                SetAppearInfoMap("6", new AppearInfoBean(GameUtil.Const.TAG_NAME_MAJIRO, 1, 3.0f, 2.0f));
                SetAppearInfoMap("5", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLEGOLD, 1, 0.5f, 2.0f));
                SetAppearInfoMap("4", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 3, 1.0f, 1.0f));
                SetAppearInfoMap("3", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 1.0f));
                SetAppearInfoMap("2", new AppearInfoBean(GameUtil.Const.TAG_NAME_MOLE, 2, 0.5f, 2.0f));
                break;
        }
    }

    // --------------- 初期状態 ---------------
    void InitStart()
    {
        // 処理なし
    }

    void InitUpdate()
    {
        // プレーンを取得
        ARPlaneManager aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        TrackableCollection<ARPlane> trackableCollection = aRPlaneManager.trackables;
        // プレーンを取得できた場合、ボタンを活性化する
        if (trackableCollection.count > 0)
        {
            gameStartButton.GetComponent<Button>().interactable = true;
            arReloadButton.GetComponent<Button>().interactable = true;
            arSearchImage.SetActive(false);
            playTrackableCollection = trackableCollection;
        }
    }

    // --------------- ゲーム状態 ---------------
    void GameStart()
    {
        countTime = countBaseTime;
        countText.text = countTime.ToString();
        timerSlider.value = countTime;
        // カウントダウン開始
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_STOP, false);
        endCountDown = false;
        StartCoroutine(CountDownCoroutine());
    }

    void GameUpdate()
    {
        // カウントダウン終了まで何もしない
        if (!endCountDown)
            return;

        // モグラ生成処理
        GenetateMole();

        // モグラタップを検知
        DetectionMoleTap();

        // 残り時間を計算
        countTime -= Time.deltaTime;
        countText.text = countTime.ToString("F0");
        timerSlider.value = countTime;
        if (countTime < 0)
        {
            // 残り時間が０になったら結果画面に遷移
            countText.text = "0";
            timerSlider.value = 0;
            ChangeState(State.END);
        }
    }

    /// <summary>
    /// カウントダウンコルーチン
    /// </summary>
    IEnumerator CountDownCoroutine()
    {
        // カウントダウン開始
        readyText.gameObject.SetActive(true);

        // カウントダウン処理
        yield return new WaitForSeconds(1.0f);
        readyText.text = ConvUtil.ConvFoolCoolFont("3");
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_COUNT);
        yield return new WaitForSeconds(1.0f);
        readyText.text = ConvUtil.ConvFoolCoolFont("2");
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_COUNT);
        yield return new WaitForSeconds(1.0f);
        readyText.text = ConvUtil.ConvFoolCoolFont("1");
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_COUNT);
        yield return new WaitForSeconds(1.0f);
        readyText.text = ConvUtil.ConvFoolCoolFont("GO!");
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_GO);
        yield return new WaitForSeconds(1.0f);

        // カウントダウン終了
        assetsManager.PlayFirstBGM();
        endCountDown = true;
        readyText.text = "";
        readyText.gameObject.SetActive(false);
    }

    // --------------- 終了状態 ---------------
    void EndStart()
    {
        // BGMを停止
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_STOP, false);
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_WHISTLE);
        // BGM再生
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_MOGCLE, false);
        // 結果画面を数秒後に表示
        dispResult = false;
        Invoke("DispResult", 2.3f);
    }

    void EndUpdate()
    {
        // 結果画面が表示されるまで処理なし
        if (!dispResult)
            return;
        // テキストを前面から押し出す
        if (resultScoreText.GetComponent<RectTransform>().localScale.x > 1.0f)
            resultScoreText.GetComponent<RectTransform>().localScale -= new Vector3(0.1f, 0.12f, 0);

    }

    /// <summary>
    /// 結果画面表示
    /// </summary>
    private void DispResult()
    {
        // 結果画面表示
        gameCanvas.SetActive(false);
        resultCanvas.SetActive(true);
        resultScoreText.text = ConvUtil.ConvFoolCoolFont(score.ToString());
        // 結果モグラを設定
        resultMoleController.SetResultMole(playMode, score);
        // テキストフェード用に拡大
        resultScoreText.GetComponent<RectTransform>().localScale = new Vector3(5.0f, 6.0f, 1.0f);
        // スコアの送信
        SendScore(score);
        // 結果画面表示フラグをON
        dispResult = true;
    }

    // --------------- 共通処理 ---------------
    /// <summary>
    /// Canvas表示処理
    /// </summary>
    /// <param name="dispCanvas">表示対象のCanvas</param>
    private void DispCanvas(GameObject dispCanvas)
    {
        // 全てのCanvasを非表示
        readyCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        resultCanvas.SetActive(false);
        // 指定Canvasを表示
        dispCanvas.SetActive(true);
    }

    /// <summary>
    /// もぐら出現処理
    /// </summary>
    private void GenetateMole()
    {
        // 時間をキーに変換
        string key = countTime.ToString("F0");
        if (!appearInfoMap.ContainsKey(key))
            return;

        // もぐら出現情報Mapよりキーに紐づくBeanリストを取得
        List<AppearInfoBean> appearInfoBeanList = appearInfoMap[key];
        foreach (AppearInfoBean bean in appearInfoBeanList)
        {
            // Beanに設定されている回数分生成する
            for (int i = 0; i < bean.Count; i++)
            {
                // 種類に応じて生成
                GameObject generateMole = null;
                switch (bean.Kind)
                {
                    case GameUtil.Const.TAG_NAME_MOLE:
                        generateMole = GenerateMoleObj(mole_PlacedPrefab);
                        break;
                    case GameUtil.Const.TAG_NAME_MOLEGOLD:
                        generateMole = GenerateMoleObj(molegold_PlacedPrefab);
                        break;
                    case GameUtil.Const.TAG_NAME_MAJIRO:
                        generateMole = GenerateMoleObj(majiro_PlacedPrefab);
                        break;
                }
                // Beanからエスケープタイムとアニメーション速度を設定
                MoleController mc = generateMole.GetComponent<MoleController>();
                mc.animSpeed = bean.AnimSpeed;
                mc.hideBaseTime = bean.EscapeTime;
            }
        }

        // 時間に対して一度のみ生成するためキーを削除
        appearInfoMap.Remove(key);
    }

    /// <summary>
    /// もぐらオブジェクト生成処理
    /// </summary>
    /// <param name="placedPrefab">もぐらPrefab</param>
    /// <returns>もぐらオブジェクト</returns>
    private GameObject GenerateMoleObj(GameObject placedPrefab)
    {
        // もぐらをプレーンの上に生成
        ARPlane alPlane = playArPlaneList[Random.Range(0, playArPlaneList.Count)];
        Transform planeTransform = alPlane.gameObject.transform;
        // ランダム位置に生成
        float width = alPlane.gameObject.GetComponent<Renderer>().bounds.size.x;
        float height = alPlane.gameObject.GetComponent<Renderer>().bounds.size.z;
        float randomWidth = Random.Range(-width / 2, width / 2);
        float randomHeight = Random.Range(-height / 2, height / 2);
        if (planeTransform.rotation.eulerAngles.x >= 45)
        {
            // オブジェクトの角度が45度以上（垂直）
            // 位置 x: 平面 + ランダム, y: 平面 + ランダム, z: 平面
            // 回転 x: 平面 * -1,     y: 平面 + 180,     z:平面
            return Instantiate(placedPrefab,
                new Vector3(planeTransform.position.x + randomWidth, planeTransform.position.y + randomHeight, planeTransform.position.z),
                Quaternion.Euler(planeTransform.rotation.eulerAngles.x * -1, planeTransform.rotation.eulerAngles.y + 180.0f, planeTransform.rotation.eulerAngles.z));
        }
        else
        {
            // オブジェクトの角度が0度（水平）
            // 位置 x: 平面 + ランダム, y: 平面,       z: 平面 + ランダム
            // 回転 x: 平面 * -1,     y: 平面 + 180,  z:平面
            return Instantiate(placedPrefab,
                new Vector3(planeTransform.position.x + randomWidth, planeTransform.position.y, planeTransform.position.z + randomHeight),
                Quaternion.Euler(planeTransform.rotation.eulerAngles.x * -1, planeTransform.rotation.eulerAngles.y + 180.0f, planeTransform.rotation.eulerAngles.z));
        }
    }

    /// <summary>
    /// もぐらタップ検知処理
    /// </summary>
    private void DetectionMoleTap()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // タップ対象を全て取得
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit[] hitList = Physics.RaycastAll(ray);
        if (hitList == null)
            return;

        // モグラかマジロがタップされた場合
        foreach (RaycastHit hit in hitList)
        {
            // モグラ類のみが対象
            MoleController mc = hit.transform.root.gameObject.GetComponent<MoleController>();
            if (mc == null || mc.delete)
                continue;
            // ヒット・スコア加算処理
            switch (mc.kind)
            {
                case MoleController.KIND.MOLE:
                    mc.Hit();
                    AddScore(moleScore);
                    break;
                case MoleController.KIND.MOLEGOLD:
                    mc.Hit();
                    AddScore(molegoldScore);
                    break;
                case MoleController.KIND.MAJIRO:
                    mc.Hit();
                    AddScore(majiroScore);
                    break;
            }
        }
    }

    /// <summary>
    /// スコアの加算処理
    /// </summary>
    /// <param name="addScore">加算するスコア</param>
    private void AddScore(int addScore)
    {
        score += addScore;
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// ハイスコアの送信処理
    /// </summary>
    /// <param name="highScore">ハイスコア</param>
    public void SendScore(int highScore)
    {
#if UNITY_ANDROID
        // Android: LeaderBoardに登録
        string leaderBoardID = null;
        switch (playMode)
        {
            case GameUtil.Const.MODE_EASY:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_EASY_ANDROID;
                break;
            case GameUtil.Const.MODE_NORMAL:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_NORMAL_ANDROID;
                break;
            case GameUtil.Const.MODE_HARD:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_HARD_ANDROID;
                break;
        }

        Social.ReportScore(highScore, leaderBoardID, (bool success) => {
            if (success)
            {
                // 送信が成功した時の処理
                Debug.Log("Score Regist Successed!!");
            }
            else
            {
                // 送信が失敗した時の処理
                Debug.Log("Score Regist Failed!!");
            }
        });
#elif UNITY_IPHONE
        // iOS: LeaderBoardに登録
        string leaderBoardID = null;
        switch (playMode)
        {
            case GameUtil.Const.MODE_EASY:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_EASY_IOS;
                break;
            case GameUtil.Const.MODE_NORMAL:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_NORMAL_IOS;
                break;
            case GameUtil.Const.MODE_HARD:
                leaderBoardID = GameUtil.Const.LEADER_BOARD_ID_HARD_IOS;
                break;
        }
        int score = 0;
        if (int.TryParse(highScore.ToString(), out score))
        {
            Social.ReportScore(score, leaderBoardID, success => {
                if (success)
                {
                    // 送信が成功した時の処理
                    Debug.Log("Score Regist Successed!!");
                }
                else
                {
                    // 送信が失敗した時の処理
                    Debug.Log("Score Regist Failed!!");
                }
            });
        }
#endif
    }

    // --------------- UIイベントトリガー ---------------
    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void PushGameStartButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // ゲーム状態に遷移
        ChangeState(State.GAME);
        // プレーンをリストに変換
        playArPlaneList = new List<ARPlane>();
        foreach (ARPlane aRPlane in playTrackableCollection)
        {
            playArPlaneList.Add(aRPlane);
        }
        // ARPlaneの色を透明にする
        foreach (ARPlane aRPlane in playArPlaneList)
        {
            Color color = aRPlane.gameObject.GetComponent<Renderer>().material.color;
            color.a = 0;
            aRPlane.gameObject.GetComponent<Renderer>().material.color = color;
        }
        // GameCanvas表示
        DispCanvas(gameCanvas);
        // PlaneManagerを無効化
        aRPlaneManager.detectionMode = PlaneDetectionMode.None;
    }

    /// <summary>
    /// ARリロードボタン押下時
    /// </summary>
    public void PushArReloadButton()
    {
        // スタートボタンを非活性
        gameStartButton.GetComponent<Button>().interactable = false;
        arReloadButton.GetComponent<Button>().interactable = false;
        arSearchImage.SetActive(true);
        // ARセッションの再読込
        if (session != null)
        {
            StartCoroutine(DoReload());
        }
    }

    /// <summary>
    /// ARセッションの再読込コルーチン
    /// </summary>
    IEnumerator DoReload()
    {
        Destroy(session.gameObject);
        yield return null;

        if (sessionPrefab != null)
        {
            session = Instantiate(sessionPrefab).GetComponent<ARSession>();
        }
    }

    /// <summary>
    /// リトライボタン押下時
    /// </summary>
    public void PushRetryButton()
    {
        // 現在のシーンを最読込
        assetsManager.PlayFirstBGM();

        // 1/3で広告を表示する
        if (Random.Range(1, 3) == 1)
        {
            SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_ADMOB).AsObservable()
            .Subscribe(_ =>
            {
                GoogleAdmobInterstitial googleAdmobInterstitial = FindObjectOfType<GoogleAdmobInterstitial>();
                googleAdmobInterstitial.param = playMode;
            });
        }
        else
        {
            // GameScene再読み込み
            SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_GAME).AsObservable()
            .Subscribe(_ =>
            {
                GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
                gameSceneManager.playMode = playMode;
            });
        }

        
    }

    /// <summary>
    /// もどるボタン押下時
    /// </summary>
    public void PushBackButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // TitleSceneに遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }
}
