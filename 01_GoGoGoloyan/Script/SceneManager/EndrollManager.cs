using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// エンドロールシーン管理クラス
/// </summary>
/// <remarks>エンドロールシーンの挙動を管理するクラス</remarks>
public class EndrollManager : MonoBehaviour
{
    /// <summary>
    /// スキップボタン押下時
    /// </summary>
    public void PushSkipButton()
    {
        // タイトル画面へ遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }

    /// <summary>
    /// 非アクティブになった時
    /// </summary>
    private void OnDisable()
    {
        // タイトル画面へ遷移
        SceneManager.LoadScene(GameUtil.Const.SCENE_NAME_TITLE);
    }
}
