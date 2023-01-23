using Godot;
using System.Linq;

public class Enemy : RigidBody2D
{
	public virtual float AttackTime { get; set; } = 2;
	public virtual decimal Health { get; private set; } = 40;
	public virtual float Speed { get; set; } = 50;
	public virtual float MaxVelocity { get; set; } = 500;
	public virtual float BulletSpeed { get; set; } = 100;
	public virtual uint ProjectileCollisionMask { get; set; } = 251;

	private Level level;
	private AnimatedSprite animatedSprite;
    private Bullet bullet;
    private Node2D bullets;
	private Vector2[] path = new Vector2[]{};
	private Direction lastDirection = Direction.Down;

	private Timer dyingTimer = new Timer(1, active: false);
	private Timer deadTimer = new Timer(5, active: false);
	private Timer hitTimer = new Timer(2f/5f, active: false); // 2 frames at play speed of 5 FPS
	private Timer reassessMovementTimer = new Timer(1.5f);
	private Timer attackTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attackTimer = new Timer(AttackTime);
		level = GetParent().GetParent().GetParent().GetParent<Level>();
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        bullet = GetNode<Bullet>("Bullet");
        bullets = GetNode<Node2D>("Bullets");
		SetProcess(true);
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);
	}

	public override void _Process(float delta)
	{
		if (deadTimer.Active) {
			HandleDead(delta);
			return;
		}

        if (dyingTimer.Active) {
			HandleDying(delta);
        }
		else {
			var reassesMovement = reassessMovementTimer.Process(delta);
			var canAttack = attackTimer.Process(delta);

			if (hitTimer.Active) {
				HandleHit(delta);
			}
			else if (canAttack) {
				HandleAttack();
			}
			else {
				HandleMovement(delta, reassesMovement);
			}
		}

		SetAnimation();
	}

	private void HandleDead(float delta) {
		if (deadTimer.Process(delta))
			Remove();
	}

	private void HandleDying(float delta) {
		if (dyingTimer.Process(delta))
			Die();
	}

	private void HandleHit(float delta) {
		if (hitTimer.Process(delta))
			hitTimer.Reset();
	}

	private void HandleMovement(float delta, bool reassess) {
		if (DistanceToPlayer <= 50) {
			path = new Vector2[0];
			return;
		}

		if (path.Length < 1 || reassess) {
			reassessMovementTimer.Reset();
			path = level.GetSimplePath(Position, level.GetNode<Player>("Player").Position);
		}
		
		if (path != null && path.Length > 0) {
			Move(Speed * delta);
		}
	}

	private void HandleAttack() {
		attackTimer.Reset();
		if (level.Player == null) return;

		var shot = (Bullet)bullet.Duplicate();
		bullets.AddChild(shot);

		GD.Print($"Enemy attack: {Position} | {GlobalPosition}");

		var linearVelocity = Position.DirectionTo(level.Player.Position) * BulletSpeed;
		shot.Fire(null, level.Player.Position, linearVelocity, ProjectileCollisionMask);
	}

	private float DistanceToPlayer {
		get {
			var player = level.Player;
			if (player == null) return 0;
			return GlobalPosition.DistanceTo(player.GlobalPosition);
		}
	}

    private void SetAnimation() {
        animatedSprite.FlipH = false;

        if (dyingTimer.Active) {
            animatedSprite.Animation = "Die";
            return;
        }
		
		// TODO: Utilize Damage Right, Damage Up, Damage Down animations

		if (LinearVelocity.IsZero()) {
			animatedSprite.Animation = hitTimer.Active ? "Damage Down" : "Run Down";
		}

		var direction = LinearVelocity.Length() > 1 ? LinearVelocity.GetDirection() : lastDirection;
		lastDirection = direction;

		if (direction == Direction.Right) {
			animatedSprite.Animation = hitTimer.Active ? "Damage Right" : "Run Side";
        }
		else if (direction == Direction.Down) { 
			animatedSprite.Animation = hitTimer.Active ? "Damage Down" : "Run Down";
        }
		else if (direction == Direction.Up) {
			animatedSprite.Animation = hitTimer.Active ? "Damage Up" : "Damage Up";
        }
		else if (direction == Direction.Left) {
			animatedSprite.Animation = hitTimer.Active ? "Damage Right" : "Run Side";
            animatedSprite.FlipH = true;
        }
    }

	private void Move(float speed) {
		var dist = Position.DistanceTo(path[0]);
		if (dist < 10)
			path = path.Skip(1).Take(path.Length - 1).ToArray();
		else 
			Position = Position.LinearInterpolate(path[0], speed / dist);
	}

	public void Hit(decimal damage) {
		if (deadTimer.Active || dyingTimer.Active) return;

		Health -= damage;
		if (Health <= 0)
			StartDying();
		
		hitTimer.Active = true;
		hitTimer.Reset();
		animatedSprite.Frame = 0; // reset animation on hit
	}

	private void StartDying() {
        dyingTimer.Active = true;
		LinearVelocity = Vector2.Zero;
	}

	private void Die() {
		deadTimer.Active = true;
		animatedSprite.Playing = false;
	}

    private void Remove() {
		((Room)GetParent().GetParent()).EnemyKilled(this);
    }
}
