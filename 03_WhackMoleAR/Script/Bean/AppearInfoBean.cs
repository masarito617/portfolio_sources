using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// もぐら出現情報クラス
/// </summary>
/// <remarks>もぐらの出現情報を管理するBeanクラス</remarks>
public class AppearInfoBean
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="kind">もぐらの種類（MOLE,MAJIRO,MOLEGOLD）</param>
    /// <param name="count">もぐらの出現数（1〜5）</param>
    /// <param name="escapeTime">もぐらが隠れるまでの時間（0.5秒〜5.0秒）</param>
    /// <param name="animSpeed">アニメーションスピード（1.0〜3.0）</param>
    public AppearInfoBean(string kind, int count, float escapeTime, float animSpeed)
    {
        this.Kind = kind;
        this.Count = count;
        this.EscapeTime = escapeTime;
        this.AnimSpeed = animSpeed;
    }

    // Getter（ReadOnly）
    public string Kind { get; }      // もぐらの種類（MOLE,MAJIRO,MOLEGOLD）
    public int Count { get; }        // もぐらの出現数（1〜5）
    public float EscapeTime { get; } // もぐらが隠れるまでの時間（0.5秒〜5.0秒）
    public float AnimSpeed { get; }  // アニメーションスピード（1.0〜3.0）
}
