using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NCMB;
using SocialConnector;

/// <summary>
/// タイムアタック結果シーン管理クラス
/// </summary>
/// <remarks>タイムアタック結果シーンの挙動を管理するクラス</remarks>
public class TimeAttackClearManager : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;

    /** UI部品 */
    private const string RESULT_AREA = "ResultArea";
    private const string CLEAR_TIME_NAME = "ClearTimeText";
    private const string CLEAR_STAGE_NAME = "ClearStageText";
    private const string CLEAR_BEST_IMAGE_NAME = "BestImage";
    private const string PLAYER_NAME_TEXT_NAME = "PlayerNameText";
    private const string REGIST_BUTTON_NAME = "RegistButton";
    private const string LOADING_TEXT_NAME = "LoadingText";
    private const string REGIST_MSG_TEXT_NAME = "RegistMsgText";
    private const string SEARCH_ERROR_TEXT_NAME = "SearchErrorText";
    private const string RANK_ROW = "RankRow";
    private const string NAME_ROW = "NameRow";
    private const string TIME_ROW = "TimeRow";
    GameObject resultArea;
    TextMeshProUGUI clearTimeText;
    TextMeshProUGUI clearStageText;
    GameObject clearBestImage;
    TMP_InputField playerNameText;
    GameObject registButton;
    TextMeshProUGUI loadingText;
    TextMeshProUGUI registMsgText;
    TextMeshProUGUI searchErrorText;

    /** 設定値 */
    public float clearTime;
    public string clearStageName;
    public bool bestTime;

    /** 定数 */
    private int DISP_RANK = 5; // ランキング表示数

    /** 変数 */
    bool boolInit;
    List<HighTime> dispHighRankList;

    /** ランキング結果クラス */
    public class HighTime
    {
        public string name; // 名前
        public double time; // クリアタイム

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="time">クリアタイム</param>
        public HighTime(string name, double time)
        {
            this.name = name;
            this.time = time;
        }
    }

    void Start()
    {
        // GUI部品取得
        resultArea = GameObject.Find(RESULT_AREA);
        clearTimeText = GameObject.Find(CLEAR_TIME_NAME).GetComponent<TextMeshProUGUI>();
        clearStageText = GameObject.Find(CLEAR_STAGE_NAME).GetComponent<TextMeshProUGUI>();
        clearBestImage = GameObject.Find(CLEAR_BEST_IMAGE_NAME);
        playerNameText = GameObject.Find(PLAYER_NAME_TEXT_NAME).GetComponent<TMP_InputField>();
        registButton = GameObject.Find(REGIST_BUTTON_NAME);
        loadingText = GameObject.Find(LOADING_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        registMsgText = GameObject.Find(REGIST_MSG_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        searchErrorText = GameObject.Find(SEARCH_ERROR_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        // コンポーネント取得
        assetsManager = FindObjectOfType<AssetsManager>();

        // BGM再生
        assetsManager.PlayBGM(GameUtil.Const.BGM_KEY_DOOR);

        // 読み込むまで非表示
        resultArea.SetActive(false);
    }

    void Update()
    {
        // 遷移パラメータを受け取るまで何もしない
        if (clearTime == 0.0f || clearStageName == null)
            return;

        // 結果画面の表示
        if (!boolInit)
        {
            // クリアタイムを設定
            clearStageText.text = clearStageName;
            clearTimeText.text = clearTime.ToString("n2");
            // ベストタイムなら画像表示
            clearBestImage.SetActive(bestTime);
            // 画面表示
            resultArea.SetActive(true);
            boolInit = true;
            // TOP5を更新
            FindClearTimeRankTop5();
        }
    }

    /// <summary>
    /// クリアタイムを登録する
    /// </summary>
    private void RegistClearTime()
    {
        // 名前未記入の場合、名無しに変更
        string registName = playerNameText.text;
        if (string.IsNullOrEmpty(registName))
            registName = "名無し";
        else if (registName.Length > 5)
            registName.Substring(0, 5);
        // レコードの作成
        NCMBObject record = new NCMBObject(GameUtil.Const.NCMB_HIGHTIME_TABLE);
        record[GameUtil.Const.NCMB_HIGHTIME_COL_NAME] = registName;
        record[GameUtil.Const.NCMB_HIGHTIME_COL_STAGE] = clearStageName;
        record[GameUtil.Const.NCMB_HIGHTIME_COL_TIME] = clearTime;
        // データストアへの登録
        record.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                // 保存失敗した場合
                registMsgText.text = "登録に失敗しました。";
                registMsgText.color = Color.red;
                Debug.Log("保存に失敗しました。ErrorCode : " + (string) e.ErrorMessage);
                // 登録ボタンを活性にする
                registButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                // 保存成功した場合
                registMsgText.text = "登録に成功しました。";
                registMsgText.color = Color.blue;
                // ランクリストを再表示
                FindClearTimeRankTop5();
            }
        });
    }

    /// <summary>
    /// サーバーからクリアタイムトップ5を取得
    /// </summary>  
    public void FindClearTimeRankTop5()
    {
        // 検索条件がNULLならreturn
        if (clearStageName == null)
            return;

        // ランクリストを非表示にしてローディングを表示する
        loadingText.gameObject.SetActive(true);
        searchErrorText.gameObject.SetActive(false);
        for (int i = 0; i < DISP_RANK; i++)
        {
            TextMeshProUGUI rankText = GameObject.Find(RANK_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = GameObject.Find(NAME_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = GameObject.Find(TIME_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
            rankText.color = new Color(0f, 0f, 0f, 0f);
            nameText.color = new Color(0f, 0f, 0f, 0f);
            timeText.color = new Color(0f, 0f, 0f, 0f);
        }

        // クリアタイム降順でステージ名を条件に取得
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(GameUtil.Const.NCMB_HIGHTIME_TABLE);
        query.WhereEqualTo(GameUtil.Const.NCMB_HIGHTIME_COL_STAGE, clearStageName);
        query.OrderByAscending(GameUtil.Const.NCMB_HIGHTIME_COL_TIME);
        query.Limit = DISP_RANK;
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {

            if (e != null)
            {
                // 取得失敗した場合
                Debug.Log("取得に失敗しました。ErrorCode : " + (string)e.ErrorMessage);
                // 取得失敗メッセージを表示する
                loadingText.gameObject.SetActive(false);
                searchErrorText.gameObject.SetActive(true);
            }
            else
            {
                // 取得成功した場合
                // リスト初期化
                dispHighRankList = new List<HighTime>();
                // 取得したレコードをHighScoreクラスとして保存
                foreach (NCMBObject obj in objList)
                {
                    string name = System.Convert.ToString(obj[GameUtil.Const.NCMB_HIGHTIME_COL_NAME]);
                    double time = System.Convert.ToDouble(obj[GameUtil.Const.NCMB_HIGHTIME_COL_TIME]);

                    dispHighRankList.Add(new HighTime(name, time));
                }

                // ランキングTOP5を更新
                loadingText.gameObject.SetActive(false);
                for (int i = 0; i < DISP_RANK; i++)
                {
                    TextMeshProUGUI rankText = GameObject.Find(RANK_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI nameText = GameObject.Find(NAME_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI timeText = GameObject.Find(TIME_ROW + (i + 1)).GetComponent<TextMeshProUGUI>();
                    rankText.color = new Color(0f, 0f, 0f, 255f);
                    nameText.color = new Color(0f, 0f, 0f, 255f);
                    timeText.color = new Color(0f, 0f, 0f, 255f);
                    if (dispHighRankList != null && dispHighRankList.Count > i)
                    {
                        string name = dispHighRankList[i].name;
                        string time = dispHighRankList[i].time.ToString("n2");
                        // プレイヤー名を５文字以下にする
                        if (name.Length >= 5)
                            name = name.Substring(0, 5);
                        nameText.text = name;
                        timeText.text = time + " 秒";
                    }
                    else
                    {
                        nameText.text = "-";
                        timeText.text = "--.-- 秒";
                    }
                }
            }
        });
    }

    /// <summary>
    /// SNS共有処理コルーチン
    /// </summary>
    public IEnumerator _Share()
    {
        string imgPath = Application.persistentDataPath + "/image.png";

        // 前回のデータを削除
        File.Delete(imgPath);
        // 削除が完了するまで待機
        while (true)
        {
            if (!File.Exists(imgPath)) break;
            yield return null;
        }

        // スクリーンショットを取得
        ScreenCapture.CaptureScreenshot("image.png");
        // 撮影画像の書き込みが完了するまで待機
        while (true)
        {
            if (File.Exists(imgPath)) break;
            yield return null;
        }
        // 撮影画像の保存処理のため、１フレーム待機
        yield return new WaitForEndOfFrame();

        // 投稿する
        string tweetText = clearStageName + "を" + clearTime.ToString("n2") + "秒でクリア！\n#ゴーゴーゴロヤン";
        string tweetURL = "";
        SocialConnector.SocialConnector.Share(tweetText, tweetURL, imgPath);
    }

    /// <summary>
    /// 戻るボタン押下時
    /// </summary>
    public void PushBackButton()
    {
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }

    /// <summary>
    /// 登録ボタン押下時
    /// </summary>
    public void PushRegistButton()
    {
        // 登録ボタンを非活性にする
        registButton.GetComponent<Button>().interactable = false;
        // クリアタイムを登録
        RegistClearTime();
    }

    /// <summary>
    /// 共有ボタン押下時
    /// </summary>
    public void PushShareButton()
    {
        // 共有画面を表示
        StartCoroutine(_Share());
    }
}
