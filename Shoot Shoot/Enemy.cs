using Godot;
using System.Linq;

public class Enemy : RigidBody2D
{
	public decimal Health { get; private set; }
	private Level level;
	private AnimatedSprite animatedSprite;
	private Vector2[] path = new Vector2[]{};
	private int speed = 100;
	private int maxVelocity = 500;
	private float timeSinceLastReassess = 0;
	private float maxTimeSinceLastReassess = 1.5f;
	private Direction lastDirection = Direction.Down;

	private bool dying = false;
	private float deathAnimationTime = 0;
	private float deathAnimationMaxTime = 1;

	private bool dead = false;
	private float deadBodyTime = 0;
	private float deadBodyMaxTime = 5;

	private bool hit = false;
	private float hitTime = 0;
	private float hitMaxTime = 2f/5f; // 2 frames at play speed of 5 FPS

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		level = GetParent().GetParent().GetParent().GetParent<Level>();
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Health = 40;
		SetProcess(true);
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);
	}

	public override void _Process(float delta)
	{
		if (dead) {
			HandleDead(delta);
			return;
		}

		if (dying) {
			HandleDying(delta);
		}
		else {
			HandleMovement(delta);

			if (hit)
				HandleHit(delta);
		}

		SetAnimation();
	}

	private void HandleDead(float delta) {
		deadBodyTime += delta;
		if (deadBodyTime > deadBodyMaxTime)
			Remove();
	}

	private void HandleDying(float delta) {
		deathAnimationTime += delta;
		
		if (deathAnimationTime > deathAnimationMaxTime)
			Die();
	}

	private void HandleHit(float delta) {
		hitTime += delta;
		if (hitTime > hitMaxTime) {
			hit = false;
		}
	}

	private void HandleMovement(float delta) {
		timeSinceLastReassess += delta;

		if (DistanceToPlayer <= 50) {
			path = new Vector2[0];
			return;
		}

		if (path.Length < 1 || timeSinceLastReassess > maxTimeSinceLastReassess) {
			timeSinceLastReassess = 0;
			path = level.GetSimplePath(Position, level.GetNode<Player>("Player").Position);
		}
		
		if (path != null && path.Length > 0) {
			Move(speed * delta);
		}
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

		if (dying) {
			animatedSprite.Animation = "Die";
			return;
		}
		
		// TODO: Utilize Damage Right, Damage Up, Damage Down animations

		if (LinearVelocity.IsZero()) {
			animatedSprite.Animation = hit ? "Damage Down" : "Run Down";
		}

		var direction = LinearVelocity.Length() > 1 ? LinearVelocity.GetDirection() : lastDirection;
		lastDirection = direction;

		if (direction == Direction.Right) {
			animatedSprite.Animation = hit ? "Damage Right" : "Run Side";
		}
		else if (direction == Direction.Down) { 
			animatedSprite.Animation = hit ? "Damage Down" : "Run Down";
		}
		else if (direction == Direction.Up) {
			animatedSprite.Animation = hit ? "Damage Up" : "Damage Up";
		}
		else if (direction == Direction.Left) {
			animatedSprite.Animation = hit ? "Damage Right" : "Run Side";
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
		if (dead || dying) return;

		Health -= damage;
		if (Health <= 0)
			StartDying();
		
		hit = true;
		hitTime = 0;
		animatedSprite.Frame = 0; // reset animation on hit
	}

	private void StartDying() {
		dying = true;
		LinearVelocity = Vector2.Zero;
	}

	private void Die() {
		dead = true;
		animatedSprite.Playing = false;
	}

	private void Remove() {
		((Room)GetParent().GetParent()).EnemyKilled(this);
	}
}
