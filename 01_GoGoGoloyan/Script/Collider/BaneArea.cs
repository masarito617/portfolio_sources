using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バネクラス
/// </summary>
/// <remarks>バネオブジェクトの挙動を管理するクラス</remarks>
public class BaneArea : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;

    /** 設定値 */
    public float power = 1800.0f;

    private void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
    }

    /// <summary>
    /// トリガー衝突時イベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーを上方向に飛ばす
        if (collision.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * power);
            // 効果音再生
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_BANE);
        }
    }
}
