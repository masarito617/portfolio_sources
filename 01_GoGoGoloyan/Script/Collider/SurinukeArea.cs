using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// すりぬけクラス
/// </summary>
/// <remarks>すりぬけオブジェクト(雲など)の挙動を管理するクラス</remarks>
public class SurinukeArea : MonoBehaviour
{
    /** 変数 */
    bool surinuke;
    BoxCollider2D colliderGround;

    void Start()
    {
        colliderGround = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // すり抜けフラグがONの場合、コライダを無効化
        if (surinuke)
            colliderGround.enabled = false;
        else
            colliderGround.enabled = true;
    }

    /// <summary>
    /// トリガー衝突時イベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // プレイヤーが衝突中の場合、すり抜けフラグをON
        if (collision.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER)
            surinuke = true;
    }

    /// <summary>
    /// トリガー衝突範囲から抜けた時のイベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // プレイヤーが衝突から抜けた場合、すり抜けフラグをOFF
        if (collision.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER)
            surinuke = false;
    }
}
