using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定数クラス
/// </summary>
/// <remarks>ゲーム共通の定数を管理するクラス</remarks>
namespace GameUtil
{
    public static class Const
    {
        /** リソースパス */
        public const string RESOURCES_PATH_SOUND = "Sound";
        public const string RESOURCES_PATH_EFFECT = "Effect";

        /** エフェクトキー */
        public const string EFFECT_KEY_MAGNE_PLUS = "Eff_Magne_Plus";
        public const string EFFECT_KEY_MAGNE_MINUS = "Eff_Magne_Minus";
        public const string EFFECT_KEY_SHOOTING_STAR = "Eff_ShootingStar";

        /** シーン名 */
        public const string SCENE_NAME_GAME = "GameScene";
        public const string SCENE_NAME_TITLE = "TitleScene";
        public const string SCENE_NAME_ADMOB = "AdmobScene";

        /** SoundAssetsキー(ファイル名) */
        public const string BGM_KEY_STOP = "StopBGM";
        public const string BGM_KEY_MAGTHEMA = "MAGTHEMA";
        public const string BGM_KEY_MAGTECH = "MAGTECH";
        public const string BGM_KEY_MAGGOAL = "MAGGOAL";
        public const string SOUND_KEY_MAGNE_PLUS = "se_magne_plus";
        public const string SOUND_KEY_MAGNE_MINUS = "se_magne_minus";
        public const string SOUND_KEY_CLICK = "se_click";
        public const string SOUND_KEY_LEVELUP = "se_levelup";

        /** TAGキー */
        public const string TAG_STAGE = "STAGE";
        public const string TAG_MAGNET = "MAGNET";

        /** SAVEキー */
        public const string SAVE_KEY_BEST_SCORE = "SAVE_KEY_BEST_SCORE";                   // 自己ベストスコア
        public const string SAVE_KEY_BOOL_BGM_VOLUME_OFF = "SAVE_KEY_BOOL_BGM_VOLUME_OFF"; // BGMのオンオフ
        public const string SAVE_KEY_BOOL_SE_VOLUME_OFF = "SAVE_KEY_BOOL_SE_VOLUME_OFF";   // SEのオンオフ
        public const string SAVE_KEY_BOOL_MASTER = "SAVE_KEY_BOOL_MASTER";                 // MASTERフラグ(スコア1000)

        /** LeaderBoardID(ランキング用) */
        public const string LEADER_BOARD_ID_IOS = "【ボードID(iOS)】";
        public const string LEADER_BOARD_ID_ANDROID = "【ボードID(Android)】";
    }
}
