using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボリューム操作クラス
/// </summary>
/// <remarks>ボリューム操作の挙動を管理するクラス</remarks>
public class VolumeButtonController : MonoBehaviour
{
    /** コンポーネント */
    AssetsManager assetsManager;

    /** 設定値 */
    public Sprite volumeOnImage;
    public Sprite volumeOffImage;
    public Kind kind;
    public enum Kind
    {
        BGM, // BGMの切り替え
        SE,  // SEの切り替え
    };

    /** 変数 */
    private string kindSaveKey;

    private void Start()
    {
        assetsManager = FindObjectOfType<AssetsManager>();

        // 保存キーの取得
        if (kind == Kind.BGM)
            kindSaveKey = GameUtil.Const.SAVE_KEY_BOOL_BGM_VOLUME_OFF;
        else if (kind == Kind.SE)
            kindSaveKey = GameUtil.Const.SAVE_KEY_BOOL_SE_VOLUME_OFF;

        // ボタンイメージの切り替え
        bool boolVolumeOff = GameSystemManager.GetBool(kindSaveKey);
        ChangeButtonImage(boolVolumeOff);
    }

    /// <summary>
    /// 音量ボタン押下時
    /// </summary>
    public void PushVolumeButton()
    {
        bool boolVolumeChange = !GameSystemManager.GetBool(kindSaveKey);
        // ボタンイメージの切り替え
        ChangeButtonImage(boolVolumeChange);
        // 音量設定を変更する
        assetsManager.ChangeVolumeOnOff(kindSaveKey, boolVolumeChange);
        // オンオフ情報を保存
        GameSystemManager.SetBool(kindSaveKey, boolVolumeChange);
    }

    /// <summary>
    /// ボタンイメージ差し替え処理
    /// </summary>
    /// <param name="boolVolumeOff">ボリュームオンオフフラグ</param>
    private void ChangeButtonImage(bool boolVolumeOff)
    {
        if (boolVolumeOff)
            gameObject.GetComponent<Button>().GetComponent<Image>().sprite = volumeOffImage;
        else
            gameObject.GetComponent<Button>().GetComponent<Image>().sprite = volumeOnImage;
    }
}
