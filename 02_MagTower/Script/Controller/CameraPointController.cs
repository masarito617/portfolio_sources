using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラポイント操作クラス
/// </summary>
/// <remarks>カメラの視点位置調整用のポイント操作クラス</remarks>
public class CameraPointController : MonoBehaviour
{
    public Camera usingCamera;
    private Vector3 pos = new Vector3(0, 0, 0);
    private Vector3 center = new Vector3(0, 0, 0);
    private float radius;         // タワー全体の半径
    private float margin = 10.0f; // 半径を少し余分にとるための値
    private float distance;       // タワー全体が写る距離

    void Start()
    {
        // カメラオブジェクトを取得
        CameraController cameraController = FindObjectOfType<CameraController>();
        usingCamera = cameraController.GetComponent<Camera>();
    }

    /// <summary>
    /// タワー全体が映るカメラ位置を返却する
    /// </summary>
    /// <returns>タワー全体が映るカメラ位置</returns>
    public Vector3 GetAllTowerLookPosition()
    {
        // マグネタワーのオブジェクトリストを取得
        List<Transform> magneTowerTransformList = new List<Transform>();
        MagnetController[] magneList = FindObjectsOfType<MagnetController>();
        foreach (MagnetController magne in magneList)
        {
            // 落下中のマグネは対象外
            if (magne.status == MagnetController.State.DROP)
                continue;
            // マグネの角度が90度または270度の場合、見やすいよう回転させる
            Vector3 magneAngle = magne.transform.localEulerAngles;
            if ((85 < magneAngle.y && magneAngle.y < 95) || (265 < magneAngle.y && magneAngle.y < 275))
                magne.transform.rotation = Quaternion.Euler(magneAngle.x, magneAngle.y - 90.0f, magneAngle.z);
            // リストに追加
            magneTowerTransformList.Add(magne.transform);
        }
        //オブジェクトのポジションの平均値を算出
        foreach (Transform trans in magneTowerTransformList)
        {     
            pos += trans.position;
        }
        center = pos / magneTowerTransformList.Count;
        // CenterPointのポジションをタワーの中心に配置
        transform.position = center;
        // 中心から最も遠いオブジェクトとの距離を算出
        foreach (Transform trans in magneTowerTransformList)
        {     
            radius = Mathf.Max(radius, Vector3.Distance(center, trans.position));
        }
        // カメラをCenterPointの高さの位置に移動する
        usingCamera.transform.position = new Vector3(usingCamera.transform.position.x, transform.position.y, 0);
        // タワー全体が写る距離を算出し、位置情報として返却する
        // 式： 半径 / tan(カメラの描画角度 / 2)
        distance = (radius + margin) / Mathf.Tan(usingCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return new Vector3(distance, transform.position.y, 0);
    }
}
