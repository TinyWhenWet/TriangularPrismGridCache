using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ShapeGrid
{
	public sealed class SpatialEncoder<T> where T : unmanaged
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

		// Private fields.
		private static HashSet<Type> s_types = new()
		{
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
		};

		private dynamic _offset;
		private int _bits;
		private int _x;
		private int _y;
		private int _z;

		// Public methods.
		public T Encode(float3 position, float size = 1f)
		{
			return (T)(dynamic)(
				(long)(position.x / size + _offset) << _x |
				(long)(position.y / size + _offset) << _y |
				(long)(position.z / size + _offset) << _z
			);
		}

		public float3 Decode(dynamic value, float size = 1f)
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