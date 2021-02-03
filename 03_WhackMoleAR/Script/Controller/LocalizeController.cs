using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ローカライズ操作クラス
/// </summary>
/// <remarks>画像、テキストのローカライズを行う共通クラス</remarks>
public class LocalizeController : MonoBehaviour
{
    /** 設定値 */
    public Sprite m_image_jp;
    public Sprite m_image_en;
    public string m_text_jp;
    public string m_text_en;

    void Start()
    {
        SetLanguageUI();
    }

    /// <summary>
    /// 言語に合わせたオブジェクトを設定
    /// </summary>
    void SetLanguageUI()
    {
        Image img = GetComponent<Image>();
        Text txt = GetComponent<Text>();

        SystemLanguage sl = Application.systemLanguage;
        switch (sl)
        {
            // ＊＊＊ 日本語 ＊＊＊
            case SystemLanguage.Japanese:
                if (img) img.sprite = m_image_jp;
                if (txt) txt.text = m_text_jp;
                break;
            // ＊＊＊ 英語（デフォルト） ＊＊＊
            case SystemLanguage.English:
            default:
                if (img) img.sprite = m_image_en;
                if (txt) txt.text = m_text_en;
                break;
        }
    }
}
