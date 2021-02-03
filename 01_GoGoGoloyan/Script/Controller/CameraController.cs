using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// カメラ操作クラス
/// </summary>
/// <remarks>カメラの挙動を管理するクラス</remarks>
public class CameraController : MonoBehaviour
{
    /** コンポーネント */
    GameObject goroyan;

    /** 変数 */
    Vector3 initPosition;

    void Start()
    {
        goroyan = GameObject.Find(GameUtil.Const.GOROYAN_NAME);
        initPosition = transform.position;
    }

    void Update()
    {
        // ゴロヤンがいない場合処理しない
        if (goroyan == null)
            return;

        // カメラの位置をゴロヤンに合わせる
        Vector3 goroyanPos = goroyan.transform.position;
        // ステージによってカメラ移動の挙動を変える
        switch (SceneManager.GetActiveScene().name)
        {
            // ＊＊＊ 縦スクロール画面のステージ ＊＊＊
            case GameUtil.Const.SCENE_NAME_STAGE1:
            case GameUtil.Const.SCENE_NAME_STAGE3:
                // ゴロヤンの縦位置に合わせる
                // カメラが初期位置の高さより下がらないようにする
                if (goroyanPos.y > initPosition.y)
                    transform.position = new Vector3(transform.position.x, goroyanPos.y, transform.position.z);
                break;
            // ＊＊＊ 横スクロール画面のステージ ＊＊＊
            case GameUtil.Const.SCENE_NAME_STAGE2:
            case GameUtil.Const.SCENE_NAME_STAGE4:
                // ゴロヤンの横位置に合わせる
                transform.position = new Vector3(goroyanPos.x + 1.2f, transform.position.y, transform.position.z);
                break;
            // ＊＊＊ 固定画面のステージ ＊＊＊
            case GameUtil.Const.SCENE_NAME_STAGE5:
                // カメラも固定
                break;
        }
    }

    /// <summary>
    /// 初期位置にもどす
    /// </summary>
    public void SetInitPosition()
    {
        transform.position = initPosition;
    }
}
