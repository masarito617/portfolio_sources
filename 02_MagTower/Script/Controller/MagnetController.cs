using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimplePhysicsToolkit;

/// <summary>
/// マグネ操作クラス
/// </summary>
/// <remarks>マグネの挙動を管理するクラス</remarks>
public class MagnetController : MonoBehaviour
{
    /** 設定値 */
    public bool initMagnet = false; // 初期マグネ
    public State status;            // ステータス

    /** コンポーネント */
    CameraController cameraController;
    GameSceneManager gameSceneManager;

    /** 変数 */
    private float gameOverBaseTime = 5.0f; // ゲームオーバー残り時間
    private float gameOverTime;            // ゲームオーバー残り時間(格納用)

    // ステータス
    public enum State
    {
        DROP,    // 落下状態
        TOWER,   // タワー(非マグネット)状態
        TOWER_MG // タワー(マグネット)状態
    }

    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        gameSceneManager = FindObjectOfType<GameSceneManager>();
        gameOverTime = gameOverBaseTime;

        if (initMagnet)
        {
            // *** 初期マグネ ***
            // タワー(マグネット)状態に遷移
            status = State.TOWER_MG;
            GetComponent<Magnet>().enable = true;
            GetComponent<Rigidbody>().isKinematic = true;
            // カメラターゲットを変更
            cameraController.SetLookTarget(gameObject);
        }
        else
        {
            // *** 落下マグネ ***
            // 落下状態に遷移
            status = State.DROP;
            // マグネコンポーネントを無効化
            GetComponent<Magnet>().enable = false;
        }
    }

    void Update()
    {
        // タイトル画面は対象外
        if (SceneManager.GetActiveScene().name == GameUtil.Const.SCENE_NAME_TITLE)
            return;

        // 落下したら破棄する
        if (transform.position.y < -10)
            Destroy(gameObject);

        // 落下オブジェクトのままで3秒以上経ったらゲームオーバー
        if (status == State.DROP)
        {
            gameOverTime -= Time.deltaTime;
            if (gameOverTime < 0)
                gameSceneManager.GameOver();
        }
    }

    /// <summary>
    /// コライダ衝突時のイベント
    /// </summary>
    /// <param name="collision">衝突対象</param>
    private void OnCollisionEnter(Collision collision)
    {
        // マグネット以外は処理対象外
        if (collision.transform.tag != GameUtil.Const.TAG_MAGNET)
            return;
        
        // 衝突対象のマグネを取得
        MagnetController colMagController = collision.transform.GetComponent<MagnetController>();
        // 衝突対象が落下状態で、自身がタワー(マグネット)状態の場合
        if (status == State.TOWER_MG && colMagController.status == State.DROP)
        {
            // マグネ結合処理
            if (GetComponent<Magnet>().attract)
            {
                // 落下マグネの高さ以外の位置・回転を合わせる
                collision.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.transform.rotation = Quaternion.Euler(0.0f, transform.localEulerAngles.y % 360.0f - 180.0f, 0.0f);

                // タワー(非マグネット)状態に遷移
                colMagController.status = State.TOWER_MG;
                collision.transform.GetComponent<Magnet>().enable = true;
                collision.transform.GetComponent<Rigidbody>().isKinematic = true;
                // 自身のマグネットを無効化
                status = State.TOWER;
                GetComponent<Magnet>().enable = false;
                // カメラターゲットを変更
                cameraController.SetLookTarget(collision.gameObject);
                // スコアを加算する
                gameSceneManager.AddScore();
            }
        }
    }
}
