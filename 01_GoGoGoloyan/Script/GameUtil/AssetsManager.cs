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
    public IDictionary<string, AudioClip> soundAssetsMap = new Dictionary<string, AudioClip>(); // SoundAsset管理Map
    private string playingBgmScene; // BGM再生中のシーン名

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
        // ボリュームの調整
        float volume = 1.0f;
        switch (key)
        {
            case GameUtil.Const.SOUND_KEY_DELETE:
                volume = 0.4f;
                break;
            case GameUtil.Const.SOUND_KEY_ATTACK:
            case GameUtil.Const.SOUND_KEY_GAMEOVER:
            case GameUtil.Const.SOUND_KEY_BOYON:
            case GameUtil.Const.SOUND_KEY_DIMAX:
            case GameUtil.Const.SOUND_KEY_DIMAX_RELEASE:
                volume = 0.5f;
                break;
            case GameUtil.Const.SOUND_KEY_CLICK:
            case GameUtil.Const.SOUND_KEY_BOMB:
            case GameUtil.Const.SOUND_KEY_JUMP:
            case GameUtil.Const.SOUND_KEY_BANE:
                volume = 0.7f;
                break;
            case GameUtil.Const.SOUND_KEY_WIND:
                volume = 0.9f;
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
    public void PlayBGM(string key)
    {
        // 停止キーなら停止して返却
        if (GameUtil.Const.BGM_KEY_STOP == key)
        { 
            StopBGM();
            return;
        }
        // 既に同じBGMが再生中ならそのまま
        AudioClip audioClip = getAudioClip(key);
        if (bgmAudioSource.clip == audioClip)
            return;
        // 再生中なら停止
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();
        // ボリュームの調整
        float volume = 1.0f;
        switch (key)
        {
            case GameUtil.Const.BGM_KEY_GOROTHEMA:
                volume = 1.0f;
                break;
            case GameUtil.Const.BGM_KEY_FOREST:
            case GameUtil.Const.BGM_KEY_SEA:
            case GameUtil.Const.SOUND_KEY_BOMB:
                volume = 0.6f;
                break;
            case GameUtil.Const.BGM_KEY_BANANAMIDA:
            case GameUtil.Const.BGM_KEY_HOPING:
                volume = 0.35f;
                break;
            case GameUtil.Const.BGM_KEY_BANANAMAN:
            case GameUtil.Const.BGM_KEY_DOOR:
                volume = 0.25f;
                break;
            case GameUtil.Const.BGM_KEY_ATTENSION:
            case GameUtil.Const.BGM_KEY_SUPERSTAR:
            case GameUtil.Const.BGM_KEY_FLAY:
                volume = 0.2f;
                break;
        }
        // BGM再生
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
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
    public AudioClip getPlayingAudio()
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
                PlayBGM(GameUtil.Const.BGM_KEY_GOROTHEMA);
                break;
            // イベントシーン
            case GameUtil.Const.SCENE_NAME_EVENT:
                // イベント内容によって切替
                EventManager eventManager = FindObjectOfType<EventManager>();
                switch (eventManager.loadEventParam)
                {
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_START:
                        PlayBGM(GameUtil.Const.BGM_KEY_FOREST);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE1_END:
                        PlayBGM(GameUtil.Const.BGM_KEY_DOOR);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_START:
                        PlayBGM(GameUtil.Const.BGM_KEY_SEA);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END:
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE2_END_2:
                        PlayBGM(GameUtil.Const.BGM_KEY_FOREST);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_START:
                        PlayBGM(GameUtil.Const.BGM_KEY_SUPERSTAR);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE3_END:
                        PlayBGM(GameUtil.Const.BGM_KEY_DOOR);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_START:
                        PlayBGM(GameUtil.Const.BGM_KEY_FOREST);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE4_END:
                        PlayBGM(GameUtil.Const.BGM_KEY_BANANAMAN);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_START:
                        PlayBGM(GameUtil.Const.BGM_KEY_STOP);
                        break;
                    case GameUtil.Const.FUNGUS_KEY_EVENT_EPISODE5_END:
                        PlayBGM(GameUtil.Const.BGM_KEY_STOP);
                        break;
                }
                break;
            // 各ステージシーン
            case GameUtil.Const.SCENE_NAME_STAGE1:
                PlayBGM(GameUtil.Const.BGM_KEY_HOPING);
                break;
            case GameUtil.Const.SCENE_NAME_STAGE2:
                PlayBGM(GameUtil.Const.BGM_KEY_SEA);
                break;
            case GameUtil.Const.SCENE_NAME_STAGE3:
                PlayBGM(GameUtil.Const.BGM_KEY_FLAY);
                break;
            case GameUtil.Const.SCENE_NAME_STAGE4:
                PlayBGM(GameUtil.Const.BGM_KEY_BANANAMAN);
                break;
            case GameUtil.Const.SCENE_NAME_STAGE5:
                PlayBGM(GameUtil.Const.BGM_KEY_ATTENSION);
                break;
        }
    }
}
