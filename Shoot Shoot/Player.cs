using Godot;

public class Player : RigidBody2D
{
    private float walkMaxVelocity = 200f;
    private float walkAcceleration = 800f;
    private float walkDecceleration = 800f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        base._IntegrateForces(state);

        var step = state.Step;
        var linearVelocity = state.LinearVelocity;

        var interaction = ListenToPlayerInput();

        GD.Print($"{interaction.MoveLeft} {interaction.MoveUp} {interaction.MoveRight} {interaction.MoveDown}");

        linearVelocity = ProcessPlayerMovement(interaction, linearVelocity, step);

        state.LinearVelocity = linearVelocity;
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
            Input.IsActionPressed("move_up"), Input.IsActionPressed("move_down"));
    }
}
