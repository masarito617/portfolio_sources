using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃エリアクラス
/// </summary>
/// <remarks>攻撃エリア内の衝突判定を行うクラス</remarks>
public class AttackArea : MonoBehaviour
{
    /** 設定値 */
    public GameObject hitEffect;
    public GameObject bombEffect;

    /** 変数 */
    float power = 1200.0f;

    /// <summary>
    /// トリガー衝突時イベント
    /// </summary>
    /// <param name="other">衝突対象</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            // ＊＊＊ カニかカラスの場合 ＊＊＊
            case GameUtil.Const.TAG_NAME_KANI:
            case GameUtil.Const.TAG_NAME_KARASU:
                // ヒットエフェクト発生
                GameObject enemyHitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                Destroy(enemyHitEffect, 0.3f);
                // 力を加える方向を決定
                Vector2 vecPower = transform.right * power;
                if (transform.root.transform.position.x - transform.position.x > 0)
                    vecPower *= -1;
                // 吹っ飛ばす処理
                other.SendMessage("Damage", vecPower);
                // カニ討伐フラグをtrueに設定
                GameSystemManager.boolKaniAttack = true;
                break;
            // ＊＊＊ バナキングの場合 ＊＊＊
            case GameUtil.Const.TAG_NAME_BANAKING:
                PlayerController playerController = transform.root.GetComponent<PlayerController>();
                BanakingController bananakingController = other.GetComponent<BanakingController>();
                if (playerController.dimax)
                {
                    // ゴロヤンが巨大化していたらダメージを与える
                    GameObject bomHitEffect = Instantiate(bombEffect, other.transform.position, Quaternion.identity) as GameObject;
                    Destroy(bomHitEffect, 0.3f);
                    other.SendMessage("Damage");
                }
                else
                {
                    // 防御状態でなければ揺らす
                    if (!bananakingController.guarding)
                    {
                        GameObject banaHitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                        Destroy(banaHitEffect, 0.3f);
                        other.SendMessage("Shake");
                    }
                }
                break;
        }
    }
}
