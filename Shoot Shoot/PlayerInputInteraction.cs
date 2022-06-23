public class PlayerInputInteraction
{
    public bool MoveLeft { get; set; }
    public bool MoveRight {get;set;}
    public bool MoveUp { get; set; }
    public bool MoveDown { get; set; }
    public bool Shoot { get; set; }

    public PlayerInputInteraction(bool moveLeft, bool moveRight, bool moveUp, bool moveDown, bool shoot) {
        MoveLeft = moveLeft;
        MoveRight = moveRight;
        MoveUp = moveUp;
        MoveDown = moveDown;
        Shoot = shoot;
    }
}