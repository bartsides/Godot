using Godot;
using System;

public partial class weapon_animation : AnimatedSprite2D
{
	public void AnimationFinishedHandler() {
		Animation = "default";
	}
}
