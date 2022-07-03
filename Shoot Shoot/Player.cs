using System.Collections.Generic;
using Godot;

public class Player : RigidBody2D
{
	public int PlayerNumber { get; set; } = 1;
	public bool UseController = true;

	// TODO: Faster decceleration
	private float walkMaxVelocity = 200;
	private float walkAcceleration = 500;

	private bool shooting;
	private float attackTime;
	private float minAttackTime = 0.3f;

	private PackedScene gunScene;
	private Gun CurrentWeapon;
	private float gunRadius = 12;
	private Vector2 LastNonZeroAimDirection = new Vector2(1, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shooting = false;
		attackTime = 0;

		SetWeapons(new List<PackedScene>{
			GD.Load<PackedScene>("res://Shoot Shoot/Gun.tscn")
		});
	}

	private void SetWeapons(List<PackedScene> guns) {
		var weaponsNode = GetNode("Weapons");
		foreach(var gun in guns) {
			weaponsNode.AddChild(gun.Instance());
		}

		foreach(Node2D node in weaponsNode.GetChildren()) {
			if (node is Gun weapon) {
				if (CurrentWeapon == null) {
					GD.Print("Setting current weapon");
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

		var interaction = ListenToPlayerInput();

		ProcessWeaponPosition(interaction);
		ProcessAttack(interaction, step);

		linearVelocity = ProcessPlayerMovement(interaction, linearVelocity, step);

		state.LinearVelocity = linearVelocity;
	}

	private void ProcessWeaponPosition(PlayerInputInteraction interaction) {
		if (CurrentWeapon == null) return;

		// var position = Position;
		// var mouse = GetGlobalMousePosition();

		CurrentWeapon.Position = interaction.AimVector * gunRadius;
		CurrentWeapon.LookAt(interaction.AimVector);
		
		// CurrentWeapon.Position = Position.DirectionTo(mouse) * gunRadius;
		// CurrentWeapon.LookAt(mouse);
		// CurrentWeapon.Rotate(Mathf.Deg2Rad(90));
	}

	private void ProcessAttack(PlayerInputInteraction interaction, float step) {
		if (attackTime < minAttackTime)
			attackTime += step;
		if (interaction.Shoot && attackTime > minAttackTime)
			CallDeferred("Attack", interaction.AimVector);
		shooting = interaction.Shoot;
	}

	public void Attack(Vector2 direction) {
		attackTime = 0f;
		if (CurrentWeapon == null) return;

		var projectile = CurrentWeapon.Shoot(direction);
		if (projectile != null)
			AddCollisionExceptionWith(projectile);
	}

	private Vector2 ProcessPlayerMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
		linearVelocity = ProcessPlayerDirectionalMovement(interaction, linearVelocity, step);

		return linearVelocity;
	}

	private Vector2 ProcessPlayerDirectionalMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
		var stepAcceleration = walkAcceleration * step;

		if (interaction.MoveLeft && linearVelocity.x > -walkMaxVelocity ||
			interaction.MoveRight && linearVelocity.x < walkMaxVelocity) 
		{
			var mult = interaction.MoveLeft && linearVelocity.x > 0 || interaction.MoveRight && linearVelocity.x < 0
				? 5 : 1;
			linearVelocity.x += interaction.MoveVector.x * stepAcceleration * mult * 2;
		}

		if (interaction.MoveUp && linearVelocity.y > -walkMaxVelocity ||
			interaction.MoveDown && linearVelocity.y < walkMaxVelocity) 
		{
			var mult = interaction.MoveUp && linearVelocity.y > 0 || interaction.MoveDown && linearVelocity.y < 0 
				? 5 : 1;
			linearVelocity.y += interaction.MoveVector.y * stepAcceleration * mult;
		}

		return linearVelocity;
	}

	private PlayerInputInteraction ListenToPlayerInput() {
		// return new PlayerInputInteraction(Input.IsActionPressed("move_left"), Input.IsActionPressed("move_right"), 
		// 	Input.IsActionPressed("move_up"), Input.IsActionPressed("move_down"), Input.IsActionPressed("shoot"));
		Vector2 aimVector;
		if (UseController) {
			aimVector = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down");
		} else {
			aimVector = Position.DirectionTo(GetGlobalMousePosition());
		}
		aimVector = aimVector.Normalized();

		var tol = 0.00001f;
		if (aimVector.x > -tol && aimVector.x < tol && aimVector.y > -tol && aimVector.y < tol) {
			aimVector = LastNonZeroAimDirection;
		} else {
			LastNonZeroAimDirection = aimVector;
		}
		
		return new PlayerInputInteraction(
			Input.GetVector("move_left", "move_right", "move_up", "move_down"),
			aimVector, Input.IsActionPressed("shoot")
		);
	}
}
