using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スカイボックス操作クラス
/// </summary>
/// <remarks>スカイボックスの挙動を管理するクラス</remarks>
public class SkyboxController : MonoBehaviour
{
    /** 設定値 */
    public Material sky;
    private float rotateSpeed = 0.035f; //回転スピード

    /** 変数 */
    float rotationRepeatValue;

    private void Awake()
    {
        // ゲーム内に一つだけ保持
        if (FindObjectsOfType<SkyboxController>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetNormalColor();
    }

    void Update()
    {
        // Materialを回転させる
        rotationRepeatValue = Mathf.Repeat(sky.GetFloat("_Rotation") + rotateSpeed, 360f);
        sky.SetFloat("_Rotation", rotationRepeatValue);
        // 回転させたMaterialをskyboxに設定
        RenderSettings.skybox = sky;
    }

    /// <summary>
    /// 通常色の設定
    /// </summary>
    public void SetNormalColor()
    {
        sky.SetColor("_Tint", new Color(128f/255f, 128f/255f, 128f/255f, 128f/255f));
    }

    /// <summary>
    /// レベルアップ色の設定
    /// </summary>
    public void SetLevelUpColor()
    {
        sky.SetColor("_Tint", new Color(100f / 255f, 0f / 255f, 255f / 255f, 128f / 255f));
    }
}