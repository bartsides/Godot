namespace MyGodotGame;

public partial class AnimationPlayer : Godot.AnimationPlayer
{
	public void _on_animation_finished(string animationName) {
		if (animationName != "RESET");
			Play("RESET");
	}
}
