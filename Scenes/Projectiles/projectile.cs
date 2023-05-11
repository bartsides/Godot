using Godot;

public partial class projectile : RigidBody2D
{
    [Export]
    public virtual double MaxLifetime { get; set; } = 5;
    [Export]
    public virtual double Damage { get;  set; } = 20;
    [Export]
    public virtual int MaxEnemiesHit { get; set; } = 1;
    [Export]
    public virtual int MaxBounces { get; set; } = 1;

    protected bool Active { get; set; } = false;
    private Timer lifetimeTimer;

    public override void _Ready()
    {
        lifetimeTimer = new Timer(MaxLifetime, active: true);
    }

    // public virtual bool HandleHitEnemy(Enemy node) {
    //     node.Hit(Damage);

    //     MaxEnemiesHit--;
    //     if (MaxEnemiesHit > 0) return true;

    //     Die();
    //     return false;
    // }

    public virtual bool HandleHitWall(TileMap node) {
        MaxBounces--;
        if (MaxBounces > 0) return true;

        Die();
        return false;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);
        // for (var i = 0; i < state.GetContactCount(); i++) {
        //     var node = state.GetContactColliderObject(i);
        //     if (node is Enemy enemy) {
        //         if (!HandleHitEnemy(enemy))
        //             return;
        //     }
        //     else if (node is TileMap tilemap) {
        //         if (!HandleHitWall(tilemap))
        //             return;
        //     }
        // }
    }

    public override void _Process(double delta)
    {
        if (lifetimeTimer.Process(delta)) {
            Die();
        }
    }

    protected void Die() {
        GD.Print("Bullet dying");
        GetParent().RemoveChild(this);
    }
}