using System.Collections.Generic;
using System;
using Godot;

public partial class player : CharacterBody2D
{
	[Export]
	public float Friction { get; set; } = 50;
	[Export]
	public float Acceleration { get; set; } = 20;
	[Export]
	public float Speed { get; set; } = 200;

	public int PlayerNumber { get; set; } = 1;
	public bool UseController = false;
	private uint ProjectileCollisionLayer = Helpers.GenerateCollisionMask(playerProjectiles: true);
	private uint ProjectileCollisionMask = Helpers.GenerateCollisionMask(walls: true, enemies: true);

	private Timer attackTimer = new Timer(0.3f, active: false);

	private PackedScene gunScene;
	private weapon currentWeapon;
	private Marker2D weaponLeftPosition;
	private Marker2D weaponRightPosition;
	private Vector2 lastAimDirection = new Vector2(1, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionLayer = Helpers.GenerateCollisionMask(false, true, false, false, false);
		CollisionMask = Helpers.GenerateCollisionMask(true, false, true, false, true);
		weaponLeftPosition = GetNode<Marker2D>("WeaponLeftPosition");
		weaponRightPosition = GetNode<Marker2D>("WeaponRightPosition");

		SetWeapons(new List<Node>{
			//GD.Load<PackedScene>("res://Scenes/Weapons/Plasma Gun/plasma_gun.tscn").Instantiate()
			GD.Load<PackedScene>("res://Scenes/Weapons/BigPistol.tscn").Instantiate()
		});
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

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		var step = (float) delta;

		var input = ListenToPlayerInput();

		ProcessWeaponPosition(input);
		ProcessAttack(input, step);
		ProcessPlayerMovement(input, step);
	}

	private void ProcessWeaponPosition(player_input input) {
		if (currentWeapon == null) {
			GD.Print("No current weapon");
			return;
		}
		var weaponMarker = input.AimVector.IsFacingRight() ? weaponRightPosition : weaponLeftPosition;
		currentWeapon.Position = weaponMarker.Position;
		currentWeapon.LookAt(GetGlobalMousePosition());
		currentWeapon.SetOrientation();
	}

	private void ProcessAttack(player_input input, float step) {
		var canAttack = attackTimer.Process(step);
		if (input.Shoot && canAttack)
			CallDeferred("Attack");
		attackTimer.Active = input.Shoot;
	}

	public void Attack() {
		attackTimer.Reset();
		if (currentWeapon == null) return;
		currentWeapon.Shoot(ProjectileCollisionLayer, ProjectileCollisionMask);
	}

	private void ProcessPlayerMovement(player_input input, float step) {
		ProcessPlayerDirectionalMovement(input, step);
		MoveAndSlide();
		//SetPlayerAnimation(input);
	}

	private void SetPlayerAnimation(player_input input) {
		if (input.MoveVector.IsZero()) {
			//animatedSprite.Animation = "idle";
			return;
		}

		// var movementAngle = Mathf.RadToDeg(input.MoveVector.Angle());
		// if (movementAngle >= -45 && movementAngle <= 45)
		// 	animatedSprite.Animation = "right";
		// else if (movementAngle > 45 && movementAngle < 135)
		// 	animatedSprite.Animation = "down";
		// else if (movementAngle < -45 && movementAngle > -135)
		// 	animatedSprite.Animation = "up";
		// else
		// 	animatedSprite.Animation = "left";
	}

	private void ProcessPlayerDirectionalMovement(player_input input, float step) {
		// Check if stopping
		if (!input.MoveUp && !input.MoveRight && !input.MoveDown && !input.MoveLeft) {
			GD.Print("slowing down");
			// Apply friction
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
			return;
		}

		GD.Print("moving");
		Velocity = Velocity.MoveToward(input.MoveVector * Speed, Acceleration);
	}

	private player_input ListenToPlayerInput() {
		var moveVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector2 aimVector;
		if (UseController) {
			aimVector = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down");
		} else {
			aimVector = GlobalPosition.DirectionTo(GetGlobalMousePosition());
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
