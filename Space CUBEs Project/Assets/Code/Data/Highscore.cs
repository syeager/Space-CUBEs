// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.13
// Edited: 2014.10.14

using System;

[Serializable]
public struct Highscore
{
    public readonly int rank;
    public readonly int score;
    public readonly TimeSpan time;

    public string TimeString
    {
        get { return string.Format("{0:D2}:{1:D2}:{2:D3}", time.Minutes, time.Seconds, time.Milliseconds); }
    }


    public Highscore(int rank, int score, TimeSpan time)
    {
        this.rank = rank;
        this.score = score;
        this.time = time;
    }
}