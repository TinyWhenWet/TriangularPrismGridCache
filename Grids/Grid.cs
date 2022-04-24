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

		public virtual bool Add(float3 position, TValue item)
		{
			TKey key = GetIndex(position);

			if (!Items.TryGetValue(key, out HashSet<TValue> set))
			{
				set = new HashSet<TValue>();
				Items.Add(key, set);
			}

			return set.Add(item);
		}

		public virtual bool Remove(float3 position, TValue item)
		{
			TKey key = GetIndex(position);

			if (!Items.TryGetValue(key, out HashSet<TValue> set))
			{
				return false;
			}

			return set.Remove(item);
		}

		public virtual bool Contains(float3 position, TValue item)
		{
			TKey key = GetIndex(position);

			if (!Items.TryGetValue(key, out HashSet<TValue> set))
			{
				return false;
			}

			return set.Contains(item);
		}
	}
}