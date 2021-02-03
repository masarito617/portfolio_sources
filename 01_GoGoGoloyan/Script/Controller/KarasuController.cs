using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミー（カラス）操作クラス
/// </summary>
/// <remarks>カラスの挙動を管理するクラス</remarks>
public class KarasuController : MonoBehaviour
{
    /** 設定値 */
    public GameObject tama;         // 球オブジェクト
    public float tamaSpan = 3.0f;   // 球を放つ間隔
    public float yureSpeedY = 1.0f; // 縦方向の移動速度
    public float yureSpanY = 0.5f;  // 縦方向の揺れ間隔
    public float yureSpeedX = 0.5f; // 横方向の移動速度
    public float yureSpanX = 1.5f;  // 横方向の揺れ間隔

    /** 変数 */
    Vector3 initPosition;
    float tamaReleaseTime;
    bool damaged;

    void Start()
    {
        initPosition = transform.position;
        damaged = false;
        tamaReleaseTime = tamaSpan;
    }

    void Update()
    {
        // 吹っ飛ばされてる時は処理しない
        if (damaged)
            return;

        // 揺れの方向転換
        if ((yureSpeedY > 0 && transform.position.y - initPosition.y > yureSpanY) ||
            (yureSpeedY < 0 && transform.position.y - initPosition.y < -yureSpanY))
            yureSpeedY *= -1;
        if ((yureSpeedX > 0 && transform.position.x - initPosition.x > yureSpanX) ||
            (yureSpeedX < 0 && transform.position.x - initPosition.x < -yureSpanX))
            yureSpeedX *= -1;
        // 上下に揺れさせる
        transform.Translate(yureSpeedX * Time.deltaTime, yureSpeedY * Time.deltaTime, 0);

        // 一定間隔ごとに球を生成
        tamaReleaseTime -= Time.deltaTime;
        if (tamaReleaseTime < 0)
        {
            Instantiate(tama, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
            tamaReleaseTime = tamaSpan;
        }
    }

    /// <summary>
    /// 吹っ飛ばされる処理
    /// </summary>
    /// <param name="vecPower">吹っ飛ばされる力</param>
    public void Damage(Vector2 vecPower)
    {
        damaged = true;
        GetComponent<Rigidbody2D>().AddForce(vecPower);
    }
}
