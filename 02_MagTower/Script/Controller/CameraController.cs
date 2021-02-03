using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ操作クラス
/// </summary>
/// <remarks>カメラの挙動を管理するクラス</remarks>
public class CameraController : MonoBehaviour
{
    /** 設定値 */
    private float lookHeight = 2.5f;  // カメラ移動を開始する高さ
    private float lookSpeed = 0.25f;  // カメラ移動速度
    private Vector3 velocityZero = Vector3.zero;

    /** コンポーネント */
    private CameraPointController cameraPointController;

    /** 変数 */
    private GameObject lookTarget;   // カメラ視点対象
    private Vector3 lookPosition;    // カメラ視点の位置
    private bool changeLook = false; // カメラ視点変更フラグ

    void Start()
    {
        cameraPointController = FindObjectOfType<CameraPointController>();
    }

    void Update()
    {
        // カメラ視点位置に到着した場合
        if (transform.position == lookPosition)
            changeLook = false;
        // カメラ視点位置へ一定速度で移動する
        if (changeLook)
            transform.position = Vector3.SmoothDamp(transform.position, lookPosition, ref velocityZero, lookSpeed);
    }

    /// <summary>
    /// カメラ視点対象設定処理
    /// </summary>
    /// <param name="target">視点対象</param>
    public void SetLookTarget(GameObject target)
    {
        // ターゲットが未設定または移動対象外の高さの場合
        if (lookTarget == null || target.transform.position.y <= lookHeight)
        {
            lookTarget = target;
            return;
        }
        // カメラ視点対象を再設定する
        float diffY = target.transform.position.y - lookTarget.transform.position.y;
        lookPosition = transform.position + new Vector3(0, diffY, 0);
        changeLook = true;
        lookTarget = target;
    }

    /// <summary>
    /// タワー全体の視点対象設定処理
    /// </summary>
    public void LookTowerTarget()
    {
        // CameraPointオブジェクトよりカメラ視点位置を設定
        lookPosition = cameraPointController.GetAllTowerLookPosition();
        transform.LookAt(cameraPointController.transform);
        changeLook = true;
    }

    /// <summary>
    /// カメラ視点対象取得処理
    /// </summary>
    /// <returns>視点対象</returns>
    public GameObject GetLookTarget()
    {
        return lookTarget;
    }
}
