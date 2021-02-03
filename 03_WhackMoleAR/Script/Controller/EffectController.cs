using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// もぐらエフェクト操作クラス
/// </summary>
/// <remarks>もぐらエフェクト（吹き出し、スコア）の挙動を管理するクラス</remarks>
public class EffectController : MonoBehaviour
{
    /** コンポーネント */
    MeshRenderer objRenderer;

    /** 変数 */
    Vector3 velocityZero = Vector3.zero;
    Vector3 movePosition;         // 移動先
    float maxSize = 1.0f;         // 最大サイズ
    float deltaSize = 0.15f;      // 拡大させる比率
    float positionYMove = 0.12f;  // Y軸移動先
    float positionXMove = 0.05f;  // X軸移動量（ランダム）
    float destroyBaseTime = 1.5f; // 破棄時間
    float destroyTime;            // 残り破棄時間

    void Start()
    {
        objRenderer = gameObject.GetComponentsInChildren<MeshRenderer>()[0];

        // 初期サイズ
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // 移動させる位置を設定
        // X軸：ランダム値 Y軸：固定
        movePosition = transform.position + new Vector3(Random.Range(-positionXMove, positionXMove), positionYMove, 0);

        // 破棄時間を設定
        destroyTime = destroyBaseTime;
    }

    void Update()
    {
        // 最大サイズになるまで拡大させる
        float scaleX = transform.localScale.x;
        if (scaleX < maxSize)
            transform.localScale += new Vector3(deltaSize, deltaSize, deltaSize);

        // Y軸を移動させる
        transform.position =
            Vector3.SmoothDamp(transform.position, movePosition, ref velocityZero, 0.3f);

        // カメラ方向を向かせる
        Vector3 p = Camera.main.transform.position;
        transform.LookAt(p);

        // 一定時間経過したらフェードアウトする
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
            FeedOutUpdate();
    }

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    void FeedOutUpdate()
    {
        // フェードアウト
        Color color = objRenderer.material.color;
        color.a -= 0.05f;
        objRenderer.material.color = color;

        // 見えなくなったらオブジェクト破棄
        if (color.a == 0)
            Destroy(gameObject);
    }
}
