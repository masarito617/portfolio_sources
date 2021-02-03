using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミー（カニ）操作クラス
/// </summary>
/// <remarks>カニの挙動を操作するクラス</remarks>
public class KaniController : MonoBehaviour
{
    /** 設定値 */
    public float walkSpeed = 1.5f;

    /** コンポーネント */
    GameObject goroyan;

    /** 変数 */
    bool moveStart;
    bool damaged;

    void Start()
    {
        goroyan = GameObject.Find(GameUtil.Const.GOROYAN_NAME);
        moveStart = false;
        damaged = false;
    }

    void Update()
    {
        // ゴロヤンがいなければ何もしない
        if (goroyan == null)
            return;

        // ゴロヤンに近づいたら移動スタート
        if (Vector3.Distance(goroyan.transform.position, transform.position) <= (7.0f))
            moveStart = true;

        // 移動開始後の処理
        if (moveStart)
        {
            // 横移動させる
            if (!damaged)
                transform.Translate(-walkSpeed * Time.deltaTime, 0, 0);

            // ゴロヤンから離れたら破壊
            if (Vector3.Distance(goroyan.transform.position, transform.position) >= (10.0f))
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// 吹っ飛ばされる処理
    /// </summary>
    /// <param name="vecPower">吹っ飛ばされる強さ</param>
    public void Damage(Vector2 vecPower)
    {
        damaged = true;
        GetComponent<Rigidbody2D>().AddForce(vecPower);
    }
}
