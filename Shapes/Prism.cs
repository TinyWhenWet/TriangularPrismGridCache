using System;
using Unity.Mathematics;

namespace ShapeGrid
{
	public class Prism : Shape
	{
		// Public properties.
		public override int[] Indices => new int[]
		{
			2, 1, 0,
			3, 4, 5,

			0, 1, 3,
			1, 4, 3,

			0, 3, 5,
			2, 0, 5,

			2, 5, 1,
			1, 5, 4,
		};

		public override float3[] Vertices => new float3[]
		{
			new float3(-0.5f, -0.5f, -0.5f),
			new float3(-0.5f, -0.5f, 0.5f),
			new float3(0.5f, -0.5f, 0f),
			new float3(-0.5f, 0.5f, -0.5f),
			new float3(-0.5f, 0.5f, 0.5f),
			new float3(0.5f, 0.5f, 0f),
		};

		// Public methods.
		public override float3 GetPosition(int3 grid, float size = 1f)
		{
			return new float3(
				grid.x * size,
				grid.y * size,
				grid.z * size / 2f
			);
		}

		public override int3 GetGrid(float3 position, float size = 1f)
		{
			float x = position.x / size;
			float y = position.y / size;
			float z = position.z / size;

			float xAbs = Math.Abs(x) + 0.5f;
			float yAbs = Math.Abs(y) + 0.5f;
			float zAbs = Math.Abs(z) * 2f;

			int xEven = (int)((Math.Abs(x) + Math.Sign(x) * 0.5) % 2.0);
			float zEven = zAbs % 2f;

			zAbs += (xEven == 0 && zEven > 1f) || (xEven == 1 && zEven < 1f) ? 1f - xAbs % 1f : xAbs % 1f;

			return new int3(
				(int)xAbs * Math.Sign(x),
				(int)yAbs * Math.Sign(y),
				(int)zAbs * Math.Sign(z)
			);
		}

		public override float3[] GetCorners(float3 position, int3 grid, float size = 1f)
		{
			float3 first, second, third;
			float3 gridPosition = GetPosition(grid, size);
			float3 delta = position - gridPosition;

			float scale = Math.Abs(grid.x) % 2 - Math.Abs(grid.z) % 2 == 0 ? 1f : -1f;
			double angle = Math.Atan2(delta.z, delta.x * scale);
			float y = delta.y > 0f ? size * 0.5f : -size * 0.5f;

			bool isPositive = angle > 0.0;

			// The first corner.
			if (angle >= -2.356194 && angle <= 2.356194)
			{
				first = new float3(size * scale * 0.5f, y, 0f);
			}
			else if (isPositive)
			{
				first = new float3(-size * scale * 0.5f, y, -size * 0.5f);
			}
			else
			{
				first = new float3(-size * scale * 0.5f, y, size * 0.5f);
			}

			// The second corner.
			if (isPositive)
			{
				second = new float3(-size * scale * 0.5f, y, size * 0.5f);
			}
			else
			{
				second = new float3(-size * scale * 0.5f, y, -size * 0.5f);
			}

			// The third corner.
			third = new float3(second.x, y * -1f, second.z);

			// Return all three corners.
			return new float3[]
			{
				gridPosition + first,
				gridPosition + second,
				gridPosition + third,
			};
		}
	}
}
