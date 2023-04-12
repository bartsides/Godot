using System;
using Godot;

namespace NotRimworld.code
{
    public class Camera : Camera2D
    {
        private Vector2 _momentum = Vector2.Zero;
        private const float MaxAcceleration = 400f;
        private const float Acceleration = 100f;
        private const float Deceleration = 1000f;

        private bool _up;
        private bool _down;
        private bool _left;
        private bool _right;
    
        public override void _Process(float delta)
        {
            base._Process(delta);
            HandleMovement(delta);
        }

        private void HandleMovement(float delta)
        {
            Position += _momentum * delta;

            // If keys are pressed, increase momentum
            if (_up)
                Move(new Vector2(0, -1));
            if (_down)
                Move(new Vector2(0, 1));
            if (_left)
                Move(new Vector2(-1, 0));
            if (_right)
                Move(new Vector2(1, 0));

            // Slow momentum if no keys are pressed
            if (!_left && !_right)
            {
                var change = Deceleration * delta;
                if (_momentum.x > 0)
                    _momentum.x = Math.Max(0, _momentum.x - change);
                else if (_momentum.x < 0)
                    _momentum.x = Math.Min(0, _momentum.x + change);
            }

            if (!_up && !_down)
            {
                var change = Deceleration * delta;
                if (_momentum.y > 0)
                    _momentum.y = Math.Max(0, _momentum.y - change);
                else if (_momentum.y < 0)
                    _momentum.y = Math.Min(0, _momentum.y + change);
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsAction("mouse_wheel_up"))
                ChangeZoom(-@event.GetActionStrength("mouse_wheel_up"));
            if (@event.IsAction("mouse_wheel_down"))
                ChangeZoom(@event.GetActionStrength("mouse_wheel_down"));

            if (@event.IsActionPressed("move_up"))
                _up = true;
            else if (@event.IsActionReleased("move_up"))
                _up = false;

            if (@event.IsActionPressed("move_down"))
                _down = true;
            else if (@event.IsActionReleased("move_down"))
                _down = false;

            if (@event.IsActionPressed("move_left"))
                _left = true;
            else if (@event.IsActionReleased("move_left"))
                _left = false;

            if (@event.IsActionPressed("move_right"))
                _right = true;
            else if (@event.IsActionReleased("move_right"))
                _right = false;
        }

        private void Move(Vector2 direction)
        {
            // TODO: Adjust movement by zoom level?
            _momentum.x = Math.Max(-MaxAcceleration,
                Math.Min(MaxAcceleration, _momentum.x + direction.x * Acceleration));
            _momentum.y = Math.Max(-MaxAcceleration,
                Math.Min(MaxAcceleration, _momentum.y + direction.y * Acceleration));
        }

        private void ChangeZoom(float amount)
        {
            var change = new Vector2(amount, amount);
            var newZoom = Zoom + change;

            // Zoom cannot be 0
            newZoom.x = Math.Max(0.00000001f, newZoom.x);
            newZoom.y = Math.Max(0.00000001f, newZoom.y);
        
            Zoom = newZoom;
        }
    }
}
