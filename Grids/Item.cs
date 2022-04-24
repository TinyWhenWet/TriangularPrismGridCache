using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ShapeGrid
{
	public class Item<TKey, TValue> : IDisposable where TKey : unmanaged
	{
		public Item(Grid<TKey, TValue> grid, float3 position, TValue value)
		{
			Grid = grid;
			Value = value;
			Position = position;
		}

		// Protected fields.
		protected float3 _position;
		protected float3? _lastPosition = null;

		// Public properties.
		public Grid<TKey, TValue> Grid { get; private set; }
		public TValue Value { get; private set; }
		public TKey[] Corners { get; private set; }

		public float3 Position
		{
			get => _position;

			set
			{
				_position = value;

				if (ShouldUpdate())
				{
					_lastPosition = _position;

					UpdatePosition();
				}
			}
		}

		// Public methods.
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}

		public virtual void Dispose()
		{
			if (Corners == null)
			{
				return;
			}

			foreach (TKey corner in Corners)
			{
				if (Grid.Items.TryGetValue(corner, out HashSet<TValue> set))
				{
					set.Remove(Value);
				}
			}

			Corners = null;
		}

		// Protected methods.
		protected virtual bool ShouldUpdate()
		{
			if (!_lastPosition.HasValue)
			{
				return true;
			}

			float3 delta = (_position - _lastPosition.Value);
			float magnitude = Math.Abs(delta.x) + Math.Abs(delta.y) + Math.Abs(delta.z);

			return magnitude > 1f;
		}

		protected virtual void UpdatePosition()
		{
			// Cache shape.
			Shape shape = Grid.Shape;

			// Get current corners.
			float3[] _corners = shape.GetCorners(Position, shape.GetGrid(Position, Grid.Size), Grid.Size);
			TKey[] corners = new TKey[_corners.Length];

			for (int i = 0; i < _corners.Length; i++)
			{
				corners[i] = Grid.GetIndex(_corners[i]);
			}
		
			// Uncache previous corners.
			if (Corners != null)
			{
				for (int i = 0; i < _corners.Length; i++)
				{
					TKey corner = Corners[i];

					if (!corners[i].Equals(corner) && Grid.Items.TryGetValue(corner, out HashSet<TValue> set))
					{
						set.Remove(Value);
					}
				}
			}

			// Set new corners.
			for (int i = 0; i < _corners.Length; i++)
			{
				TKey corner = corners[i];

				if (Corners == null || !corner.Equals(Corners[i]))
				{
					if (!Grid.Items.TryGetValue(corner, out HashSet<TValue> set))
					{
						set = new HashSet<TValue>();
						Grid.Items.Add(corner, set);
					}

					set.Add(Value);
				}
			}

			Corners = corners;
		}
	}
}