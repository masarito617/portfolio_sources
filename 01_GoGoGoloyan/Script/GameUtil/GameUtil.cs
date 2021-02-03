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

        /** セーブキー */
        public const string SAVE_KEY_TUTORIAL = "tutorialEnd";
        public const string SAVE_KEY_CLEAR_STAGE1 = "boolClearStage1";
        public const string SAVE_KEY_CLEAR_STAGE2 = "boolClearStage2";
        public const string SAVE_KEY_CLEAR_STAGE3 = "boolClearStage3";
        public const string SAVE_KEY_CLEAR_STAGE4 = "boolClearStage4";
        public const string SAVE_KEY_CLEAR_STAGE5 = "boolClearStage5";
        public const string SAVE_KEY_BEST_CLEAR_TIME_STAGE1 = "floatClearTimeStage1";
        public const string SAVE_KEY_BEST_CLEAR_TIME_STAGE2 = "floatClearTimeStage2";
        public const string SAVE_KEY_BEST_CLEAR_TIME_STAGE3 = "floatClearTimeStage3";
        public const string SAVE_KEY_BEST_CLEAR_TIME_STAGE4 = "floatClearTimeStage4";
        public const string SAVE_KEY_BEST_CLEAR_TIME_STAGE5 = "floatClearTimeStage5";

        /** オブジェクト名 */
        public const string GOROYAN_NAME = "GOROYAN";
        public const string CONTROL_UI_NAME = "ControlUI";

        /** UI部品名 */
        public const string UI_IMAGE_ZANGORO = "ZangoroImage";
        public const string UI_IMAGE_GAMEOVER = "GameOverImage";
        public const string UI_TEXT_TIME = "TimeText";
        public const string UI_TEXT_ELAPSED_TIME = "ElapsedTimeText";

        /** TAG名 */
        public const string TAG_NAME_PLAYER = "Player";
        public const string TAG_NAME_GOAL = "GOAL";
        public const string TAG_NAME_KANI = "KANI";
        public const string TAG_NAME_UNI = "UNI";
        public const string TAG_NAME_KARASU = "KARASU";
        public const string TAG_NAME_TAMA = "TAMA";
        public const string TAG_NAME_HARI = "HARI";
        public const string TAG_NAME_BANAKING = "BANAKING";
        public const string TAG_NAME_KYOBANA = "KYOBANA";
        public const string TAG_NAME_DELETE_ZONE = "DELETEZONE";

        /** シーン名 */
        public const string SCENE_NAME_TITLE = "TitleScene";
        public const string SCENE_NAME_EVENT = "EventScene";
        public const string SCENE_NAME_STAGE1 = "Stage1Scene";
        public const string SCENE_NAME_STAGE2 = "Stage2Scene";
        public const string SCENE_NAME_STAGE3 = "Stage3Scene";
        public const string SCENE_NAME_STAGE4 = "Stage4Scene";
        public const string SCENE_NAME_STAGE5 = "Stage5Scene";
        public const string SCENE_NAME_END    = "EndScene";
        public const string SCENE_NAME_ADMOB  = "AdmobScene";
        public const string SCENE_NAME_TIMEATTACK_CLEAR = "TimeAttackClearScene";

        /** SoundAssetsキー(ファイル名) */
        public const string BGM_KEY_STOP = "StopBGM";
        public const string BGM_KEY_GOROTHEMA = "GOROTHEMA";
        public const string BGM_KEY_FOREST = "FOREST";
        public const string BGM_KEY_SEA = "SEA";
        public const string BGM_KEY_HOPING = "HOPING";
        public const string BGM_KEY_DOOR = "DOOR";
        public const string BGM_KEY_BANANAMIDA = "BANANAMIDA";
        public const string BGM_KEY_BANANAMAN = "BANANAMAN";
        public const string BGM_KEY_FLAY = "FLAY";
        public const string BGM_KEY_SUPERSTAR = "SUPERSTAR";
        public const string BGM_KEY_ATTENSION = "aTTension!!";
        public const string SOUND_KEY_ATTACK = "se_attack";
        public const string SOUND_KEY_JUMP = "se_jump";
        public const string SOUND_KEY_JUMP_LONG = "se_jump_long";
        public const string SOUND_KEY_WIND = "se_wind";
        public const string SOUND_KEY_DELETE = "se_delete";
        public const string SOUND_KEY_CLICK = "se_click";
        public const string SOUND_KEY_GAMEOVER = "se_gameover";
        public const string SOUND_KEY_BOYON = "se_boyon";
        public const string SOUND_KEY_BANE = "se_bane";
        public const string SOUND_KEY_BOMB = "se_bomb"; 
        public const string SOUND_KEY_DIMAX = "se_dimax";
        public const string SOUND_KEY_DIMAX_RELEASE = "se_dimax_release";

        /** Fungusキー */
        public const string FUNGUS_CONTINUE_BLOCK = "ContinueBlock";
        public const string FUNGUS_END_BLOCK = "EndBlock";
        public const string FUNGUS_KEY_EVENT_END = "EventEnd";
        public const string FUNGUS_KEY_BACK_END = "BackEnd";
        public const string FUNGUS_KEY_PLAY_BGM = "PlayBGM";
        public const string FUNGUS_KEY_PLAY_SE = "PlaySE";
        public const string FUNGUS_KEY_SET_BACKGROUD = "SetBackground";
        public const string FUNGUS_KEY_EVENT_TUTORIAL_START = "StartTutorial";
        public const string FUNGUS_KEY_EVENT_EPISODE1_START = "Episode1Start";
        public const string FUNGUS_KEY_EVENT_EPISODE1_END   = "Episode1End";
        public const string FUNGUS_KEY_EVENT_EPISODE2_START = "Episode2Start";
        public const string FUNGUS_KEY_EVENT_EPISODE2_END   = "Episode2End";
        public const string FUNGUS_KEY_EVENT_EPISODE2_END_2 = "Episode2End_2";
        public const string FUNGUS_KEY_EVENT_EPISODE3_START = "Episode3Start";
        public const string FUNGUS_KEY_EVENT_EPISODE3_END   = "Episode3End";
        public const string FUNGUS_KEY_EVENT_EPISODE4_START = "Episode4Start";
        public const string FUNGUS_KEY_EVENT_EPISODE4_END   = "Episode4End";
        public const string FUNGUS_KEY_EVENT_EPISODE5_START = "Episode5Start";
        public const string FUNGUS_KEY_EVENT_EPISODE5_END   = "Episode5End";

        /** Animationキー */
        public const string ANIMETION_NAME_ATTACK = "Attack";
        public const string ANIMATION_KEY_JUMP_TRIGGER = "JumpTrigger";
        public const string ANIMATION_KEY_ATTACK_TRIGGER = "AttackTrigger";
        public const string ANIMATION_KEY_BANAKING_ATTACK_TRIGGER = "AttackTrigger";
        public const string ANIMATION_KEY_BANAKING_GUARD_TRIGGER = "GuardTrigger";
        public const string ANIMATION_KEY_BANAKING_SHAKE_TRIGGER = "ShakeTrigger";
        public const string ANIMATION_KEY_BANAKING_DAMAGE_TRIGGER = "DamageTrigger";

        /** NCMBテーブル情報 */
        public const string NCMB_HIGHTIME_TABLE = "HighTimeData";
        public const string NCMB_HIGHTIME_COL_NAME = "name";
        public const string NCMB_HIGHTIME_COL_STAGE = "stage";
        public const string NCMB_HIGHTIME_COL_TIME = "time";
    }
}
