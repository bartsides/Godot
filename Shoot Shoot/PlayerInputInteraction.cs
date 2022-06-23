public class PlayerInputInteraction
{
    public bool MoveLeft { get; set; }
    public bool MoveRight {get;set;}
    public bool MoveUp { get; set; }
    public bool MoveDown { get; set; }

    public PlayerInputInteraction(bool moveLeft, bool moveRight, bool moveUp, bool moveDown) {
        MoveLeft = moveLeft;
        MoveRight = moveRight;
        MoveUp = moveUp;
        MoveDown = moveDown;
    }
}