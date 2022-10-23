using Godot;

public class GunAnimation : AnimatedSprite
{
    public void _on_AnimatedSprite_animation_finished() {
        if (Animation != "default")
            Animation = "default";
    }
}
