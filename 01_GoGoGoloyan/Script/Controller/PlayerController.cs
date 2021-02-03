using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fungus;
using UniRx;

/// <summary>
/// プレイヤー操作クラス
/// </summary>
/// <remarks>プレイヤー(ゴロヤン)の操作や挙動を管理するクラス</remarks>
public class PlayerController : MonoBehaviour
{
    /** 設定値 */
    public GameObject deadEffect;
    public float jumpForce = 680.0f;
    public float walkForce = 15.0f;

    /** コンポーネント */
    AssetsManager assetsManager;
    StageManager stageManager;
    CameraController cameraController;
    Rigidbody2D rigid2D;
    Animator animator;
    AttackArea attackArea;

    /** 変数 */
    Vector3 initPosition;
    float scale = 0.2f;
    float maxWalkSpeed = 2.0f;
    bool moveLeft;
    bool moveRight;
    bool died;
    bool dimax;

    void Awake()
    {
        // 動きもっさり改善
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        stageManager = FindObjectOfType<StageManager>();
        cameraController = FindObjectOfType<CameraController>();
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackArea = FindObjectOfType<AttackArea>();
        moveLeft = false;
        moveRight = false;

        // 攻撃コライダを無効化
        attackArea.GetComponent<BoxCollider2D>().enabled = false;
        // 開始位置を保持
        initPosition = transform.position;
        // 死亡フラグを初期値に戻す
        died = false;
    }

    void Update()
    {
        // ジャンプ処理
        if(Input.GetKeyDown(KeyCode.Space))
        {
            JumpButtonDown();
        }

        // 移動処理
        float speedx = Mathf.Abs(rigid2D.velocity.x);
        if (Input.GetKey(KeyCode.RightArrow) || moveRight)
        {
            // 右キー押下時
            if (transform.localScale.x < 0)
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 1);
            rigid2D.AddForce(transform.right * walkForce);
            if (speedx < maxWalkSpeed)
                rigid2D.AddForce(transform.right * walkForce);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || moveLeft)
        {
            // 左キー押下時
            if (transform.localScale.x > 0)
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 1);
            rigid2D.AddForce(transform.right * walkForce * -1);
            if (speedx < maxWalkSpeed)
                rigid2D.AddForce(transform.right * walkForce * -1);
        }

        // アニメーションの速度変更
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(GameUtil.Const.ANIMETION_NAME_ATTACK))
        {
            // 攻撃中の場合スピード固定
            animator.speed = 3.5f;
            // 攻撃コライダを有効化
            attackArea.GetComponent<BoxCollider2D>().enabled = true;
            attackArea.GetComponent<BoxCollider2D>().transform.position = transform.position;
            attackArea.GetComponent<BoxCollider2D>().transform.Translate(scale * 5, 0, 0);
        }
        else
        {
            // 攻撃中でない場合、移動速度に合わせる
            animator.speed = speedx / 2.0f;
            // 攻撃コライダを無効化
            attackArea.GetComponent<BoxCollider2D>().enabled = false;
        }

        // 落下した場合の処理
        if (transform.position.y < -10)
            PlayerDead(0.5f);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    /// <param name="waitTime">死亡後の待機時間</param>
    private void PlayerDead(float waitTime)
    {
        // 既に死亡してたら処理終了
        if (died)
            return;

        // 死亡フラグをONにする
        died = true;

        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_DELETE);

        // 死亡エフェクト
        GameObject effect = Instantiate(deadEffect, transform.position, Quaternion.identity) as GameObject;
        effect.transform.localPosition = transform.position + new Vector3(0.0f, 3.0f * scale, 0.0f);
        Destroy(effect, 0.3f);

        if (GameSystemManager.timeAttackMode)
        {
            // ＊＊＊ タイムアタックモード ＊＊＊
            // 初期位置に戻す
            Invoke("SetInitPosition", 2.0f);
            gameObject.SetActive(false);
        }
        else
        {
            // ＊＊＊ 通常モード ＊＊＊
            // 残ゴロマイナス処理
            stageManager.MinusZangoro();
            if (stageManager.zangoro == 0)
            {
                // ゲームオーバー
                stageManager.GameOver();
                Destroy(gameObject);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == GameUtil.Const.SCENE_NAME_STAGE5)
                {
                    // ボス戦の場合、初期位置に戻す
                    Invoke("SetInitPosition", 2.0f);
                    gameObject.SetActive(false);
                }
                else
                {
                    // 指定秒後にシーン再読み込み
                    stageManager.LoadCurrentScene(waitTime);
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 初期位置に戻す処理
    /// </summary>
    private void SetInitPosition()
    {
        // 初期設定
        gameObject.SetActive(true);
        transform.rotation = Quaternion.identity;
        transform.position = initPosition;
        // カメラ位置も戻す
        cameraController.SetInitPosition();
        // 死亡フラグを初期値に戻す
        died = false;
    }

    /// <summary>
    /// ステージクリア処理
    /// </summary>
    public void StageClear()
    {
        if (GameSystemManager.timeAttackMode)
        {
            // ＊＊＊ タイムアタックモード ＊＊＊
            stageManager.TimeAttackClear();
        }
        else
        {
            // ＊＊＊ 通常モード ＊＊＊
            // 残ゴロを初期値に戻す
            GameSystemManager.stageTmpZangro = 3;

            // ステージによって遷移イベントと設定するフラグを変更
            string loadEventKey = null;
            switch (SceneManager.GetActiveScene().name)
            {
                case GameUtil.Const.SCENE_NAME_STAGE1:
                    // ステージ１クリア時
                    GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE1, true);
                    loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_END;
                    break;
                case GameUtil.Const.SCENE_NAME_STAGE2:
                    // ステージ２クリア時
                    GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE2, true);
                    if (GameSystemManager.boolKaniAttack)
                        loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END;
                    else
                        loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END_2;
                    break;
                case GameUtil.Const.SCENE_NAME_STAGE3:
                    // ステージ３クリア時
                    GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE3, true);
                    loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_END;
                    break;
                case GameUtil.Const.SCENE_NAME_STAGE4:
                    // ステージ４クリア時
                    GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE4, true);
                    loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_END;
                    break;
                case GameUtil.Const.SCENE_NAME_STAGE5:
                    // ステージ５クリア時
                    GameSystemManager.SetBool(GameUtil.Const.SAVE_KEY_CLEAR_STAGE5, true);
                    loadEventKey = GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_END;
                    break;
            }

            // イベントシーンに遷移
            SceneManager.LoadSceneAsync(GameUtil.Const.SCENE_NAME_EVENT).AsObservable()
                                .Subscribe(_ =>
                                {
                                    EventManager eventManager = FindObjectOfType<EventManager>() as EventManager;
                                    eventManager.loadEventParam = loadEventKey;
                                });
        }
    }

    /// <summary>
    /// 巨大化処理
    /// </summary>
    public void Dimax()
    {
        // 変更後のサイズ設定
        scale = 1.0f;
        dimax = true;
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_DIMAX);
        // サイズ変更
        Invoke("ScaleChange", 0.2f);
    }

    /// <summary>
    /// 巨大化解放処理
    /// </summary>
    public void DimaxRelease()
    {
        // 変更後のサイズ設定
        scale = 0.2f;
        dimax = false;
        // サイズ変更
        Invoke("ScaleChange", 1.5f);
    }

    /// <summary>
    /// サイズ変更処理
    /// </summary>
    private void ScaleChange()
    {
        // 効果音再生
        if (!dimax)
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_DIMAX_RELEASE);
        // サイズ変更
        transform.localScale = new Vector3(scale, scale, 1);
    }

    // ---------- トリガーイベント ----------
    /// <summary>
    /// コライダ衝突時イベント
    /// </summary>
    /// <param name="other">衝突対象</param>
    void OnCollisionEnter2D(UnityEngine.Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            // 敵に衝突した場合
            case GameUtil.Const.TAG_NAME_KANI:
            case GameUtil.Const.TAG_NAME_UNI:
            case GameUtil.Const.TAG_NAME_KARASU:
            case GameUtil.Const.TAG_NAME_TAMA:
            case GameUtil.Const.TAG_NAME_HARI:
                if (dimax)
                    Destroy(other.gameObject);
                else
                    PlayerDead(1.0f);
                break;
            // デリートゾーンの場合
            case GameUtil.Const.TAG_NAME_DELETE_ZONE:
                PlayerDead(1.0f);
                break;
            // 巨バナを手に入れた場合
            case GameUtil.Const.TAG_NAME_KYOBANA:
                Destroy(other.gameObject);
                Dimax();
                break;
        }
    }

    /// <summary>
    /// トリガー衝突時イベント
    /// </summary>
    /// <param name="other">衝突対象</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            // ゴールに到着した場合
            case GameUtil.Const.TAG_NAME_GOAL:
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                StageClear();
                break;
        }
    }

    // ---------- ボタン押下処理 ----------
    /// <summary>
    /// ジャンプボタン押下時
    /// </summary>
    public void JumpButtonDown()
    {
        animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_JUMP_TRIGGER);
        rigid2D.AddForce(transform.up * jumpForce);
        // 効果音再生
        if (Physics.gravity.y <= -5.0f)
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_JUMP);
        else
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_JUMP_LONG);
    }

    /// <summary>
    /// 攻撃ボタン押下時
    /// </summary>
    public void AttackButtonDown()
    {
        animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_ATTACK_TRIGGER);
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_ATTACK);
    }

    /// <summary>
    /// 戻るボタン押下時
    /// </summary>
    public void BackButtonDown()
    {
        // 残ゴロを初期値に戻す
        GameSystemManager.stageTmpZangro = 3;
        // タイトルシーンに遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }

    /// <summary>
    /// リロードボタン押下時
    /// </summary>
    public void ReloadButtonDown()
    {
        // 起き上がる
        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 左移動ボタン離した時
    /// </summary>
    public void LButtonPushUp()
    {
        moveLeft = false;
    }

    /// <summary>
    /// 左移動ボタン押下時
    /// </summary>
    public void LButtonPushDown()
    {
        moveLeft = true;
    }

    /// <summary>
    /// 右移動ボタン離した時
    /// </summary>
    public void RButtonPushUp()
    {
        moveRight = false;
    }

    /// <summary>
    /// 右移動ボタン押下時
    /// </summary>
    public void RButtonPushDown()
    {
        moveRight = true;
    }
}
