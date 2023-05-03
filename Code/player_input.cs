namespace MyGodotGame;

public class player_input
{
    public bool MoveLeft { get; set; }
    public bool MoveRight {get;set;}
    public bool MoveUp { get; set; }
    public bool MoveDown { get; set; }
    public bool Shoot { get; set; }
    public Vector2 MoveVector { get; set; }
    public Vector2 AimVector { get; set; }

    public player_input(Vector2 movementVector, Vector2 aimVector, bool shoot) {
        MoveVector = movementVector;
        AimVector = aimVector;
        Shoot = shoot;
        
        MoveLeft = MoveVector.X < 0;
        MoveRight = MoveVector.X > 0;
        MoveUp = MoveVector.Y < 0;
        MoveDown = MoveVector.Y > 0;
    }
}