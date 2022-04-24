using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ShapeGrid
{
	public class SpatialEncoder<T> where T : unmanaged
	{
		public SpatialEncoder()
		{
			if (!s_types.Contains(typeof(T)))
			{
				throw new Exception($"Type {typeof(T).Name} is not supported for {GetType().Name}.");
			}

			unsafe
			{
				_bits = sizeof(T) * 8;
			}

			int componentSize = (int)(_bits / 3.0);

			_x = 0;
			_y = componentSize;
			_z = componentSize * 2;

			_offset = Math.Pow(2, componentSize) / 2 - 1;
		}

		// Protected fields.
		protected static HashSet<Type> s_types = new()
		{
			typeof(uint),
			typeof(ulong),
			typeof(byte),
			typeof(ushort),
		};

		protected dynamic _offset;
		protected int _bits;
		protected int _x;
		protected int _y;
		protected int _z;

		// Public methods.
		public virtual T Encode(float3 position, float size = 1f)
		{
			return (T)(dynamic)(
				(long)(position.x / size + _offset) << _x |
				(long)(position.y / size + _offset) << _y |
				(long)(position.z / size + _offset) << _z
			);
		}

		public virtual float3 Decode(dynamic value, float size = 1f)
		{
			dynamic mask = (1 << _y) - 1;

			return new(
				(float)(((value >> _x) & mask) - _offset) * size,
				(float)(((value >> _y) & mask) - _offset) * size,
				(float)(((value >> _z) & mask) - _offset) * size
			);
		}
	}
}