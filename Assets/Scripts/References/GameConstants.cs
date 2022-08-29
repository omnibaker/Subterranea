using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    public static class SceneName
    {
        public const string MAIN = "Main";
        public const string GAMEPLAY = "Play";
        public const string STARTMENU = "StartMenu";
    }

    public static class LevelSettings
    {
        public const int CAVES_PER_LEVEL = 3;
        public const int POINT_FOR_LEVELUP = 100;
    }

    public static class GameEvent
    {
        public const string PAUSE = "Pause";
        public const string UNPAUSE = "Unpause";
    }

    public static class ResFolder
    {
        public const string CAVES = "Caves";
    }

    public static class PrefRef
    {
        public const string HIGHEST_SCORE = "highestScore";
        public const string HIGHEST_LEVEL = "highestUnlockedLevel";
    }

    public static class UIButtonLabels
    {
        public const string YES = "YES";
        public const string NO = "NO";
        public const string OK = "OK";
        public const string CANCEL = "CANCEL";
    }

    public static class UIMessages
    {
        public const string COMPLETED_GAME = "Congratulations!\nYou have completed the final cave!";
        public const string QUIT_GAME = "Do you want to end this game?";
        public const string PLAY_AGAIN = "Do you want to play again?";
    }

    public static class UIHeader
    {
        public const string GAME_OVER = "GAME OVER";
        public const string GAME_QUIT = "QUIT";
        public const string GAME_COMPLETED = "GAME COMPLETED!";
    }

    public static class UIValues
    {
        public const float DYNAMIC_WAIT = 2f;
    }

    public static class AnimationTags
    {
        public const string READY_TO_EXLODE = "IsReadyToExplode";
        public const string READY_TO_DIE = "IsReadyToDie";
    }

    public static class GameTags
    {
        public static Dictionary<int, string> Tags { get; set; } = new Dictionary<int, string>();
        public static string GetTagByEnum(TagNames name)
        {
            return Tags[(int)name];
        }
    }

    public enum FailureReason
    {
        Crashed,
        OutOfTime
    }

    public enum TagNames
    {
        CrashableObject,
        EndZone,
        BonusBox
    }

    public enum TimeLimit
    {
        MIN_1 = 60,
        MIN_2 = 120,
        MIN_3 = 180,
        MIN_5 = 300,
        MIN_10 = 600,
        SEC_10 = 10,
        SEC_30 = 30
    }

    public enum OptionPanelType
    {
        GameOver,
        GameQuit,
        GameCompleted,
    }

    public enum GameSounds
    {
        Thruster,
        BounceOfWall,
        Explode,
        LandedInEndZone,
        BonusFullShields,
        BonusOneUp
    }

    public enum GameMusic
    {
        None,
        IntroMusic,
        GameMusic
    }
}

