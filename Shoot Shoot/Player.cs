using System.Collections.Generic;
using Godot;

public class Player : RigidBody2D
{
	// TODO: Faster decceleration
	private float walkMaxVelocity = 200;
	private float walkAcceleration = 500;

	private bool shooting;
	private float attackTime;
	private float minAttackTime = 0.3f;

	private PackedScene gunScene;
	private Gun CurrentWeapon;
	private float gunRadius = 12;

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

		ProcessWeaponPosition();

		var step = state.Step;
		var linearVelocity = state.LinearVelocity;

		var interaction = ListenToPlayerInput();

		ProcessAttack(interaction, step);

		linearVelocity = ProcessPlayerMovement(interaction, linearVelocity, step);

		state.LinearVelocity = linearVelocity;
	}

	private void ProcessWeaponPosition() {
		if (CurrentWeapon == null) return;

		var position = Position;
		var mouse = GetGlobalMousePosition();
		
		CurrentWeapon.Position = Position.DirectionTo(mouse) * gunRadius;
		CurrentWeapon.LookAt(mouse);
		CurrentWeapon.Rotate(Mathf.Deg2Rad(90));
	}

	private void ProcessAttack(PlayerInputInteraction interaction, float step) {
		if (attackTime < minAttackTime)
			attackTime += step;
		if (interaction.Shoot && attackTime > minAttackTime)
			CallDeferred("Attack");
		shooting = interaction.Shoot;
	}

	public void Attack() {
		attackTime = 0f;
		if (CurrentWeapon == null) return;

		var projectile = CurrentWeapon.Shoot();
		if (projectile != null)
			AddCollisionExceptionWith(projectile);
	}

	private Vector2 ProcessPlayerMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
		linearVelocity = ProcessPlayerDirectionalMovement(interaction, linearVelocity, step);

		return linearVelocity;
	}

	private Vector2 ProcessPlayerDirectionalMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
		if (interaction.MoveLeft && !interaction.MoveRight) {
			if (linearVelocity.x > -walkMaxVelocity) {
				linearVelocity.x -= walkAcceleration * step * (linearVelocity.x > 0 ? 5 : 1);
			}
		}
		else if (interaction.MoveRight && !interaction.MoveLeft) {
			if (linearVelocity.x < walkMaxVelocity) {
				linearVelocity.x += walkAcceleration * step * (linearVelocity.x < 0 ? 5 : 1);
			}
		}

		if (interaction.MoveUp && !interaction.MoveDown) {
			if (linearVelocity.y > -walkMaxVelocity) {
				linearVelocity.y -= walkAcceleration * step * (linearVelocity.y > 0 ? 5 : 1);
			}
		}
		else if (interaction.MoveDown && !interaction.MoveUp) {
			if (linearVelocity.y < walkMaxVelocity) {
				linearVelocity.y += walkAcceleration * step * (linearVelocity.y < 0 ? 5 : 1);
			}
		}

		return linearVelocity;
	}

	private PlayerInputInteraction ListenToPlayerInput() {
		return new PlayerInputInteraction(Input.IsActionPressed("move_left"), Input.IsActionPressed("move_right"), 
			Input.IsActionPressed("move_up"), Input.IsActionPressed("move_down"), Input.IsActionPressed("shoot"));
	}
}
