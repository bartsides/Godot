using System.Collections.Generic;
using Godot;

public class Player : RigidBody2D
{
	public int PlayerNumber { get; set; } = 1;
	public bool UseController = false;
	private uint ProjectileCollisionMask = Helpers.GenerateCollisionMask(true, false, true, false);
	private float moveSpeed = 900;

	private Timer attackTimer = new Timer(0.3f, active: false);

	private PackedScene gunScene;
	private Gun currentWeapon;
	private float gunRadius = 12;
	private Vector2 lastAimDirection = new Vector2(1, 0);
	private AnimatedSprite animatedSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		SetWeapons(new List<Node>{
			GD.Load<PackedScene>("res://Shoot Shoot/Weapons/Plasma Gun/PlasmaGun.tscn").Instance()
		});

		GD.Print($"Player location {Position.x},{Position.y}");
	}

	private void SetWeapons(List<Node> guns) {
		var weaponsNode = GetNode("Weapons");
		foreach(var gun in guns) {
			weaponsNode.AddChild(gun);
		}

		foreach(Node2D node in weaponsNode.GetChildren()) {
			if (node is Gun weapon) {
				if (currentWeapon == null) {
					currentWeapon = weapon;
					node.Visible = true;
				} else {
					node.Visible = false;
				}
			}
		}
	}

	public List<Gun> GetWeapons() {
		var weapons = new List<Gun>();

		if (currentWeapon != null) weapons.Add(currentWeapon);

		var weaponsNode = GetNode("Weapons");
		foreach (Node2D node in weaponsNode.GetChildren()) {
			if (node is Gun weapon && weapon != currentWeapon) {
				weapons.Add(weapon);
			}
		}

		return weapons;
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);

		var input = ListenToPlayerInput();

		ProcessWeaponPosition(input);
		ProcessAttack(input, state.Step);
		ProcessPlayerMovement(input, state, state.Step);
	}

	private void ProcessWeaponPosition(PlayerInput input) {
		if (currentWeapon == null) return;

		currentWeapon.Position = input.AimVector * gunRadius;
		// TODO: Make sure weapon is facing out
		var mousePos = GetGlobalMousePosition();
		currentWeapon.LookAt(mousePos);
		currentWeapon.SetOrientation(currentWeapon.GlobalPosition.DirectionTo(mousePos));
	}

	private void ProcessAttack(PlayerInput input, float step) {
		var canAttack = attackTimer.Process(step);
		if (input.Shoot && canAttack)
			CallDeferred("Attack", input.AimVector);
		attackTimer.Active = input.Shoot;
	}

	public void Attack(Vector2 direction) {
		attackTimer.Reset();
		if (currentWeapon == null) return;

		GD.Print($"Player layer {CollisionLayer}");
		currentWeapon.Shoot(direction, ProjectileCollisionMask);
	}

	private void ProcessPlayerMovement(PlayerInput input, Physics2DDirectBodyState state, float step) {
		state.LinearVelocity = ProcessPlayerDirectionalMovement(input, state.LinearVelocity, step);

		SetPlayerAnimation(input);
	}

	private void SetPlayerAnimation(PlayerInput input) {
		if (input.MoveVector.IsZero()) {
			animatedSprite.Animation = "idle";
			return;
		}

		var movementAngle = Mathf.Rad2Deg(input.MoveVector.Angle());
		if (movementAngle >= -45 && movementAngle <= 45)
			animatedSprite.Animation = "right";
		else if (movementAngle > 45 && movementAngle < 135)
			animatedSprite.Animation = "down";
		else if (movementAngle < -45 && movementAngle > -135)
			animatedSprite.Animation = "up";
		else
			animatedSprite.Animation = "left";
	}

	private Vector2 ProcessPlayerDirectionalMovement(PlayerInput input, Vector2 linearVelocity, float step) {
		// Check if stopping
		if (!input.MoveUp && !input.MoveRight && !input.MoveDown && !input.MoveLeft &&
			linearVelocity.Length() <= moveSpeed) {
			return Vector2.Zero;
		}

		linearVelocity += input.MoveVector * moveSpeed * step;

		return linearVelocity;
	}

	private PlayerInput ListenToPlayerInput() {
		var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");

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
		
		return new PlayerInput(moveVector, aimVector, Input.IsActionPressed("shoot"));
	}
}
