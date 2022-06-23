using Godot;

public class Player : RigidBody2D
{
    private float walkMaxVelocity = 200f;
    private float walkAcceleration = 800f;
    private float walkDecceleration = 800f;

    private bool shooting;
    private float shootTime;
    private float minShootTime = 1f;

    private PackedScene bulletScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        shooting = false;
        shootTime = 0;

        // TODO: Have weapon handle bullet specifics
        bulletScene = GD.Load<PackedScene>("res://Shoot Shoot/Bullet.tscn");
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        base._IntegrateForces(state);

        var step = state.Step;
        var linearVelocity = state.LinearVelocity;

        var interaction = ListenToPlayerInput();

        ProcessShooting(interaction, step);

        linearVelocity = ProcessPlayerMovement(interaction, linearVelocity, step);

        state.LinearVelocity = linearVelocity;
    }

    private void ProcessShooting(PlayerInputInteraction interaction, float step) {
        if (shootTime < minShootTime)
            shootTime += step;
        if (interaction.Shoot && shootTime > minShootTime)
            CallDeferred("ShootBullet");
        shooting = interaction.Shoot;
    }

    public void ShootBullet() {
        shootTime = 0f;
        RigidBody2D bullet = bulletScene.Instance() as RigidBody2D;
        
        //Position2D bulletShoot = GetNode("BulletShoot") as Position2D;
        //Vector2 bulletPosition = Position + bulletShoot.Position * (new Vector2(side, 1.0f));

        bullet.Position = Position;
        GetParent().AddChild(bullet);

        bullet.LinearVelocity = Position.DirectionTo(GetGlobalMousePosition()) * 800f;

        // Particles2D particles = GetNode("Sprite/Smoke") as Particles2D;
        // particles.Restart();
        // AudioStreamPlayer2D soundShoot = GetNode("SoundShoot") as AudioStreamPlayer2D;
        // soundShoot.Play();

        AddCollisionExceptionWith(bullet);
    }

    private Vector2 ProcessPlayerMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
        linearVelocity = ProcessPlayerDirectionalMovement(interaction, linearVelocity, step);

        return linearVelocity;
    }

    private Vector2 ProcessPlayerDirectionalMovement(PlayerInputInteraction interaction, Vector2 linearVelocity, float step) {
        if (interaction.MoveLeft && !interaction.MoveRight) {
            if (linearVelocity.x > -walkMaxVelocity) {
                linearVelocity.x -= walkAcceleration * step;
            }
        }
        else if (interaction.MoveRight && !interaction.MoveLeft) {
            if (linearVelocity.x < walkMaxVelocity) {
                linearVelocity.x += walkAcceleration * step;
            }
        }

        if (interaction.MoveUp && !interaction.MoveDown) {
            if (linearVelocity.y > -walkMaxVelocity) {
                linearVelocity.y -= walkAcceleration * step;
            }
        }
        else if (interaction.MoveDown && !interaction.MoveUp) {
            if (linearVelocity.y < walkMaxVelocity) {
                linearVelocity.y += walkAcceleration * step;
            }
        }

        return linearVelocity;
    }

    private PlayerInputInteraction ListenToPlayerInput() {
        return new PlayerInputInteraction(Input.IsActionPressed("move_left"), Input.IsActionPressed("move_right"), 
            Input.IsActionPressed("move_up"), Input.IsActionPressed("move_down"), Input.IsActionPressed("shoot"));
    }
}
