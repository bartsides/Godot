using System.Collections.Generic;
using Godot;

public partial class player : RigidBody2D
{
	public int PlayerNumber { get; set; } = 1;
	public bool UseController = false;
	private uint ProjectileCollisionMask = Helpers.GenerateCollisionMask(true, false, true, false, false);
	private float moveSpeed = 900;

	private Timer attackTimer = new Timer(0.3f, active: false);

	private PackedScene gunScene;
	private weapon currentWeapon;
	private float gunRadius = 20;
	private Vector2 lastAimDirection = new Vector2(1, 0);
	private AnimatedSprite2D animatedSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionLayer = Helpers.GenerateCollisionMask(false, true, false, false, false);
		CollisionMask = Helpers.GenerateCollisionMask(true, false, true, false, true);
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		SetWeapons(new List<Node>{
			GD.Load<PackedScene>("res://Scenes/Weapons/Plasma Gun/plasma_gun.tscn").Instantiate()
		});

		GD.Print($"Player location {Position.X},{Position.Y}");
	}

	private void SetWeapons(List<Node> weapons) {
		var weaponsNode = GetNode<Node2D>("Weapons");
		foreach(var weapon in weapons) {
			weaponsNode.AddChild(weapon);
		}

		foreach(Node2D node in weaponsNode.GetChildren()) {
			if (node is weapon weapon) {
				if (currentWeapon == null) {
					currentWeapon = weapon;
					node.Visible = true;
				} else {
					node.Visible = false;
				}
			}
		}
	}

	public List<weapon> GetWeapons() {
		var weapons = new List<weapon>();

		if (currentWeapon != null) weapons.Add(currentWeapon);

		var weaponsNode = GetNode("Weapons");
		foreach (Node2D node in weaponsNode.GetChildren()) {
			if (node is weapon weapon && weapon != currentWeapon) {
				weapons.Add(weapon);
			}
		}

		return weapons;
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		base._IntegrateForces(state);

		var input = ListenToPlayerInput();

		ProcessWeaponPosition(input);
		ProcessAttack(input, state.Step);
		ProcessPlayerMovement(input, state, state.Step);
	}

	private void ProcessWeaponPosition(player_input input) {
		if (currentWeapon == null) {
			GD.Print("No current weapon");
			return;
		}

		currentWeapon.Position = input.AimVector * gunRadius;
		currentWeapon.LookAt(currentWeapon.Position * 2);
		currentWeapon.SetOrientation();
	}

	private void ProcessAttack(player_input input, float step) {
		var canAttack = attackTimer.Process(step);
		if (input.Shoot && canAttack)
			CallDeferred("Attack", input.AimVector);
		attackTimer.Active = input.Shoot;
	}

	public void Attack(Vector2 direction) {
		attackTimer.Reset();
		if (currentWeapon == null) return;

		currentWeapon.Shoot(direction, ProjectileCollisionMask);
	}

	private void ProcessPlayerMovement(player_input input, PhysicsDirectBodyState2D state, float step) {
		state.LinearVelocity = ProcessPlayerDirectionalMovement(input, state.LinearVelocity, step);

		SetPlayerAnimation(input);
	}

	private void SetPlayerAnimation(player_input input) {
		if (input.MoveVector.IsZero()) {
			animatedSprite.Animation = "idle";
			return;
		}

		var movementAngle = Mathf.RadToDeg(input.MoveVector.Angle());
		if (movementAngle >= -45 && movementAngle <= 45)
			animatedSprite.Animation = "right";
		else if (movementAngle > 45 && movementAngle < 135)
			animatedSprite.Animation = "down";
		else if (movementAngle < -45 && movementAngle > -135)
			animatedSprite.Animation = "up";
		else
			animatedSprite.Animation = "left";
	}

	private Vector2 ProcessPlayerDirectionalMovement(player_input input, Vector2 linearVelocity, float step) {
		// Check if stopping
		if (!input.MoveUp && !input.MoveRight && !input.MoveDown && !input.MoveLeft &&
			linearVelocity.Length() <= moveSpeed) {
			return Vector2.Zero;
		}

		linearVelocity += input.MoveVector * moveSpeed * step;

		return linearVelocity;
	}

	private player_input ListenToPlayerInput() {
		var moveVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector2 aimVector;
		if (UseController) {
			aimVector = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down");
		} else {
			aimVector = Position.DirectionTo(GetGlobalMousePosition());
		}
		aimVector = aimVector.Normalized();
		
		if (aimVector.IsZero()) {
			if (moveVector.IsZero())
				aimVector = lastAimDirection;
			else
				aimVector = moveVector.Normalized();
		}

		lastAimDirection = aimVector;
		
		return new player_input(moveVector, aimVector, Input.IsActionPressed("shoot"));
	}
}
