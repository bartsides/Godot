using System.Collections.Generic;
using Godot;

public class Player : RigidBody2D
{
	public int PlayerNumber { get; set; } = 1;
	public bool UseController = false;

	// TODO: Faster decceleration
	private float walkMaxVelocity = 200;
	private float walkAcceleration = 500;

	private bool shooting;
	private float attackTime;
	private float minAttackTime = 0.3f;

	private PackedScene gunScene;
	private Gun CurrentWeapon;
	private float gunRadius = 12;
	private Vector2 LastAimDirection = new Vector2(1, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shooting = false;
		attackTime = 0;

		SetWeapons(new List<PackedScene>{
			GD.Load<PackedScene>("res://Shoot Shoot/Gun.tscn")
		});

		GD.Print($"Player location {Position.x},{Position.y}");
	}

	private void SetWeapons(List<PackedScene> guns) {
		var weaponsNode = GetNode("Weapons");
		foreach(var gun in guns) {
			weaponsNode.AddChild(gun.Instance());
		}

		foreach(Node2D node in weaponsNode.GetChildren()) {
			if (node is Gun weapon) {
				if (CurrentWeapon == null) {
					CurrentWeapon = weapon;
					node.Visible = true;
				} else {
					node.Visible = false;
				}
			}
		}
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);


		var step = state.Step;
		var linearVelocity = state.LinearVelocity;

		var input = ListenToPlayerInput();

		ProcessWeaponPosition(input);
		ProcessAttack(input, step);

		ProcessPlayerMovement(input, state, linearVelocity, step);
	}

	private void ProcessWeaponPosition(PlayerInput input) {
		if (CurrentWeapon == null) return;

		CurrentWeapon.Position = input.AimVector * gunRadius;
		CurrentWeapon.LookAt(GetGlobalMousePosition());
		CurrentWeapon.Rotate(Mathf.Deg2Rad(90));
	}

	private void ProcessAttack(PlayerInput input, float step) {
		if (attackTime < minAttackTime)
			attackTime += step;
		if (input.Shoot && attackTime > minAttackTime)
			CallDeferred("Attack", input.AimVector);
		shooting = input.Shoot;
	}

	public void Attack(Vector2 direction) {
		attackTime = 0f;
		if (CurrentWeapon == null) return;

		var projectile = CurrentWeapon.Shoot(direction);
		if (projectile != null)
			AddCollisionExceptionWith(projectile);
	}

	private void ProcessPlayerMovement(PlayerInput input, Physics2DDirectBodyState state, Vector2 linearVelocity, float step) {
		state.LinearVelocity = ProcessPlayerDirectionalMovement(input, linearVelocity, step);

		// Determine movement direction
		var movementAngle = Mathf.Rad2Deg(input.MoveVector.Angle());
		Direction playerDirection;
		bool still = false;
		if (input.MoveVector.IsZero()) {
			playerDirection = Direction.Down;
			still = true;
		}
		else if (movementAngle >= -45 && movementAngle <= 45)
			playerDirection = Direction.Right;
		else if (movementAngle > 45 && movementAngle < 135)
			playerDirection = Direction.Down;
		else if (movementAngle < -45 && movementAngle > -135)
			playerDirection = Direction.Up;
		else
			playerDirection = Direction.Left;

		GD.Print($"{movementAngle}: {playerDirection} {still}");
	}

	private Vector2 ProcessPlayerDirectionalMovement(PlayerInput input, Vector2 linearVelocity, float step) {
		var stepAcceleration = walkAcceleration * step;

		if (input.MoveLeft && linearVelocity.x > -walkMaxVelocity ||
			input.MoveRight && linearVelocity.x < walkMaxVelocity) 
		{
			var mult = input.MoveLeft && linearVelocity.x > 0 || input.MoveRight && linearVelocity.x < 0
				? 5 
				: 1;
			linearVelocity.x += input.MoveVector.x * stepAcceleration * mult * 2;
		}

		if (input.MoveUp && linearVelocity.y > -walkMaxVelocity ||
			input.MoveDown && linearVelocity.y < walkMaxVelocity) 
		{
			var mult = input.MoveUp && linearVelocity.y > 0 || input.MoveDown && linearVelocity.y < 0 
				? 5 
				: 1;
			linearVelocity.y += input.MoveVector.y * stepAcceleration * mult;
		}

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
				aimVector = LastAimDirection;
			else
				aimVector = moveVector.Normalized();
		}

		LastAimDirection = aimVector;
		
		return new PlayerInput(moveVector, aimVector, Input.IsActionPressed("shoot"));
	}
}
