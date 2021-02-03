using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ操作クラス
/// </summary>
/// <remarks>カメラの挙動を管理するクラス</remarks>
public class CameraController : MonoBehaviour
{
    /** 定数 */
    private const string AR_CAMERA_NAME = "AR Camera";

    /** コンポーネント */
    GameObject arCamera;

    void Start()
    {
        arCamera = GameObject.Find(AR_CAMERA_NAME);
    }

    void Update()
    {
        // ARカメラの位置と同じにする
        transform.position = arCamera.transform.root.position;
        transform.rotation = arCamera.transform.root.rotation;
    }
}
