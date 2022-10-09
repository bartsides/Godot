using Godot;

public class Bullet : Projectile
{
    public override void _Ready()
    {
        Damage = 20;
        MaxLifetime = 2f;
    }

    public override bool HandleHitEnemy(Enemy node)
    {
        if (!base.HandleHitEnemy(node))
            return false;
        GD.Print("!!!");
        return true;
    }
}
