using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスAI（バナキング）操作クラス
/// </summary>
/// <remarks>バナキングの挙動を管理するクラス</remarks>
public class BanakingController : MonoBehaviour
{
    /** 設定値 */
    public GameObject bombEffect;           // 爆発エフェクト
    public GameObject kyobanaPrefab;        // 巨バナオブジェクト
    public GameObject kaniPrefab;           // カニオブジェクト
    public GameObject karasuPrefab;         // カラスオブジェクト
    public GameObject uniPrefab;            // ウニオブジェクト
    public Transform kaniTransform1;        // カニの召喚位置１
    public Transform kaniTransform2;        // カニの召喚位置２
    public Transform kaniTransform3;        // カニの召喚位置３
    public Transform uniTransform1;         // ウニの召喚位置１
    public Transform uniTransform2;         // ウニの召喚位置２
    public Transform uniTransform3;         // ウニの召喚位置３
    public Transform karasuTransform1;      // カラスの召喚位置１
    public Transform karasuTransform2;      // カラスの召喚位置２
    public Transform karasuTransform3;      // カラスの召喚位置３
    public int HP = 30;                     // HP
    public float atackWaitBaseTime = 8.0f;  // 攻撃状態までの待機時間
    public float guardWaitBaseTime = 10.0f; // ガード状態までの待機時間

    /** コンポーネント */
    Animator animator;
    AssetsManager assetsManager;

    /** 変数 */
    float attackWaitTime;
    float guardWaitTime;
    public bool guarding;
    int attackCount;

    // ステータス
    enum State
    {
        Staying,    // 待機状態
        Attacking,  // 攻撃状態
        Died,       // 死亡状態
    };
    State state = State.Staying;     // 現在のステータス
    State nextState = State.Staying; // 次のステータス

    void Start()
    {
        animator = GetComponent<Animator>();
        assetsManager = FindObjectOfType<AssetsManager>();
        // 待機状態変数初期化
        StayStart();
    }

    void Update()
    {
        // ステータスに応じてメソッドを呼び出す
        switch (state)
        {
            case State.Staying:
                Staying();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
        if (state != nextState)
        {
            state = nextState;
            switch (state)
            {
                case State.Staying:
                    StayStart();
                    break;
                case State.Attacking:
                    AttackStart();
                    break;
                case State.Died:
                    Died();
                    break;
            }
        }
    }

    /// <summary>
    /// ステータス変更処理
    /// </summary>
    /// <param name="nextState">変更先ステータス</param>
    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    /// <summary>
    /// ステータス初期化処理
    /// </summary>
    void StateStartCommon()
    {
        attackWaitTime = atackWaitBaseTime;
        guardWaitTime = guardWaitBaseTime;
        guarding = false;
    }

    // ---------- 待機 ----------
    private void StayStart()
    {
        StateStartCommon();
        guarding = true; // 防御状態をON
    }

    private void Staying()
    {
        if (attackWaitTime > 0)
        {
            attackWaitTime -= Time.deltaTime;
        }
        else
        {
            // 攻撃状態までの待機時間が過ぎたら攻撃状態に遷移
            ChangeState(State.Attacking);
        }
    }

    // ---------- 攻撃 ----------
    private void AttackStart()
    {
        StateStartCommon();
        animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_BANAKING_ATTACK_TRIGGER);
        // 敵の再生成
        ClearEnemy();
        Invoke("Summon", 1.0f);
    }

    private void Attacking()
    {
        if (guardWaitTime > 0)
        {
            guardWaitTime -= Time.deltaTime;
        }
        else
        {
            // ガード状態までの待機時間が過ぎたら待機状態に遷移
            animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_BANAKING_GUARD_TRIGGER);
            ChangeState(State.Staying);
        }
    }

    /// <summary>
    /// エネミークリア処理
    /// </summary>
    private void ClearEnemy()
    {
        GameObject kani = GameObject.Find(kaniPrefab.name);
        if (kani != null)
            Destroy(kani);
        GameObject karasu = GameObject.Find(karasuPrefab.name);
        if (karasu != null)
            Destroy(karasu);
        GameObject uni = GameObject.Find(uniPrefab.name);
        if (uni != null)
            Destroy(uni);
        GameObject kyobana = GameObject.Find(GameUtil.Const.TAG_NAME_KYOBANA);
        if (kyobana != null)
            Destroy(kyobana);
    }

    /// <summary>
    /// エネミー召喚処理
    /// </summary>
    private void Summon()
    {
        // 攻撃４パターンを繰り返す
        switch (attackCount % 4)
        {
            case 0:
                SummonEnemy(kaniPrefab, kaniTransform1, kaniTransform2, kaniTransform3);
                break;
            case 1:
                SummonEnemy(kaniPrefab, kaniTransform1, kaniTransform2, kaniTransform3);
                SummonEnemy(uniPrefab, uniTransform1, uniTransform2, uniTransform3);
                break;
            case 2:
                SummonEnemy(kaniPrefab, kaniTransform1, kaniTransform2, kaniTransform3);
                SummonEnemy(karasuPrefab, karasuTransform1, karasuTransform2, karasuTransform3);
                break;
            case 3:
                SummonEnemy(kaniPrefab, kaniTransform1, kaniTransform2, kaniTransform3);
                SummonEnemy(karasuPrefab, karasuTransform1, karasuTransform2, karasuTransform3);
                SummonEnemy(uniPrefab, uniTransform1, uniTransform2, uniTransform3);
                break;
        }
        attackCount++;
    }

    /// <summary>
    /// 指定されたエネミーを召喚する
    /// </summary>
    /// <param name="enemyPrefab">エネミーオブジェクト</param>
    /// <param name="enemyTransform1">エネミー召喚位置１</param>
    /// <param name="enemyTransform2">エネミー召喚位置２</param>
    /// <param name="enemyTransform3">エネミー召喚位置３</param>
    private void SummonEnemy(GameObject enemyPrefab,
        Transform enemyTransform1,
        Transform enemyTransform2,
        Transform enemyTransform3)
    {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.name = enemyPrefab.name;
        // サイズ調整
        switch (enemy.name)
        {
            case GameUtil.Const.TAG_NAME_KANI:
                enemy.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                break;
            case GameUtil.Const.TAG_NAME_KARASU:
                enemy.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                break;
        }
        // 召喚位置を３種類からランダムに決める
        int random = Random.Range(1, 3);
        switch (random)
        {
            case 1:
                enemy.transform.position = enemyTransform1.position;
                break;
            case 2:
                enemy.transform.position = enemyTransform2.position;
                break;
            case 3:
                enemy.transform.position = enemyTransform3.position;
                break;
        }
    }

    // ---------- 死亡 ----------
    /// <summary>
    /// 死亡処理
    /// </summary>
    private void Died()
    {
        // アニメーションを停止
        animator.speed = 0;
        // めっちゃ爆発エフェクト
        Instantiate(bombEffect, transform.position, Quaternion.identity);
        // 爆発音再生
        assetsManager.PlayBGM(GameUtil.Const.SOUND_KEY_BOMB);
        // 数秒後にステージクリア
        Invoke("GameClear", 4.5f);
    }

    /// <summary>
    /// ステージクリア処理
    /// </summary>
    private void GameClear()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.StageClear();
    }

    // ---------- トリガーイベント ----------
    /// <summary>
    /// コライダ衝突時のイベント
    /// </summary>
    /// <param name="other">衝突対象</param>
    void OnCollisionEnter2D(UnityEngine.Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            // 敵が衝突してきたら消す
            case GameUtil.Const.TAG_NAME_KANI:
            case GameUtil.Const.TAG_NAME_UNI:
            case GameUtil.Const.TAG_NAME_KARASU:
                Destroy(other.gameObject);
                break;
        }
    }

    /// <summary>
    /// 揺らされた時のイベント
    /// </summary>
    public void Shake()
    {
        animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_BANAKING_SHAKE_TRIGGER);
        // 7分の1の確率で巨バナを落とす
        if (Random.Range(1, 7) == 1)
        {
            if (GameObject.Find(GameUtil.Const.TAG_NAME_KYOBANA) == null)
            {
                GameObject shakeItem = Instantiate(kyobanaPrefab);
                shakeItem.name = kyobanaPrefab.name;
                shakeItem.transform.position = transform.position + new Vector3(-0.7f, 4.5f, 0);
                shakeItem.GetComponent<Rigidbody2D>().AddForce(transform.up * 200.0f);
                shakeItem.GetComponent<Rigidbody2D>().AddForce(transform.right * -70.0f);
            }
        }
    }

    /// <summary>
    /// ダメージを受けた時のイベント
    /// </summary>
    public void Damage()
    {
        // 効果音再生
        assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_BOMB);
        // ダメージを受ける
        HP -= 10;
        if (HP <= 0)
        {
            // HPが0になったら死亡
            ChangeState(State.Died);
            // UIを非表示にする
            GameObject UI = GameObject.Find(GameUtil.Const.CONTROL_UI_NAME);
            UI.SetActive(false);
        }
        else
        {
            // ダメージを受けてゴロヤンの巨大化を解除する
            animator.SetTrigger(GameUtil.Const.ANIMATION_KEY_BANAKING_DAMAGE_TRIGGER);
            PlayerController playerController = FindObjectOfType<PlayerController>();
            playerController.DimaxRelease();
            // 待機状態に遷移
            ChangeState(State.Staying);
        }
    }
}
