using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームシステム共通クラス
/// </summary>
/// <remarks>ゲームシステム共通の処理を管理するクラス</remarks>
public class GameSystemManager : MonoBehaviour
{
    /** バナー表示フラグ(GPGS認証後) */
    public static bool dispBanner;

    // 以下、セーブ値格納用

    /// <summary>
    /// 保存されているBool値を返却する
    /// </summary>
    /// <param name="key">保存キー</param>
    /// <returns>保存キーに紐づく値</returns>
    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1;
    }

    /// <summary>
    /// Bool値を保存する
    /// </summary>
    /// <param name="key">保存キー</param>
    /// <param name="value">保存する値</param>
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    /// <summary>
    /// 保存されているFloat値を返却する
    /// </summary>
    /// <param name="key">保存キー</param>
    /// <returns>保存キーに紐づく値</returns>
    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    /// <summary>
    /// Float値を保存する
    /// </summary>
    /// <param name="key">保存キー</param>
    /// <param name="value">保存キーに紐づく値</param>
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}
