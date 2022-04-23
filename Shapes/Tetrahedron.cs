// using System;
// using Unity.Mathematics;

// namespace ShapeGrid
// {
// 	public class Tetrahedron : Shape
// 	{
// 		private static readonly float s_c0 = 1f / 3f;
// 		private static readonly float s_c1 = (float)Math.Sqrt(8.0 / 9.0);
// 		private static readonly float s_c2 = (float)Math.Sqrt(2.0 / 9.0);
// 		private static readonly float s_c3 = (float)Math.Sqrt(2.0 / 3.0);

// 		public override int[] Indices => new int[]
// 		{
// 			2, 3, 1,
// 			1, 3, 0,
// 			3, 2, 0,
// 			2, 1, 0,
// 		};

// 		public override float3[] Vertices => new float3[]
// 		{
// 			new float3(s_c1, 0f, -s_c0),
// 			new float3(-s_c2, s_c3, -s_c0),
// 			new float3(-s_c2, -s_c3, -s_c0),
// 			new float3(0f, 0f, 1f),
// 		};
// 	}
// }
