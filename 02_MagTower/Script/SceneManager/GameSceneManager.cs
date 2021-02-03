using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using TMPro;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
/// <remarks>ゲームシーンの挙動を管理するクラス</remarks>
public class GameSceneManager : MonoBehaviour
{
    /** 設定値 */
    public GameObject magnePrefab;
    private float[] magneRotList = { 0.0f, 90.0f, 180.0f, 270.0f }; // マグネ出現角度
    private float initGravity = -1.5f;        // 重力
    private float initWaitBaseTime = 0.5f;    // INIT待機時間
    private float magneWaitBaseTime = 3.0f;   // マグネ出現待機時間
    private float initWaitTime;               // INIT待機時間(格納用)
    private float magneWaitTime;              // マグネ出現待機時間(格納用)
    private string[] levelupScoreBaseList = {
        "5", "10", "20", "30", "50", "70", "100",
        "150", "200", "250", "300", "400", "500",
        "600", "700", "800", "900", "950" };  // レベルアップスコアリスト
    private List<string> levelupScoreList;    // レベルアップスコアリスト(格納用)

    /** コンポーネント */
    AssetsManager assetsManager;
    CameraController cameraController;
    SkyboxController skyboxController;
    GameObject uiCanvas;
    GameObject resultCanvas;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI resultScoreText;
    GameObject bestScoreImage;

    /** 定数 */
    private const string CANVAS_UI_NAME = "UICanvas";
    private const string CANVAS_RESULT_NAME = "ResultCanvas";
    private const string UI_TEXT_SCORE = "ScoreText";
    private const string UI_TEXT_SCORE_RESULT = "ResultScoreText";
    private const string UI_IMAGE_BEST_SCORE = "BestImage";

    /** 変数 */
    private GameObject targetStage;                 // 回転対象ステージ
    private Quaternion targetStageRotTo;            // 回転対象ステージ回転量
    private List<MagnetController> targetMagneList; // 回転対象マグネ
    private List<Quaternion> targetMagneRotToList;  // 回転対象マグネ回転量
    private float rotSpeed = 720.0f;                // 回転速度
    private bool doRot;                             // 回転フラグ
    private int score;                              // スコア
    private int magneCount;                         // 生成したマグネ数
    private GameObject levelUpEffect;               // レベルアップエフェクト

    // ステータス
    private GameState gameState;
    private GameState nextGameState;
    enum GameState
    {
        INIT, // 初期状態
        GAME, // ゲーム状態
        END   // ゲーム終了状態
    };

    void Awake()
    {
        // 動きもっさり改善
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        uiCanvas = GameObject.Find(CANVAS_UI_NAME);
        resultCanvas = GameObject.Find(CANVAS_RESULT_NAME);
        assetsManager = FindObjectOfType<AssetsManager>();
        cameraController = FindObjectOfType<CameraController>();
        skyboxController = FindObjectOfType<SkyboxController>();
        scoreText = GameObject.Find(UI_TEXT_SCORE).GetComponent<TextMeshProUGUI>();
        resultScoreText = GameObject.Find(UI_TEXT_SCORE_RESULT).GetComponent<TextMeshProUGUI>();
        bestScoreImage = GameObject.Find(UI_IMAGE_BEST_SCORE);

        // 重力を設定する
        Physics.gravity = new Vector3(0, initGravity, 0);
        // スコアを設定する
        score = 1;
        scoreText.text = score.ToString();
        magneCount = score;
        // レベルアップスコアリストを格納
        levelupScoreList = new List<string>();
        levelupScoreList.AddRange(levelupScoreBaseList);
        //結果画面を非表示
        uiCanvas.SetActive(true);
        resultCanvas.SetActive(false);
        // ゲーム初期状態
        gameState = GameState.INIT;
        nextGameState = GameState.INIT;
        InitStart();
    }

    void Update()
    {
        // ステータスに応じてメソッドを呼び出す
        switch (gameState)
        {
            case GameState.INIT:
                InitUpdate();
                break;
            case GameState.GAME:
                GameUpdate();
                break;
            case GameState.END:
                EndUpdate();
                break;
        }
        if (gameState != nextGameState)
        {
            gameState = nextGameState;
            switch (gameState)
            {
                case GameState.INIT:
                    InitStart();
                    break;
                case GameState.GAME:
                    GameStart();
                    break;
                case GameState.END:
                    EndStart();
                    break;
            }
        }
    }

    /// <summary>
    /// ステータス変更処理
    /// </summary>
    /// <param name="state">変更先ステータス</param>
    private void ChangeState(GameState state)
    {
        nextGameState = state;
    }

    /// <summary>
    /// ステータス初期化処理
    /// </summary>
    private void StateStartCommon()
    {
        initWaitTime = initWaitBaseTime;
        magneWaitTime = magneWaitBaseTime;
        doRot = false;
    }

    // ---------- 初期処理 ----------
    private void InitStart()
    {
        StateStartCommon();
        // 初期回転を設定
        SetTargetRotate(0.0f);
    }

    private void InitUpdate()
    {
        // 待機時間が過ぎたらゲーム状態へ遷移
        initWaitTime -= Time.deltaTime;
        if (initWaitTime < 0)
            ChangeState(GameState.GAME);
    }

    // ---------- ゲーム開始 ----------
    private void GameStart()
    {
        StateStartCommon();
    }

    private void GameUpdate()
    {
        // 回転処理の制御
        if (doRot)
            RotateTowardsStage();

        // 1000個生成したら処理終了
        if (magneCount >= 1000)
            return;

        // マグネの生成
        magneWaitTime -= Time.deltaTime;
        if(magneWaitTime < 0)
        {
            float magneRot = magneRotList[Random.Range(0, magneRotList.Length - 1)];
            Instantiate(magnePrefab,
                new Vector3(0, cameraController.GetLookTarget().transform.position.y + 20.0f, 0),
                Quaternion.Euler(0f, magneRot, 0));
            magneCount++;
            // 待機時間を初期化
            magneWaitTime = magneWaitBaseTime;
        }
    }

    // ---------- ゲーム終了 ----------
    private void EndStart()
    {
        StateStartCommon();
        //結果画面を表示
        uiCanvas.SetActive(false);
        resultCanvas.SetActive(true);
        bestScoreImage.SetActive(false);
        // スコアを設定
        resultScoreText.text = score.ToString();
        // ベストスコアの設定
        float bestScore = GameSystemManager.GetFloat(GameUtil.Const.SAVE_KEY_BEST_SCORE);
        if (bestScore == 0 || bestScore < score)
        {
            // ベストスコアの場合、保存する
            GameSystemManager.SetFloat(GameUtil.Const.SAVE_KEY_BEST_SCORE, score);
            bestScoreImage.SetActive(true);
        }
        // スコアの登録(ベストタイムに限らず送信)
        SendScore(score);
        // タワー全体をカメラ視点対象に設定
        cameraController.LookTowerTarget();
        // BGM変更
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_MAGGOAL, false);
    }

    private void EndUpdate()
    {
        // ＊＊＊ 処理なし ＊＊＊
    }

    /// <summary>
    /// 目的の角度までステージとマグネを回転させる処理
    /// </summary>
    private void RotateTowardsStage()
    {
        // ステージの回転が目的の角度になったら処理終了
        if (targetStage.transform.localEulerAngles.y.ToString("f2") == targetStageRotTo.eulerAngles.y.ToString("f2"))
        {
            doRot = false;
            return;
        }
        // ステージを回転させる
        targetStage.transform.rotation = Quaternion.RotateTowards(targetStage.transform.rotation, targetStageRotTo, rotSpeed * Time.deltaTime);
        // 磁石を回転させる
        for(int i = 0; i < targetMagneList.Count; i++)
        {
            MagnetController magne = targetMagneList[i];
            magne.transform.rotation = Quaternion.RotateTowards(magne.transform.rotation, targetMagneRotToList[i], rotSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 回転情報の設定
    /// </summary>
    /// <param name="rot">回転角度</param>
    private void SetTargetRotate(float rot)
    {
        // 回転フラグをTRUEにする
        doRot = true;
        // TMPオブジェクト
        List<MagnetController> tmpMagneList = new List<MagnetController>();
        List<Quaternion> tmpMagneRotToList = new List<Quaternion>();
        // オブジェクト取得
        GameObject stage = GameObject.Find(GameUtil.Const.TAG_STAGE);
        MagnetController[] magneList = FindObjectsOfType<MagnetController>();
        // 落下マグネ以外のマグネリストを取得
        foreach (MagnetController magne in magneList)
        {
            // 落下マグネは除く
            if (magne.status == MagnetController.State.DROP)
                continue;
            tmpMagneList.Add(magne);
            tmpMagneRotToList.Add(Quaternion.Euler(0.0f, (magne.transform.localEulerAngles.y + rot) % 360.0f, 0.0f));
        }
        // 回転情報を設定
        targetStage = stage;
        targetStageRotTo = Quaternion.Euler(0.0f, (stage.transform.localEulerAngles.y + rot) % 360.0f, 0.0f);
        targetMagneList = tmpMagneList;
        targetMagneRotToList = tmpMagneRotToList;
    }

    // ---------- ボタン押下イベント ----------
    /// <summary>
    /// 左ボタン押下時
    /// </summary>
    public void PushDownLButton()
    {
        // 回転処理
        if (!doRot)
            SetTargetRotate(90.0f);
    }

    /// <summary>
    /// 右ボタン押下時
    /// </summary>
    public void PushDownRButton()
    {
        // 回転処理
        if (!doRot)
            SetTargetRotate(-90.0f);
    }

    /// <summary>
    /// 戻るボタン押下時
    /// </summary>
    public void PushDownBackButton()
    {
        // タイトルシーンに戻る
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }

    /// <summary>
    /// リトライボタン押下時
    /// </summary>
    public void PushDownRetryButton()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_CLICK);
        // 結果画面非表示
        resultCanvas.SetActive(false);
        // 数秒後にリトライ
        Invoke("RetryDelay", 1.0f);
    }

    /// <summary>
    /// リトライ処理
    /// </summary>
    public void RetryDelay()
    {
        // BGM変更
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_MAGTECH, true);

        // スコア１０以上の場合 -> 50%
        // スコア１０未満の場合 -> 20%
        // で広告を表示する
        if (score >= 10 && Random.Range(1, 2) == 1)
            SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_ADMOB);
        else if (score < 10 && Random.Range(1, 5) == 1)
            SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_ADMOB);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 共有ボタン押下時
    /// </summary>
    public void PushShareButton()
    {
        // 共有画面を表示
        StartCoroutine(_Share());
    }

    // ---------- トリガーイベント ----------
    /// <summary>
    /// スコア加算処理
    /// </summary>
    public void AddScore()
    {
        // ゲーム終了時は対象外
        if (gameState == GameState.END)
            return;

        // スコアを加算
        score++;
        scoreText.text = score.ToString();

        // 1000に達したらゲームクリア
        if (score >= 1000)
        {
            // マスターフラグをTRUEにする
            GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_BOOL_MASTER, true);
            Invoke("GameOver", 3.0f);
            return;
        }

        // レベルアップ対象スコアに達した場合
        if (levelupScoreList.Contains(score.ToString()))
        {
            // スコア別にパラメータを設定
            float[] paramList = null;
            switch (score)
            {
                case 5:
                    paramList = new float[] { -2.0f, 3.0f };
                    break;
                case 10:
                    paramList = new float[] { -2.5f, 2.5f };
                    break;
                case 20:
                    paramList = new float[] { -3.0f, 2.5f };
                    break;
                case 30:
                    paramList = new float[] { -3.5f, 2.3f };
                    break;
                case 50:
                    paramList = new float[] { -4.0f, 2.3f };
                    break;
                case 70:
                    paramList = new float[] { -4.5f, 2.3f };
                    break;
                case 100:
                    paramList = new float[] { -5.0f, 2.0f };
                    break;
                case 150:
                    paramList = new float[] { -5.5f, 2.0f };
                    break;
                case 200:
                    paramList = new float[] { -6.0f, 2.0f };
                    break;
                case 250:
                    paramList = new float[] { -6.5f, 2.0f };
                    break;
                case 300:
                    paramList = new float[] { -7.0f, 1.8f };
                    break;
                case 400:
                    paramList = new float[] { -7.5f, 1.8f };
                    break;
                case 500:
                    paramList = new float[] { -8.0f, 1.8f };
                    break;
                case 600:
                    paramList = new float[] { -8.5f, 1.8f };
                    break;
                case 700:
                    paramList = new float[] { -8.7f, 1.8f };
                    break;
                case 800:
                    paramList = new float[] { -9.0f, 1.8f };
                    break;
                case 900:
                    paramList = new float[] { -9.5f, 1.8f };
                    break;
                case 950:
                    paramList = new float[] { -10.0f, 1.5f };
                    break;
            }
            // 重力とマグネ生成待機時間をレベルアップ
            Physics.gravity = new Vector3(0, paramList[0], 0);
            magneWaitBaseTime = paramList[1];
            // 効果音再生
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_LEVELUP);
            // レベルアップエフェクトを再生
            StartLevelUpEffect();
        }
    }

    /// <summary>
    /// レベルアップエフェクト開始
    /// </summary>
    void StartLevelUpEffect()
    {
        // エフェクト再生
        levelUpEffect = Instantiate(assetsManager.getEffectObject(
            GameUtil.Const.EFFECT_KEY_SHOOTING_STAR),
            new Vector3(-100, cameraController.transform.position.y, -70),
            Quaternion.identity);
        // Skyboxの色を変更
        skyboxController.SetLevelUpColor();
        // 数秒後に終了させる
        Invoke("StopLevelUpEffect", 1.5f);
    }

    /// <summary>
    /// レベルアップエフェクト終了
    /// </summary>
    void StopLevelUpEffect()
    {
        // エフェクト破棄
        Destroy(levelUpEffect);
        // Skyboxの色を戻す
        skyboxController.SetNormalColor();
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        ChangeState(GameState.END);
    }

    /// <summary>
    /// SNS共有処理コルーチン
    /// </summary>
    public IEnumerator _Share()
    {
        string imgPath = Application.persistentDataPath + "/image.png";

        // 前回のデータを削除
        File.Delete(imgPath);
        // 削除が完了するまで待機
        while (true)
        {
            if (!File.Exists(imgPath)) break;
            yield return null;
        }

        // スクリーンショットを取得
        ScreenCapture.CaptureScreenshot("image.png");
        // 撮影画像の書き込みが完了するまで待機
        while (true)
        {
            if (File.Exists(imgPath)) break;
            yield return null;
        }
        // 撮影画像の保存処理のため、１フレーム待機
        yield return new WaitForEndOfFrame();

        // 投稿する
        string tweetText;
        if (Application.systemLanguage == SystemLanguage.Japanese)
            tweetText = "マグネを" + score.ToString() + "個積みました！\n#マグネタワー\n【iOS/Android】\n"; // 日本語
        else
            tweetText = "MagTower Score: " + score.ToString() + "\n#マグネタワー\n【iOS/Android】\n";   // 日本語以外
        string tweetURL = "【ツイートURL】";
        SocialConnector.PostMessage(SocialConnector.ServiceType.Twitter, tweetText, tweetURL, imgPath);
    }

    /// <summary>
    /// ハイスコアの送信処理
    /// </summary>
    /// <param name="highScore">ハイスコア</param>
    public void SendScore(int highScore)
    {
#if UNITY_ANDROID
        // Android: LeaderBoardに登録
        Social.ReportScore(highScore, GameUtil.Const.LEADER_BOARD_ID_ANDROID, (bool success) => {
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
        int score = 0;
        if (int.TryParse(highScore.ToString(), out score))
        {
            Social.ReportScore(score, GameUtil.Const.LEADER_BOARD_ID_IOS, success => {
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
}
