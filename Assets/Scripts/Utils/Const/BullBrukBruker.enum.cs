using System;

namespace BullBrukBruker
{
    public enum BlockID
    {
        None,
        BlockBlack,
        BlockBlue,
        BlockGrey,
        BlockGreen,
        BlockOrange,
        BlockPink,
        BlockRed,
        BlockYellow,
    }

    public enum ObjectID
    {
        None = 0,
        Ball,
        Paddle,
        Item,
        HorizontalLevelBound,
        VerticalLevelBound,
        DeathBound,
    }

    public enum ItemID
    {
        None = 0,
        SpreadItem,
        TriplicateItem,
    }

    public enum PaddleStateID
    {
        None = 0,
        Ideling,
        Catching,
        Aiming,
    }

    public enum GameStateID
    {
        None = 0,
        MainMenu,
        SelectLevel,
        Load,
        Play,
        Pause,
        Win,
        Over,
        Ranking
    }

    public enum EventID
    {
        None = 0,
        SelectLevelButton_Clicked,
        PlayButton_Clicked,
        LevelButton_Clicked,
        ReturnMenuButton_Clicked,
        PauseGameButton_Clicked,
        ContinueButton_Clicked,
        StartNextAttempt,
        OutOfCells,
        OutOfLevels,
        ReplayButton_Clicked,
        HomeButton_Clicked,
        NextLevelButton_Clicked,
        RankingButton_Clicked,
        QuitButton_Clicked,
        ContactButton_Clicked,
    }

    public enum DataID
    {
        None,
        LevelProgress,
        User
    }

    public enum PrioritySoundLevel
    {
        Highest = 0,
        High = 64,
        Standard = 128,
        Low = 194,
        VeryLow = 250,
    }

    public enum AudioSequenceMode
    {
        Random,
        RandomNoImmediateRepeat,
        Sequential,
    }
}