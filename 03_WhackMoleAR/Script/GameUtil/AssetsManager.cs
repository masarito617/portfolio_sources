using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// アセット管理クラス
/// </summary>
/// <remarks>アセットを管理する共通クラス</remarks>
public class AssetsManager : MonoBehaviour
{
    /** コンポーネント */
    public AudioSource seAudioSource;
    public AudioSource bgmAudioSource;

    /** 変数 */
    IDictionary<string, AudioClip> soundAssetsMap = new Dictionary<string, AudioClip>();    // SoundAsset管理Map
    IDictionary<string, GameObject> effectAssetsMap = new Dictionary<string, GameObject>(); // EffectAsset管理Map
    private string playingBgmScene;
    private bool boolBGMOff;
    private bool boolSEOff;

    private void Awake()
    {
        // ゲーム内に一つだけ保持
        if (FindObjectsOfType<AssetsManager>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        seAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // ボリュームオンオフ情報を取得
        boolBGMOff = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_BOOL_BGM_VOLUME_OFF);
        boolSEOff = GameSystemManager.GetBool(GameUtil.Const.SAVE_KEY_BOOL_SE_VOLUME_OFF);
    }

    private void Update()
    {
        // シーンが切り替わった時、初期BGMを再生
        if (SceneManager.GetActiveScene().name != playingBgmScene)
        {
            playingBgmScene = SceneManager.GetActiveScene().name;
            PlayFirstBGM();
        }
    }

    /// <summary>
    /// キーからエフェクトを取得する
    /// </summary>
    /// <param name="key">エフェクトに紐づくキー</param>
    /// <returns>エフェクトオブジェクト</returns>
    public GameObject getEffectObject(string key)
    {
        // キーが存在しない場合、初期値設定
        if (!effectAssetsMap.ContainsKey(key))
            effectAssetsMap.Add(key, null);

        // キーに対応するリソースがNULLなら読み込み
        if (effectAssetsMap[key] == null)
            effectAssetsMap[key] = Resources.Load(GameUtil.Const.RESOURCES_PATH_EFFECT + "/" + key) as GameObject;
        return effectAssetsMap[key];
    }

    /// <summary>
    /// キーから効果音を取得する
    /// </summary>
    /// <param name="key">効果音に紐づくキー</param>
    /// <returns>効果音</returns>
    public AudioClip getAudioClip(string key)
    {
        // キーが存在しない場合、初期値設定
        if (!soundAssetsMap.ContainsKey(key))
            soundAssetsMap.Add(key, null);

        // キーに対応するリソースがNULLなら読み込み
        if (soundAssetsMap[key] == null)
            soundAssetsMap[key] = Resources.Load(GameUtil.Const.RESOURCES_PATH_SOUND + "/" + key) as AudioClip;
        return soundAssetsMap[key];
    }

    /// <summary>
    /// キーに紐づく効果音を再生する
    /// </summary>
    /// <param name="key">効果音に紐づくキー</param>
    public void PlayOneShot(string key)
    {
        // ボリュームオフの場合、再生しない
        if (boolSEOff)
            return;

        // ボリュームの調整
        float volume = 1.0f;
        switch (key)
        {
            case GameUtil.Const.SOUND_KEY_HIT:
            case GameUtil.Const.SOUND_KEY_COUNT:
            case GameUtil.Const.SOUND_KEY_GO:
                volume = 1.0f;
                break;
            
            case GameUtil.Const.SOUND_KEY_GUARD:
                volume = 0.7f;
                break;
                case GameUtil.Const.SOUND_KEY_CLICK:
            case GameUtil.Const.SOUND_KEY_WHISTLE:
                volume = 0.5f;
                break;
        }
        // 効果音再生
        AudioClip audioClip = getAudioClip(key);
        seAudioSource.PlayOneShot(audioClip, volume);
    }

    /// <summary>
    /// キーに紐づくBGMを再生する
    /// </summary>
    /// <param name="key">BGMに紐づくキー</param>
    public void PlayBGM(string key, bool loop)
    {
        // 停止キーなら停止して返却
        if (GameUtil.Const.BGM_KEY_STOP == key)
        {
            StopBGM();
            return;
        }
        // 既に同じBGMが再生中ならそのまま
        AudioClip audioClip = getAudioClip(key);
        if (bgmAudioSource.clip == audioClip && bgmAudioSource.isPlaying)
            return;
        // 再生中なら停止
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();
        // ボリュームの調整
        float volume = 1.0f;
        switch (key)
        {
            case GameUtil.Const.BGM_KEY_MOGTHEMA:
                volume = 0.8f;
                break;
            case GameUtil.Const.BGM_KEY_MOGCLE:
                volume = 0.6f;
                break;
            case GameUtil.Const.BGM_KEY_MOGTHEMA_LOOP:
                volume = 0.5f;
                break;
        }
        // BGM再生
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.loop = loop;
        bgmAudioSource.Play();
        // ボリュームオフの場合、停止する
        if (boolBGMOff)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }
    }

    /// <summary>
    /// 再生中のBGMを停止する
    /// </summary>
    public void StopBGM()
    {
        // 再生中なら停止
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();
    }

    /// <summary>
    /// 再生中のBGMを返却
    /// </summary>
    /// <returns>再生中のBGM</returns>
    public AudioClip GetPlayingAudio()
    {
        if (!bgmAudioSource.isPlaying)
            return null;
        return bgmAudioSource.clip;
    }

    /// <summary>
    /// 各シーンの初期BGM再生
    /// </summary>
    public void PlayFirstBGM()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            // タイトルシーン
            case GameUtil.Const.SCENE_NAME_TITLE:
                PlayBGM(GameUtil.Const.BGM_KEY_MOGTHEMA, true);
                break;
            // ゲームシーン
            case GameUtil.Const.SCENE_NAME_GAME:
                PlayBGM(GameUtil.Const.BGM_KEY_MOGTHEMA_LOOP, true);
                break;
        }
    }

    /// <summary>
    /// ボリュームオンオフ情報設定
    /// </summary>
    /// <param name="key">ボリューム調整対象キー（BGM, SE）</param>
    /// <param name="boolVolumeOff">ボリュームオンオフフラグ</param>
    public void ChangeVolumeOnOff(string key, bool boolVolumeOff)
    {
        // オンオフ情報の設定
        if (key == GameUtil.Const.SAVE_KEY_BOOL_BGM_VOLUME_OFF)
            boolBGMOff = boolVolumeOff;
        else if (key == GameUtil.Const.SAVE_KEY_BOOL_SE_VOLUME_OFF)
            boolSEOff = boolVolumeOff;
        // BGM再生中なら音量を停止
        if (key == GameUtil.Const.SAVE_KEY_BOOL_BGM_VOLUME_OFF)
        {
            if (boolBGMOff)
            {
                bgmAudioSource.Stop();
                bgmAudioSource.clip = null;
            }
            else
            {
                PlayFirstBGM();
            }
        }
    }
}
