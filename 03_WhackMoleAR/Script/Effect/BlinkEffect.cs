using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 点滅エフェクトクラス
/// </summary>
/// <remarks>点滅エフェクト効果を付与するクラス</remarks>
public class BlinkEffect : MonoBehaviour
{
    /** 設定値 */
    public float speed = 1.0f;

    /** コンポーネント */
    private Text text;
    private Image image;

    /** 変数 */
    private float time;

    private enum ObjType
    {
        TEXT,
        IMAGE
    };
    private ObjType thisObjType = ObjType.TEXT;

    void Start()
    {
        // アタッチしてるオブジェクトを判別
        if (this.gameObject.GetComponent<Image>())
        {
            thisObjType = ObjType.IMAGE;
            image = this.gameObject.GetComponent<Image>();
        }
        else if (this.gameObject.GetComponent<Text>())
        {
            thisObjType = ObjType.TEXT;
            text = this.gameObject.GetComponent<Text>();
        }
    }

    void Update()
    {
        // オブジェクトのAlpha値を更新
        if (thisObjType == ObjType.IMAGE)
        {
            image.color = GetAlphaColor(image.color);
        }
        else if (thisObjType == ObjType.TEXT)
        {
            text.color = GetAlphaColor(text.color);
        }
    }

    /// <summary>
    /// Alpha値を更新してColorを返す
    /// </summary>
    /// <param name="color">現在のColor</param>
    /// <returns>Alpha値更新後のColor</returns>
    Color GetAlphaColor(Color color)
    {
        // sin値よりAlphaを一定周期で変化させる
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;
        return color;
    }
}
