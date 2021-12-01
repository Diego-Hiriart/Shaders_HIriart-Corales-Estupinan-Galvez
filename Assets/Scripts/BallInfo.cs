using System;
using System.Collections;
using System.Collections.Generic;

//Class that stores the info of where the ball is and what number it is
[Serializable]
public class BallInfo
{
    private float posX;
    private float posY;
    private float posZ;
    private int ballNumber;

    public BallInfo(float x, float y, float z, int number)
    {
        this.posX = x;
        this.posY = y;
        this.posZ = z;
        this.ballNumber = number;
    }

    public float GetPosX()
    {
        return this.posX;
    }

    public float GetPosY()
    {
        return this.posY;
    }

    public float GetPosZ()
    {
        return this.posZ;
    }

    public int GetBallNumber()
    {
        return this.ballNumber;
    }
}