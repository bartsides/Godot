using Godot;

public partial class projectile : CharacterBody2D
{
    private const bool debug = true;
    [Export]
    public virtual double MaxLifetime { get; set; } = 5;
    [Export]
    public virtual double Damage { get;  set; } = 10;
    [Export]
    public virtual int MaxEnemiesHit { get; set; } = 1;
    [Export]
    public virtual int MaxBounces { get; set; } = 2;

    protected bool Active { get; set; } = false;
    private Timer lifetimeTimer;

    public override void _Ready()
    {
        lifetimeTimer = new Timer(MaxLifetime, active: true);
    }

    public virtual bool HandleHitEnemy(enemy node) {
        node.Damage(Damage);

        MaxEnemiesHit--;
        if (MaxEnemiesHit > 0) return true;

        Die();
        return false;
    }

    public virtual bool HandleHitWall(TileMap node) {
        MaxBounces--;
        if (MaxBounces > 0) return true;

        Die();
        return false;
    }

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

        MoveAndSlide();

        for (var i = 0; i < GetSlideCollisionCount(); i++) {
            var collision = GetSlideCollision(i);
            var node = collision.GetCollider();
            
            if (node is enemy enemy) {
                if (!HandleHitEnemy(enemy))
                    return;
            }
            else if (node is TileMap tilemap) {
                if (!HandleHitWall(tilemap))
                    return;
            }
        }
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

    public void _OnBodyEntered(Node body) {
        if (debug) GD.Print("Projectile body entered");

        if (body is enemy enemy) {
            if (debug) GD.Print("Projectile hit enemy");

            if (!HandleHitEnemy(enemy))
                return;
        }
        else if (body is TileMap tilemap) {
            if (debug) GD.Print("Projectile hit tilemap");

            if (!HandleHitWall(tilemap))
                return;
        }
    }
}