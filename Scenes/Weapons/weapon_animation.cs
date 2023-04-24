using Godot;
using System;

public partial class weapon_animation : AnimatedSprite2D
{
	public override void _Ready() {
		this.AnimationFinished += () => AnimationFinishedHandler();
	}

	public void AnimationFinishedHandler() {
		Animation = "default";
	}
}
