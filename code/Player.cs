using Godot;
using NotRimworld.Directives;
using NotRimworld.Enums;
using NotRimworld.Needs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotRimworld.code
{
    public class Player : Node2D
    {
        public PlayerState State { get; private set; }

        public IDirective Directive { get; private set; }
        private bool _movingToDirective = false;

        private List<INeed> _needs;

        private const float Mass = 5f;
        private const float ArriveDistance = 10f;
        [Export]
        private float _speed = 200f;

        private List<Vector2> _path;
        private Vector2 _targetPointWorld;
        private Vector2 _targetPosition;
        private Vector2 _velocity;

        private Game _game;
        private Navigation2D _nav;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _path = new List<Vector2>();
            _targetPosition = new Vector2();
            _targetPointWorld = new Vector2();
            _velocity = new Vector2();

            _needs = new List<INeed> {new VapeNeed()};

            _game = GetParent().GetParent().GetParent<Game>();
            _nav = _game.GetNode<Navigation2D>("Nav");

            // Create separate material
            var sprite = GetNode<AnimatedSprite>("AnimatedSprite");
            sprite.Material = sprite.Material.Duplicate() as ShaderMaterial;

            ChangeState(PlayerState.Idle);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            //IncrementNeeds(delta);
            //HandleDirectives(delta);
            HandleMove(delta);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            //if (@event.IsActionPressed("click"))
            //{
            //	ClearDirective();
            //	SetTargetPosition(GetGlobalMousePosition());
            //}
        }

        private void SetTargetPosition(Vector2 position)
        {
            _targetPosition = position;
            ChangeState(PlayerState.Follow);
        }

        private void ChangeState(PlayerState state)
        {
            if (state == PlayerState.Follow)
            {
                //_path = _game.GetPath(Position, _targetPosition);
                //if (_path == null || _path.Count <= 1)
                //{
                //    ChangeState(PlayerState.Idle);
                //    return;
                //}

                //_targetPointWorld = _path[1];
            }

            State = state;
        }

        private void IncrementNeeds(float delta)
        {
            foreach (var need in _needs)
            {
                need.Handle(this, delta);
            }
        }

        public INeed GetNeed(string type)
        {
            return _needs.FirstOrDefault(need => need.GetType().ToString() == type);
        }

        private void HandleDirectives(float delta)
        {
            if (Directive != null)
                Directive.Handle(this, delta);
            else
                SetNextDirective();
        }

        private void SetNextDirective()
        {
            var nextNeed = _needs.Where(need => need.Value >= need.Minimum).OrderBy(need => need.Value).FirstOrDefault();
            if (nextNeed != null)
                Directive = nextNeed.GetDirective();
        }

        public void ClearDirective()
        {
            Directive = null;
            _movingToDirective = false;
        }

        private void HandleMove(float delta)
        {
            GD.Print("handle move");
            if (_path.Count < 2)
            {
                GD.Print("return");
                SetProcess(false);
                return;
            }

            var nextPoint = _path[1];
            var distance = Position.DistanceTo(nextPoint);
            if (distance > 5)
            {
                Position = Position.LinearInterpolate(nextPoint, (200 * delta) / distance);
                LookAt(_path[1]);
            }
            else
            {
                _path.RemoveAt(1);
            }

            _path[0] = Position;
            SetLinePoints(_path);
        }

        private void SetLinePoints(List<Vector2> points)
        {
            // TODO: Set line points
        }

        public void MoveTo(Vector2 destination)
        {
            SetProcess(true);
            GD.Print($"{Position} -> {destination}");
            _path = _nav.GetSimplePath(Position, destination).ToList();
            _path.Insert(0, Position);
            GD.Print($"Points: " + _path.Count);
            SetLinePoints(_path);
        }

        private bool ArrivedTo(Vector2 destination)
        {
            return Position.DistanceTo(destination) < ArriveDistance;
        }

        public void GoToClosest(int id)
        {
            if (_game == null) throw new Exception("tilemap not found");
            var cells = _game.GetUsedCellsById(id);
            if (cells.Count == 0) return;

            var characterPosition = _game.WorldToMap(_game.ToLocal(GlobalPosition));
            var closestCell = Vector2.Inf;
            var shortestPath = new List<Vector2>();

            foreach (Vector2 cell in cells)
            {
                var path = _game.GetPath(characterPosition, cell);
                if (path == null || path.Count < 1) continue;

                if (shortestPath.Count == 0 || shortestPath.Count > path.Count)
                {
                    closestCell = cell;
                    shortestPath = path;
                }
            }

            if (shortestPath.Count < 1) return;

            var destination = shortestPath.Last();
            if (destination.Equals(closestCell))
                destination = shortestPath[shortestPath.Count - 2];

            SetTargetPosition(_game.GetCellPosition(destination));
            _movingToDirective = true;
        }

        public void Highlight(bool highlight)
        {
            var material = GetNode<AnimatedSprite>("AnimatedSprite").Material as ShaderMaterial;
            material.SetShaderParam("visible", highlight);
            //material.SetShaderParam("outline_color", new Color("1fa0ff"));
        }
    }
}
