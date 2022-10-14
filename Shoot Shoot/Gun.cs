using Godot;

public class Gun : Node2D
{
    public float Speed = 800;
    private PackedScene bulletScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bulletScene = GD.Load<PackedScene>("res://Shoot Shoot/Bullet.tscn");
    }

    public Projectile Shoot(Vector2 direction) {
        // TODO: Shoot gun sound and smoke

        var bullet = bulletScene.Instance<Bullet>();
        AddChild(bullet);
        bullet.Position = GetNode<Position2D>("BulletPosition").Position;
        bullet.LinearVelocity = direction * Speed;
        bullet.ZIndex = -1;
        return bullet;
    }
}
