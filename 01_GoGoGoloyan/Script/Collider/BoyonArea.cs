using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボヨンクラス
/// </summary>
/// <remarks>ボヨンとしたオブジェクトの挙動を管理するクラス</remarks>
public class BoyonArea : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;
    GameObject goroyan;

    private void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();
        goroyan = GameObject.Find(GameUtil.Const.GOROYAN_NAME);
    }

    /// <summary>
    /// コライダ衝突時イベント
    /// </summary>
    /// <param name="other">衝突対象</param>
    void OnCollisionEnter2D(UnityEngine.Collision2D other)
    {
        // プレイヤーが離れているときは何もしない
        if (Vector3.Distance(goroyan.transform.position, transform.position) >= (10.0f))
            return;

        // プレイヤーかカニがぶつかってきたら跳ね返す
        if (other.gameObject.tag == GameUtil.Const.TAG_NAME_PLAYER
            || other.gameObject.tag == GameUtil.Const.TAG_NAME_KANI)
        {
            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            other.gameObject.transform.rotation = Quaternion.identity;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * -500.0f); ;
            // 効果音再生
            assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_BOYON);
        }
    }
}
