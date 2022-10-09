using Godot;
using System;

public class Enemy : RigidBody2D
{
    public decimal Health { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Health = 40;
    }

    public void Hit(decimal damage) {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    private void Die() {
        GetParent().RemoveChild(this);
    }
}
