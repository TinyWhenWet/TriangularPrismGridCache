using Unity.Mathematics;

namespace ShapeGrid
{
	public abstract class Shape
	{
		public abstract int[] Indices { get; }
		
		public abstract float3[] Vertices { get; }

		public abstract float3[] GetCorners(float3 position, int3 grid, float size = 1f);

		public abstract int3 GetGrid(float3 position, float size = 1f);

		public abstract float3 GetPosition(int3 grid, float size = 1f);
	}
}