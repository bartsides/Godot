// Feel free to move these global usings to something like GameMaster.cs or Globals.cs
global using Godot;
global using System;

public static class Helpers {
    public static uint GenerateCollisionMask(bool walls = false, bool player = false, 
        bool enemies = false, bool playerProjectiles = false, bool enemyProjectiles = false) 
    {
        uint mask = 0;

        if (walls)
            mask |= 1 << 0;

        if (player)
            mask |= 1 << 1;
        
        if (enemies)
            mask |= 1 << 2;

        if (playerProjectiles)
            mask |= 1 << 3;

        if (enemyProjectiles)
            mask |= 1 << 4;

        return mask;
    }
}