using Godot;

public static class Helpers {
    public static uint GenerateCollisionMask(bool walls, bool player, bool enemies, 
        bool playerProjectiles, bool enemyProjectiles) {
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