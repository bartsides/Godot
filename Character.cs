using Godot;
using NotRimworld.Directives;
using NotRimworld.Enums;
using NotRimworld.Needs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotRimworld
{
	public class Character : Position2D
	{
		public CharacterState State { get; private set; }

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

		private TileMap _tileMap;
		private TileMap TileMap
		{
			get
			{
				if (_tileMap != null) return _tileMap;
				_tileMap = GetParent().GetNode<TileMap>("TileMap");
				return _tileMap;
			}
		}


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
        {
            _path = new List<Vector2>();
            _targetPosition = new Vector2();
            _targetPointWorld = new Vector2();
            _velocity = new Vector2();

			_needs = new List<INeed> {new VapeNeed()};

			ChangeState(CharacterState.Idle);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			IncrementNeeds(delta);
			HandleDirectives(delta);
			HandleMove(delta);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed("click"))
			{
				ClearDirective();
				SetTargetPosition(GetGlobalMousePosition());
			}
		}

		private void SetTargetPosition(Vector2 position)
		{
			_targetPosition = position;
			ChangeState(CharacterState.Follow);
		}

		private void ChangeState(CharacterState state)
		{
			if (state == CharacterState.Follow)
			{
                _path = TileMap.GetPath(Position, _targetPosition);
                if (_path == null || _path.Count <= 1)
                {
					ChangeState(CharacterState.Idle);
                    return;
                }

                _targetPointWorld = _path[1];
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
			if (State != CharacterState.Follow) return;

			MoveTo(_targetPointWorld);

			if (!ArrivedTo(_targetPointWorld)) return;

			_path.RemoveAt(0);
            if (_path.Count == 0)
            {
				ChangeState(CharacterState.Idle);
                return;
            }

            _targetPointWorld = _path[0];
        }

        private void MoveTo(Vector2 destination)
        {
            var desiredVelocity = (destination - Position).Normalized() * _speed;
            var steering = desiredVelocity - _velocity;
            _velocity += steering / Mass;
            Position += _velocity * GetProcessDeltaTime();
            Rotation = _velocity.Angle();
        }

        private bool ArrivedTo(Vector2 destination)
        {
            return Position.DistanceTo(destination) < ArriveDistance;
        }

		public void GoToClosest(int id)
		{
			if (TileMap == null) throw new Exception("tilemap not found");
			var cells = TileMap.GetUsedCellsById(id);
			if (cells.Count == 0) return;

			var characterPosition = TileMap.WorldToMap(TileMap.ToLocal(GlobalPosition));
            var closestCell = Vector2.Inf;
            var shortestPath = new List<Vector2>();

            foreach (Vector2 cell in cells)
            {
                var path = TileMap.GetPath(characterPosition, cell);
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

			SetTargetPosition(TileMap.GetCellPosition(destination));
            _movingToDirective = true;
        }
	}
}
