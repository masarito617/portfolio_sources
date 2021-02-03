using System;

/// <summary>
/// 文字列変換クラス
/// </summary>
/// <remarks>SpriteAssets用に文字列を変換するクラス</remarks>
public static class ConvUtil
{
    /// <summary>
    /// SpriteAssets用に文字列を変換
    /// </summary>
    /// <param name="str">変換文字列</param>
    /// <returns>変換後文字列</returns>
    public static string ConvFoolCoolFont(string str)
    {
        string rtnStr = "";
        // NULLの場合対象外
        if (str == null)
            return rtnStr;
        // 文字列を一文字ずつ変換
        for (int i = 0; i < str.Length; i++)
        {
            // 文字列変換
            string convStr;
            switch (str[i])
            {
                // 文字に対応する番号を返却
                case '-':
                    convStr = "10";
                    break;
                case 'G':
                    convStr = "11";
                    break;
                case 'O':
                    convStr = "12";
                    break;
                case '!':
                    convStr = "13";
                    break;
                // 上記以外（0〜9はそのまま返却）
                default:
                    convStr = str[i].ToString();
                    break;
            }
            rtnStr += "<sprite=" + convStr + ">";
        }
        return rtnStr;
    }
}
