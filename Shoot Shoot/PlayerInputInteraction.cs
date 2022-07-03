using Godot;

public class PlayerInputInteraction
{
    public bool MoveLeft { get; set; }
    public bool MoveRight {get;set;}
    public bool MoveUp { get; set; }
    public bool MoveDown { get; set; }
    public bool Shoot { get; set; }
    public Vector2 MoveVector { get; set; }
    public Vector2 AimVector { get; set; }

    public PlayerInputInteraction(Vector2 movementVector, Vector2 aimVector, bool shoot) {
        MoveVector = movementVector;
        AimVector = aimVector;
        Shoot = shoot;
        
        MoveLeft = MoveVector.x < 0;
        MoveRight = MoveVector.x > 0;
        MoveUp = MoveVector.y < 0;
        MoveDown = MoveVector.y > 0;
    }
}