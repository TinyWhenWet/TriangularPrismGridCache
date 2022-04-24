using System.Collections.Generic;
using Unity.Mathematics;

namespace ShapeGrid
{
	public class Grid<TKey, TValue> where TKey : unmanaged
	{
		public Grid(Shape shape, float size = 1f)
		{
			Items = new();
			Shape = shape;
			Size = size;
		}

		// Public properties.
		public Dictionary<TKey, HashSet<TValue>> Items { get; private set; }
		public float Size { get; private set; }
		public Shape Shape { get; private set; }

		// Protected fields.
		protected SpatialEncoder<TKey> _encoder = new();

		// Public methods.
		public virtual TKey GetIndex(float3 position)
		{
			return _encoder.Encode(position, Size / Shape.Scale);
		}

		public virtual float3 GetPosition(TKey key)
		{
			return _encoder.Decode(key, Size / Shape.Scale);
		}

		public virtual int Add(float3 position, TValue item)
		{
			int count = 0;
			float3[] corners = Shape.GetCorners(position, Shape.GetGrid(position, Size), Size);

			foreach (float3 corner in corners)
			{
				TKey key = GetIndex(corner);
				
				if (!Items.TryGetValue(key, out HashSet<TValue> set))
				{
					set = new HashSet<TValue>();
					Items.Add(key, set);
				}

				if (set.Add(item))
				{
					count++;
				}
			}

			return count;
		}
		
		public virtual int Remove(float3 position, TValue item)
		{
			int count = 0;
			float3[] corners = Shape.GetCorners(position, Shape.GetGrid(position, Size), Size);

			foreach (float3 corner in corners)
			{
				TKey key = GetIndex(corner);
				
				if (!Items.TryGetValue(key, out HashSet<TValue> set))
				{
					continue;
				}

				if (set.Remove(item))
				{
					count++;
				}
			}

			return count;
		}

		public virtual bool Contains(float3 position, TValue item)
		{
			float3[] corners = Shape.GetCorners(position, Shape.GetGrid(position, Size), Size);

			foreach (float3 corner in corners)
			{
				TKey key = GetIndex(corner);
				
				if (Items.TryGetValue(key, out HashSet<TValue> set) && set.Contains(item))
				{
					return true;
				}
			}

			return false;
		}
	}
}