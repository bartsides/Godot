using Godot;

public static class Helpers {
    public static uint GenerateCollisionMask(bool walls, bool player, bool enemies, bool bullets) {
        uint mask = 0;

        if (walls)
            mask |= 1 << 0;

        if (player)
            mask |= 1 << 1;
        
        if (enemies)
            mask |= 1 << 2;

        if (bullets)
            mask |= 1 << 3;

        //GD.Print($"{walls}, {player}, {enemies}, {bullets}: {mask}");

        return mask;
    }
}