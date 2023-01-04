using Godot;

public class Bullet : Projectile
{
    private AnimatedSprite animatedSprite;

    public override void _Ready()
    {
        Damage = 20;
        MaxLifetime = 2f;

        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override bool HandleHitEnemy(Enemy node)
    {
        if (!Active) return false;

        if (!base.HandleHitEnemy(node))
            return false;

        if (animatedSprite != null)
            animatedSprite.Play("Hit");

        return true;
    }

    public void Fire(Vector2 position, Vector2 linearVelocity) {
        Position = position;
        LinearVelocity = linearVelocity;
        LookAt(GetGlobalMousePosition());
        Visible = true;
        Active = true;
    }
}
