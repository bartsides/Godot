using Godot;
using System;

public partial class enemy : CharacterBody2D, IDamagable
{
	[Export]
	protected virtual int Speed { get; set; } = 200;
	[Export]
	protected virtual int DistanceToTarget { get; set; } = 100;
	[Export]
	public virtual double Health { get; set; } = 100;
	[Export]
	public virtual double TotalHealth { get; set; } = 100;

	private TileMap tileMap;
	private NavigationAgent2D navAgent;
	private Timer findPlayerTimer = new Timer(1, active: true);
	private ProgressBar healthBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap = GetParent().GetParent().GetNodeOrNull<TileMap>("TileMap");
		healthBar = GetNode<ProgressBar>("Healthbar");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		navAgent.SetNavigationMap(tileMap.GetNavigationMap(0));
		navAgent.TargetDesiredDistance = DistanceToTarget;

		UpdateHealthbar();
		MoveTowardsPlayer();
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		if (findPlayerTimer.Process(delta)) {
			MoveTowardsPlayer();
			findPlayerTimer.Reset();
		}

		var atTarget = navAgent.DistanceToTarget() <= DistanceToTarget;
		if (!atTarget) {
			Velocity = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized() * Speed;
			MoveAndSlide();
		}

		UpdateHealthbar();
	}

	private void MoveTowardsPlayer() {
		navAgent.TargetPosition = GetParent().GetParent().GetNode<player>("Player").GlobalPosition;
	}

	public virtual void Damage(double amount) {
		Health -= amount;
		if (Health <= 0) Die();
	}

	protected virtual void UpdateHealthbar() {
		healthBar.MaxValue = TotalHealth;
		healthBar.Value = Math.Max(Health, 0);
	}

	protected virtual void Die() {
		GetParent().RemoveChild(this);
	}
}
