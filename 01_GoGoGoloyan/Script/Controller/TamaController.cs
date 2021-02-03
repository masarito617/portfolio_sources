using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カラスから発射される球クラス
/// </summary>
/// <remarks>カラスから発射される球の挙動を管理するクラス</remarks>
public class TamaController : MonoBehaviour
{
    /** 設定値 */
    public GameObject tamaPrefab;   // 球オブジェクト
    public float moveXSpeed = 1.5f; // 横方向の移動速度
    public float moveYSpeed = 1.5f; // 縦方向の移動速度

    /** 変数 */
    GameObject tamaL;
    GameObject tamaR;
    float deleteTime = 1.0f;

    void Start()
    {
        // センターなら球を更に二つ生成
        if (moveXSpeed == 0.0f)
        {
            tamaL = Instantiate(tamaPrefab, transform.position, Quaternion.identity);
            tamaR = Instantiate(tamaPrefab, transform.position, Quaternion.identity);
            tamaL.GetComponent<TamaController>().moveXSpeed = -1.0f;
            tamaR.GetComponent<TamaController>().moveXSpeed = 1.0f;
        }
    }

    void Update()
    {
        // 球を散らばせる
        transform.Translate(moveXSpeed * Time.deltaTime, -moveYSpeed * Time.deltaTime, 0);
        // 時間が経ったら破壊
        deleteTime -= Time.deltaTime;
        if (deleteTime < 0)
            Destroy(gameObject);
    }
}
