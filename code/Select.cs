using Godot;
using System;
using System.Collections.Generic;
using Array = Godot.Collections.Array;

namespace NotRimworld.code
{
    public class Select : Control
    {
        private bool _isSelecting;
        private Game _game;
        private List<Player> _selectedPlayers = new List<Player>();
        private CollisionShape2D _collisionShape;
        private Panel _panel;

        public override void _Ready()
        {
            _game = GetTree().CurrentScene as Game;
            _panel = GetNode<Panel>("Panel");

            var area = GetNode<Area2D>("Area2D");
            _collisionShape = area.GetNode<CollisionShape2D>("CollisionShape2D");
            area.Connect("area_entered", this, "Selection", new Array {true});
            area.Connect("area_exited", this, "Selection", new Array {false});

            _isSelecting = false;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion && _isSelecting)
            {
                var mouse = GetGlobalMousePosition();
                var pos = GetPoints()[0];
                DrawSelection(new[] {pos, new Vector2(mouse.x, pos.y), mouse, new Vector2(pos.x, mouse.y),});
            }

            if (@event.IsActionReleased("mouse_click") && _isSelecting)
            {
                _game.SelectionEnded(_selectedPlayers.ToArray());
                EndSelection();
            }

            if (@event.IsActionPressed("mouse_click"))
            {
                if (!_isSelecting)
                    _isSelecting = true;
                else
                    EndSelection();

                _game.SelectionEnded(System.Array.Empty<Player>());

                if (_isSelecting)
                {
                    var mouse = GetGlobalMousePosition();
                    DrawSelection(new[] {mouse, mouse, mouse, mouse});
                }
            }
        }

        private void EndSelection()
        {
            _isSelecting = false;
            DrawSelection(new[] {Vector2.Inf, Vector2.Inf, Vector2.Inf, Vector2.Inf});
            _selectedPlayers = new List<Player>();
        }

        private void DrawSelection(Vector2[] points)
        {
            (_collisionShape.Shape as ConvexPolygonShape2D).Points = points;
            
            _panel.SetBegin(new Vector2(Math.Min(points[0].x, points[2].x), Math.Min(points[0].y, points[2].y)));
            _panel.SetEnd(new Vector2(Math.Max(points[0].x, points[2].x), Math.Max(points[0].y, points[2].y)));
        }

        private Vector2[] GetPoints()
        {
            var shape = _collisionShape.Shape as ConvexPolygonShape2D;
            return shape.Points;
        }

        private void Selection(Area2D area, bool selected)
        {
            if (!_isSelecting) return;

            if (area.IsInGroup("Player"))
            {
                var player = area.GetParent<Player>();
                _game.SelectPlayer(player, selected);

                if (selected)
                    _selectedPlayers.Add(player);
                else
                    _selectedPlayers.Remove(player);
            }
        }
    }
}
