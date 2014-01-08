// Steve Yeager
// 1.5.2014

using System;

public class PointsUpdateArgs : EventArgs
{
    public readonly int pointsGained;
    public readonly int points;


    public PointsUpdateArgs(int pointsGained, int points)
    {
        this.pointsGained = pointsGained;
        this.points = points;
    }
}
