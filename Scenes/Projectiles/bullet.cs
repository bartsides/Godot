using Godot;
using System;

public partial class bullet : projectile
{
    private AnimatedSprite2D animatedSprite;

    public override void _Ready()
    {
        CollisionLayer = Helpers.GenerateCollisionMask(false, false, false, true, false);
        CollisionMask = Helpers.GenerateCollisionMask(true, false, true, false, false);
        Damage = 20;
        MaxLifetime = 2f;
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    // public override bool HandleHitEnemy(Enemy node)
    // {
    //     if (!Active) return false;

    //     if (!base.HandleHitEnemy(node))
    //         return false;

    //     if (animatedSprite != null)
    //         animatedSprite.Play("Hit");

    //     return true;
    // }

    public void Fire(Vector2 lookAt, Vector2 linearVelocity, uint collisionMask) {
        LinearVelocity = linearVelocity;
        LookAt(lookAt);
        CollisionMask = collisionMask;
        GD.Print($"Collision mask of bullet: {CollisionMask}");
        Visible = true;
        Active = true;
        Sleeping = false;
    }
}
