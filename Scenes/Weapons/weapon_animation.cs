using Godot;
using System;

public partial class weapon_animation : AnimatedSprite2D
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Animation != "default" && Frame == this.SpriteFrames.GetFrameCount(Animation) - 1) {
			Animation = "default";
		}
	}
}
