using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using TMPro;

/// <summary>
/// ステージシーン管理クラス
/// </summary>
/// <remarks>ステージシーンの挙動を管理するクラス</remarks>
public class StageManager : MonoBehaviour
{
    /** 設定値 */
    public Sprite[] zangoroSpriteList;

    /** コンポーネント */
    AssetsManager assetsManager;
    
    /** UI部品 */
    Image gameOverImage;
    Image zangoroImage;
    TextMeshProUGUI timeText;
    TextMeshProUGUI elapsedTimeText;

    /** 変数 */
    public float zangoro;
    public float timeCount;
    public string clearSceneName;
    bool gameOver;
    string bestTimeSaveKey;

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        gameOverImage = GameObject.Find(GameUtil.Const.UI_IMAGE_GAMEOVER).GetComponent<Image>();
        zangoroImage = GameObject.Find(GameUtil.Const.UI_IMAGE_ZANGORO).GetComponent<Image>();
        timeText = GameObject.Find(GameUtil.Const.UI_TEXT_TIME).GetComponent<TextMeshProUGUI>();
        elapsedTimeText = GameObject.Find(GameUtil.Const.UI_TEXT_ELAPSED_TIME).GetComponent<TextMeshProUGUI>();

        // 重力設定
        Physics2D.gravity = new Vector2(0, -9.8f);

        // ゲームオーバー画像非表示
        gameOverImage.gameObject.SetActive(false);

        if (GameSystemManager.timeAttackMode)
        {
            // ＊＊＊ タイムアタックモード ＊＊＊
            // 残ゴロ画像非表示
            zangoroImage.gameObject.SetActive(false);
            timeText.gameObject.SetActive(true);
            elapsedTimeText.gameObject.SetActive(true);
            // タイマーを初期化
            timeCount = 0.0f;
        }
        else
        {
            // ＊＊＊ 通常モード ＊＊＊
            // 残ゴロ画像表示
            zangoroImage.gameObject.SetActive(true);
            timeText.gameObject.SetActive(false);
            elapsedTimeText.gameObject.SetActive(false);
            // 残ゴロ設定
            if (GameSystemManager.stageTmpZangro == 0)
                GameSystemManager.stageTmpZangro = 3;
            zangoro = GameSystemManager.stageTmpZangro;
            zangoroImage.sprite = zangoroSpriteList[(int)zangoro];
            // カニ討伐フラグをfalseに設定
            GameSystemManager.boolKaniAttack = false;
        }
    }

    private void Update()
    {
        if (GameSystemManager.timeAttackMode)
        {
            // ＊＊＊ タイムアタックモード ＊＊＊
            // 90秒を超えるとゲームオーバー
            if (timeCount >= 90.0)
            {
                if (!gameOver)
                {
                    GameOver();
                    timeCount = 90.00f;
                    elapsedTimeText.text = timeCount.ToString("n2");
                }
            }
            else
            {
                // 経過時間を表示
                timeCount += Time.deltaTime;
                elapsedTimeText.text = timeCount.ToString("n2");
                if (timeCount >= 80.0f)
                    elapsedTimeText.color = Color.red;
            }
            
        }
    }

    /// <summary>
    /// 残ゴロマイナス処理
    /// </summary>
    public void MinusZangoro()
    {
        zangoro -= 1;
        zangoroImage.sprite = zangoroSpriteList[(int) zangoro];
    }

    /// <summary>
    /// タイムアタックモードクリア処理
    /// </summary>
    public void TimeAttackClear()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case GameUtil.Const.SCENE_NAME_STAGE1:
                clearSceneName = "ステージ１";
                bestTimeSaveKey = GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE1;
                break;
            case GameUtil.Const.SCENE_NAME_STAGE2:
                clearSceneName = "ステージ２";
                bestTimeSaveKey = GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE2;
                break;
            case GameUtil.Const.SCENE_NAME_STAGE3:
                clearSceneName = "ステージ３";
                bestTimeSaveKey = GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE3;
                break;
            case GameUtil.Const.SCENE_NAME_STAGE4:
                clearSceneName = "ステージ４";
                bestTimeSaveKey = GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE4;
                break;
            case GameUtil.Const.SCENE_NAME_STAGE5:
                clearSceneName = "ステージ５";
                bestTimeSaveKey = GameUtil.Const.SAVE_KEY_BEST_CLEAR_TIME_STAGE5;
                break;
        }
        // ベストタイムの場合、保存する
        float clearTime = timeCount;
        float bestTime = GameSystemManager.GetFloat(bestTimeSaveKey);
        bool boolBest = false;
        if (bestTime == 0 || bestTime > clearTime)
        {
            GameSystemManager.SetFloat(bestTimeSaveKey, clearTime);
            boolBest = true;
        }
        // クリアシーンへ遷移
        SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_TIMEATTACK_CLEAR).AsObservable()
            .Subscribe(_ =>
            {
                TimeAttackClearManager clearManager = FindObjectOfType<TimeAttackClearManager>();
                clearManager.clearTime = clearTime;
                clearManager.clearStageName = clearSceneName;
                clearManager.bestTime = boolBest;
            });
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        gameOver = true;
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_GAMEOVER);
        // ゲームオーバー画像表示
        gameOverImage.gameObject.SetActive(true);
        // 残ゴロを初期値に戻す
        GameSystemManager.stageTmpZangro = 3;
        // タイトルシーンに遷移
        StartCoroutine(LoadTitleScene(3.0f));
    }

    /// <summary>
    /// タイトルシーン遷移
    /// </summary>
    /// <param name="waitTime">遷移までの待機時間</param>
    /// <returns></returns>
    private IEnumerator LoadTitleScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }

    /// <summary>
    /// 現在のシーン再読込処理
    /// </summary>
    /// <param name="waitTime">再読込までの待機時間</param>
    public void LoadCurrentScene(float waitTime)
    {
        // 残ゴロ退避
        GameSystemManager.stageTmpZangro = zangoro;
        StartCoroutine(LoadCurrentSceneCoroutine(waitTime));
    }

    /// <summary>
    /// 現在のシーン再読込コルーチン
    /// </summary>
    /// <param name="waitTime">再読込までの待機時間</param>
    private IEnumerator LoadCurrentSceneCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
