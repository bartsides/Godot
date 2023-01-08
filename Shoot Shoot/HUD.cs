using Godot;
using System;

public class HUD : CanvasLayer
{
    private bool debug = true;

    private AnimatedSprite gun1;
    private Level level;

    private Timer RefreshTimer = new Timer(0.1f);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        level = GetParent().GetNode<Level>("Level");
        Refresh();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (RefreshTimer.Process(delta)) {
            Refresh();
            RefreshTimer.Reset();
        }
    }

    private void Refresh() {
        if (debug) GD.Print("refresh HUD");
        var weaponsNode = GetNode<Node2D>("Weapons");
        // Clear weapons
        foreach (Node2D weapon in weaponsNode.GetChildren())
            weapon.QueueFree();
        
        var player = level?.Player;
        if (player == null) return;

        var i = 0;
        var weapons = player.GetWeapons();
        if (debug) GD.Print($"Weapons: {weapons.Count}");
        foreach (var weapon in weapons) {

            var animatedSprite = (AnimatedSprite) weapon.GetNode<AnimatedSprite>("AnimatedSprite")?.Duplicate();
            if (animatedSprite == null) continue;
            
            if (debug) GD.Print($"Weapon {i}");
            i++;
            if (i > 5) break;
            var positionName = $"Weapon{1}Position2D";

            var weaponPosition = GetNode<Position2D>(positionName);
            if (weaponPosition == null) continue;

            animatedSprite.Position = weaponPosition.Position;
            animatedSprite.FlipV = false;
            weaponsNode.AddChild(animatedSprite);

            if (debug) GD.Print("Weapon added");
        }
    }
}
