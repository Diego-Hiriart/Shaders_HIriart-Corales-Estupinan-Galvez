using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SaveGameData
{
    private int points;//Score
    private List<BallInfo> balls = new List<BallInfo>();//Balls position and number

    public SaveGameData(int score, List<BallInfo> ballsStatus)
    {
        this.points = score;
        this.balls = ballsStatus;
    }

    public int GetPoints()
    {
        return this.points;
    }

    public List<BallInfo> GetBallsStatus()
    {
        return this.balls;
    }
}
