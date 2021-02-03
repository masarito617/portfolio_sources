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
        public const string EFFECT_KEY_HIT = "Eff_Hit";
        public const string EFFECT_KEY_GUARD = "Eff_Guard";
        public const string EFFECT_KEY_HIT_COMMENT = "Eff_Hit_Cmt";
        public const string EFFECT_KEY_HIT_GOLD_COMMENT = "Eff_Hit_Gold_Cmt";
        public const string EFFECT_KEY_GUARD_COMMENT = "Eff_Guard_Cmt";
        public const string EFFECT_KEY_SCORE100_COMMENT = "Eff_Score_100_Cmt";
        public const string EFFECT_KEY_SCORE300_COMMENT = "Eff_Score_300_Cmt";
        public const string EFFECT_KEY_SCORE500_COMMENT = "Eff_Score_500_Cmt";

        /** TAG名 */
        public const string TAG_NAME_MOLE = "MOLE";
        public const string TAG_NAME_MOLEGOLD = "MOLEGOLD";
        public const string TAG_NAME_MAJIRO = "MAJIRO";

        /** Animation Key */
        public const string ANIME_KEY_ESCAPE = "isEscape";
        public const string ANIME_KEY_DEAD = "isDead";

        /** Scene名 */
        public const string SCENE_NAME_TITLE = "TitleScene";
        public const string SCENE_NAME_GAME = "GameScene";
        public const string SCENE_NAME_ADMOB = "AdmobScene";

        /** SoundAssetsキー(ファイル名) */
        public const string BGM_KEY_STOP = "StopBGM";
        public const string BGM_KEY_MOGTHEMA = "MOGTHEMA";
        public const string BGM_KEY_MOGTHEMA_LOOP = "MOGTHEMA_loop";
        public const string BGM_KEY_MOGCLE = "MOGCLE";
        public const string SOUND_KEY_CLICK = "se_click";
        public const string SOUND_KEY_HIT = "se_hit";
        public const string SOUND_KEY_HIT2 = "se_hit2";
        public const string SOUND_KEY_GUARD = "se_guard";
        public const string SOUND_KEY_WHISTLE = "se_whistle";
        public const string SOUND_KEY_COUNT = "se_count";
        public const string SOUND_KEY_GO = "se_go";

        /** SAVEキー */
        public const string SAVE_KEY_BEST_SCORE_EASY = "SAVE_KEY_BEST_SCORE_EASY";         // 自己ベストスコア
        public const string SAVE_KEY_BEST_SCORE_NORMAL = "SAVE_KEY_BEST_SCORE_NORMAL";     // 自己ベストスコア
        public const string SAVE_KEY_BEST_SCORE_HARD = "SAVE_KEY_BEST_SCORE_HARD";         // 自己ベストスコア
        public const string SAVE_KEY_BOOL_BGM_VOLUME_OFF = "SAVE_KEY_BOOL_BGM_VOLUME_OFF"; // BGMのオンオフ
        public const string SAVE_KEY_BOOL_SE_VOLUME_OFF = "SAVE_KEY_BOOL_SE_VOLUME_OFF";   // SEのオンオフ

        /** モード */
        public const string MODE_EASY = "EASY";
        public const string MODE_NORMAL = "NORMAL";
        public const string MODE_HARD = "HARD";

        /** LeaderBoardID(ランキング用) */
        public const string LEADER_BOARD_ID_EASY_IOS = "【ボードID(iOS)(やさしい)】";
        public const string LEADER_BOARD_ID_NORMAL_IOS = "【ボードID(iOS)(ふつう)";
        public const string LEADER_BOARD_ID_HARD_IOS = "【ボードID(iOS)(むずかしい)";
        public const string LEADER_BOARD_ID_EASY_ANDROID = "【ボードID(Android)(やさしい)";
        public const string LEADER_BOARD_ID_NORMAL_ANDROID = "【ボードID(Android)(ふつう)";
        public const string LEADER_BOARD_ID_HARD_ANDROID = "【ボードID(Android)(むずかしい)";

        /** AdmobID */
        public const string ADMOB_IOS_APP_ID = "【広告アプリID(iOS)】";
        public const string ADMOB_IOS_BANNER_ID = "【バナー広告ID(iOS)】";
        public const string ADMOB_IOS_INTERSTITIAL_ID = "【インタースティシャル広告ID(iOS)】";
        public const string ADMOB_ANDROID_APP_ID = "【広告アプリID(Android)】";
        public const string ADMOB_ANDROID_BANNER_ID = "【バナー広告ID(Android)】";
        public const string ADMOB_ANDROID_INTERSTITIAL_ID = "【インタースティシャル広告ID(Android)】";
    }
}
