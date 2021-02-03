using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 無重量クラス
/// </summary>
/// <remarks>無重力エリアの重力を管理するクラス</remarks>
public class NoGravityArea : MonoBehaviour
{
    /// <summary>
    /// トリガー衝突時イベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが入ってきたら無重力にする
        if (collision.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER)
            Physics2D.gravity = new Vector2(0, -0.5f);
    }

    /// <summary>
    /// トリガー衝突範囲から抜けた時のイベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // プレイヤーが範囲から出たら重力を元にもどす
        if (collision.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER)
            Physics2D.gravity = new Vector2(0, -9.8f);
    }
}
