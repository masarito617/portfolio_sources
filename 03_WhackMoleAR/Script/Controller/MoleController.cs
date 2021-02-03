using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// もぐら操作クラス
/// </summary>
/// <remarks>もぐらの挙動を管理するクラス</remarks>
public class MoleController : MonoBehaviour
{
    /** コンポーネント */
    Animator animator;
    AssetsManager assetsManager;

    /** 設定値 */
    public float hideBaseTime = 5.0f; // 隠れる時間
    public float animSpeed = 1.0f;    // アニメーション速度

    /** 変数 */
    public KIND kind;              // モグラ種類
    public bool delete = false;    // 削除フラグ
    private float hideTime;        // 隠れる残時間
    private Quaternion initRotate; // 生成時の向き

    // もぐらの種類
    public enum KIND
    {
        MOLE,     // モグラ
        MOLEGOLD, // モグラゴールド
        MAJIRO    // マジロ
    }

    void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        animator = GetComponent<Animator>();
        hideTime = hideBaseTime;
        animator.speed = animSpeed;

        // もぐらの種類設定
        if (gameObject.CompareTag(GameUtil.Const.TAG_NAME_MOLE))
            kind = KIND.MOLE;
        else if (gameObject.CompareTag(GameUtil.Const.TAG_NAME_MOLEGOLD))
            kind = KIND.MOLEGOLD;
        else if (gameObject.CompareTag(GameUtil.Const.TAG_NAME_MAJIRO))
            kind = KIND.MAJIRO;

        // カメラの方向にむかせる
        if (transform.rotation.eulerAngles.x >= 45)
        {
            // オブジェクトの角度が45度以上（垂直）：Z軸をオブジェクトの位置にしてLOOKさせる
            Vector3 lookTransform = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            transform.LookAt(lookTransform, transform.up);
        }
        else
        {
            // オブジェクトの角度が0度（水平）：Y軸をオブジェクトの位置にしてLOOKさせる
            Vector3 lookTransform = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
            transform.LookAt(lookTransform, transform.up);
        }

        // 生成時の向きを取得
        initRotate = transform.rotation;
    }

    void Update()
    {
        // 生成時の向きを維持
        transform.rotation = initRotate;
        // 時間が来たら隠れる
        hideTime -= Time.deltaTime;
        if (hideTime < 0)
            Hide();
    }

    /// <summary>
    /// 隠れる処理
    /// </summary>
    private void Hide()
    {
        // 逃げる
        animator.SetTrigger(GameUtil.Const.ANIME_KEY_ESCAPE);
        // オブジェクト破棄処理
        switch (kind)
        {
            case KIND.MOLE:
            case KIND.MOLEGOLD:
                Invoke("DelayDestroy", 2.0f);
                break;
            case KIND.MAJIRO:
                Invoke("DelayDestroy", 1.5f);
                break;
        }
    }

    /// <summary>
    /// ヒット処理
    /// </summary>
    public void Hit()
    {
        if (delete)
            return;

        // エフェクト・アニメーション再生
        delete = true;
        switch (kind)
        {
            case KIND.MOLE:
                // エフェクト再生１
                GameObject hitEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_HIT);
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                // エフェクト再生２
                GameObject hitCmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_HIT_COMMENT);
                Instantiate(hitCmtEffect, transform.position, Quaternion.identity);
                // エフェクト再生３
                GameObject score100CmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_SCORE100_COMMENT);
                Instantiate(score100CmtEffect, transform.position, Quaternion.identity);
                // 効果音再生
                assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_HIT);
                // 死亡
                animator.SetTrigger(GameUtil.Const.ANIME_KEY_DEAD);
                break;
            case KIND.MOLEGOLD:
                // エフェクト再生１
                GameObject hit2Effect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_HIT);
                Instantiate(hit2Effect, transform.position, Quaternion.identity);
                // エフェクト再生２
                GameObject hit2CmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_HIT_GOLD_COMMENT);
                Instantiate(hit2CmtEffect, transform.position, Quaternion.identity);
                // エフェクト再生３
                GameObject score500CmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_SCORE500_COMMENT);
                Instantiate(score500CmtEffect, transform.position, Quaternion.identity);
                // 効果音再生
                assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_HIT2);
                // 死亡
                animator.SetTrigger(GameUtil.Const.ANIME_KEY_DEAD);
                break;
            case KIND.MAJIRO:
                // エフェクト再生１
                GameObject guardEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_GUARD);
                Instantiate(guardEffect, transform.position, Quaternion.identity);
                // エフェクト再生２
                GameObject guardCmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_GUARD_COMMENT);
                Instantiate(guardCmtEffect, transform.position, Quaternion.identity);
                // エフェクト再生３
                GameObject score300CmtEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_SCORE300_COMMENT);
                Instantiate(score300CmtEffect, transform.position, Quaternion.identity);
                // 効果音再生
                assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_GUARD);
                // ガード
                animator.SetTrigger(GameUtil.Const.ANIME_KEY_DEAD);
                break;
        }

        // オブジェクト破棄
        Invoke("DelayDestroy", 1.0f);
    }

    /// <summary>
    /// オブジェクト破棄
    /// </summary>
    void DelayDestroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 隠れるアニメーション完了時
    /// </summary>
    public void EndHide()
    {
        // ヒットを無効にする
        delete = true;
    }

    // --------------- 衝突イベント ---------------
    /// <summary>
    /// コライダ衝突中のイベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnCollisionStay(Collision collision)
    {
        // もぐらと衝突している間は重ならないよう物理挙動をONにする
        if (collision.gameObject.GetComponent<MoleController>() != null)
            GetComponent<Rigidbody>().isKinematic = false;
    }

    /// <summary>
    /// コライダ衝突範囲から抜けた時のイベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnCollisionExit(Collision collision)
    {
        // もぐらが衝突範囲から抜けたら物理挙動をOFFにする
        if (collision.gameObject.GetComponent<MoleController>() != null)
            GetComponent<Rigidbody>().isKinematic = true;
    }
}