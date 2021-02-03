using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// もぐら結果操作クラス
/// </summary>
/// <remarks>もぐら結果の表示を管理するクラス</remarks>
public class ResultMoleController : MonoBehaviour
{
    /** コンポーネント */
    private const string UI_IMAGE_RESULT_BEST = "ResultBestImage";
    private Image image;
    private Image resultBestImage;

    /** 設定値 */
    public Sprite img_mole_bad_ja;
    public Sprite img_mole_good_ja;
    public Sprite img_mole_perfect_ja;
    public Sprite img_majiro_bad_ja;
    public Sprite img_majiro_good_ja;
    public Sprite img_majiro_perfect_ja;
    public Sprite img_mole_bad_en;
    public Sprite img_mole_good_en;
    public Sprite img_mole_perfect_en;
    public Sprite img_majiro_bad_en;
    public Sprite img_majiro_good_en;
    public Sprite img_majiro_perfect_en;
    public Sprite img_mole_bad;
    public Sprite img_mole_good;
    public Sprite img_mole_perfect;
    public Sprite img_majiro_bad;
    public Sprite img_majiro_good;
    public Sprite img_majiro_perfect;

    // スコアリスト { MOLE_MAX, MOLE_NORMA, MAJIRO_NORMA, MAJIRO_MAX }
    int[] e_ScoreList = { 3000, 2000, -1000, -1500 };
    int[] n_ScoreList = { 5000, 3500, -2000, -3000 };
    int[] h_ScoreList = { 10000, 6500, -4500, -7200 };

    Sprite dispSprite;
    bool dispFlg = false; // 表示フラグ
    bool bestFlg = false; // ベストフラグ

    private void Start()
    {
        // 結果モグラの設定
        image = GetComponent<Image>();
        // 透明にする
        Color color = image.color;
        color.a = 0;
        image.color = color;

        // ベストイメージの設定
        resultBestImage = GameObject.Find(UI_IMAGE_RESULT_BEST).GetComponent<Image>();
        // 透明にする
        Color bestColor = resultBestImage.color;
        bestColor.a = 0;
        resultBestImage.color = bestColor;
    }

    private void Update()
    {
        // 表示フラグがONの場合
        if (dispFlg)
        {
            // 結果モグラが透明な場合、フェードインさせる
            if (image.color.a < 255f)
            {
                Color color = image.color;
                color.a += 0.025f;
                image.color = color;
            }
            // ベストスコアの場合
            if (bestFlg && resultBestImage.color.a < 255f)
            {
                Color bestColor = resultBestImage.color;
                bestColor.a += 0.025f;
                resultBestImage.color = bestColor;
            }
        }
    }

    /// <summary>
    /// スコア設定処理
    /// </summary>
    /// <param name="mode">モード</param>
    /// <param name="score">スコア</param>
    public void SetResultMole(string mode, int score)
    {
        // 言語設定によって設定する画像を切り替える
        SystemLanguage sl = Application.systemLanguage;
        switch (sl)
        {
            // ＊＊＊ 日本語 ＊＊＊
            case SystemLanguage.Japanese:
                img_mole_bad = img_mole_bad_ja;
                img_mole_good = img_mole_good_ja;
                img_mole_perfect = img_mole_perfect_ja;
                img_majiro_bad = img_majiro_bad_ja;
                img_majiro_good = img_majiro_good_ja;
                img_majiro_perfect = img_majiro_perfect_ja;
                break;
            // ＊＊＊ 英語（デフォルト） ＊＊＊
            case SystemLanguage.English:
            default:
                img_mole_bad = img_mole_bad_en;
                img_mole_good = img_mole_good_en;
                img_mole_perfect = img_mole_perfect_en;
                img_majiro_bad = img_majiro_bad_en;
                img_majiro_good = img_majiro_good_en;
                img_majiro_perfect = img_majiro_perfect_en;
                break;
        }
        // モードによって判定スコア、保存キーを切り替える
        int[] scoreList = null;
        string bestSaveKey = null;
        switch (mode)
        {
            case GameUtil.Const.MODE_EASY:
                scoreList = e_ScoreList;
                bestSaveKey = GameUtil.Const.SAVE_KEY_BEST_SCORE_EASY;
                break;
            case GameUtil.Const.MODE_NORMAL:
                scoreList = n_ScoreList;
                bestSaveKey = GameUtil.Const.SAVE_KEY_BEST_SCORE_NORMAL;
                break;
            case GameUtil.Const.MODE_HARD:
                scoreList = h_ScoreList;
                bestSaveKey = GameUtil.Const.SAVE_KEY_BEST_SCORE_HARD;
                break;
        }
        // スコアによって画像を切り替える
        if (score == scoreList[0])
            dispSprite = img_mole_perfect;
        else if (score >= scoreList[1])
            dispSprite = img_mole_good;
        else if (score >= 0)
            dispSprite = img_mole_bad;
        else if (score > scoreList[2])
            dispSprite = img_majiro_bad;
        else if (score > scoreList[3])
            dispSprite = img_majiro_good;
        else if (score == scoreList[3])
            dispSprite = img_majiro_perfect;
        // ベストスコアの場合、保存する
        float bestScore = GameSystemManager.GetFloat(bestSaveKey);
        if (bestScore == 0 || bestScore < score)
        {
            GameSystemManager.SetFloat(bestSaveKey, score);
            bestFlg = true;
        }
        // 表示させる
        Invoke("DispResultMole", 1.0f);
    }

    /// <summary>
    /// 結果もぐら表示処理
    /// </summary>
    private void DispResultMole()
    {
        image.sprite = dispSprite;
        dispFlg = true;
    }
}
