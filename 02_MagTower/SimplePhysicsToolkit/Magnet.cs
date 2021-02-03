using UnityEngine;
using System.Collections;

namespace SimplePhysicsToolkit {
	
	/// <summary>
	/// マグネクラス
	/// </summary>
	/// <remarks>
	/// 磁石の挙動を管理するクラス（SimplePhysicsToolkit Assets資産）
	/// ※「MAGTOWER」コメント部の処理を追加
	/// ※Assets資産のため、自身で追加した箇所以外は省略しています
	/// </remarks>
	public class Magnet : MonoBehaviour {

		// ---------- 省略 ----------

		// *** MAGTOWER ADD START ***
		public bool dispEffect = false;
		// *** MAGTOWER ADD END ***

		/// <summary>
		/// 磁石の状態更新処理
		/// </summary>
		void FixedUpdate () {
			// ---------- 省略 ----------

			// *** MAGTOWER ADD START ***
			// 落下状態のマグネ以外は処理対象外
			if (col.transform.tag != GameUtil.Const.TAG_MAGNET
				|| !(col.transform.GetComponent<MagnetController>().status == MagnetController.State.DROP))
				continue;
			// 誤差が180±10ならattractをTrueにする
			float stgMgRotation = Mathf.Abs(transform.localEulerAngles.y % 360.0f);
			float colMgRotation = Mathf.Abs(col.transform.localEulerAngles.y % 360.0f);
			float diffRotation = Mathf.Abs(stgMgRotation - colMgRotation);
			if (170.0f <= diffRotation && 190.0f >= diffRotation)
				attract = true;
			else
				attract = false;
			// *** MAGTOWER ADD END ***

			// ---------- 省略 ----------

			// *** MAGTOWER ADD START ***
			// エフェクト表示
			if (!dispEffect)
			{
				dispEffect = true;
				AssetsManager assetsManager = FindObjectOfType<AssetsManager>();
				// 結合か反発かで設定するエフェクトを変更
				GameObject effect = null;
				if (attract)
				{
					// 結合時のエフェクト設定
					GameObject plusEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_MAGNE_PLUS);
					effect = Instantiate(plusEffect, transform.position, Quaternion.identity);
					effect.transform.localPosition = transform.position + new Vector3(0, 1.5f, 0);
					// 効果音再生
					assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_MAGNE_PLUS);
				}
				else
				{
					// 反発時のエフェクト設定
					GameObject minusEffect = assetsManager.getEffectObject(GameUtil.Const.EFFECT_KEY_MAGNE_MINUS);
					effect = Instantiate(minusEffect, transform.position, Quaternion.identity);
					effect.transform.localPosition = transform.position + new Vector3(0, 2.5f, 0);
					// 効果音再生
					assetsManager.PlayOneShot(GameUtil.Const.SOUND_KEY_MAGNE_MINUS);
				}
				Destroy(effect, 1.0f);
			}
			// *** MAGTOWER ADD END ***

			// ---------- 省略 ----------
		}

		/// <summary>
		/// オブジェクトとの衝突処理
		/// </summary>
		/// <param name="col">衝突処理</param>
		void attractOrRepel(Collider col){
			// ---------- 省略 ----------

			// *** MAGTOWER ADD START ***
			// 反発時に落ちるよう力を加える
			float dropForce = Random.Range(-10f, 10f);
			col.transform.Rotate(new Vector3(0, dropForce, 5));
			col.transform.Translate(new Vector3(-0.05f, -0.05f, 0));
			// *** MAGTOWER ADD END ***

			// ---------- 省略 ----------
		}

		// ---------- 省略 ----------

	}
}
