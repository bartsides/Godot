using System;
using System.Collections.Generic;
using Godot;

public class Tileset {
    public List<int> Floors;
    public List<int> TopWalls;
    public int TopLeftCorner;
    public int TopRightCorner;
    public int LeftWall;
    public int RightWall;
    public int BottomWall;
    public int BottomLeftWall;
    public int BottomLeftCorner;
    public int BottomRightWall;
    public int BottomRightCorner;
    public int MiddleWall;
    public int Door;

    private RandomNumberGenerator rand = new RandomNumberGenerator();

    public Tileset()
    {
        rand.Seed = (ulong) DateTime.UtcNow.Ticks;
    }

    public int GetFloor() {
        return Floors[rand.RandiRange(0, Floors.Count - 1)];
    }

    public int GetTopWall() {
        return TopWalls[rand.RandiRange(0, TopWalls.Count - 1)];
    }
}