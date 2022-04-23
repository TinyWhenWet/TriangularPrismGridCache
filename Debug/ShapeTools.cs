using UnityEngine;
using Unity.Mathematics;

namespace ShapeGrid
{
	public static class ShapeTools
	{
		public static Mesh GenerateMesh(float3[] vertices, int[] indices, float size = 1f)
		{
			Vector3[] _vertices = new Vector3[vertices.Length];

			for (int i = 0; i < vertices.Length; i++)
			{
				_vertices[i] = vertices[i] * size;
			}

			// Create mesh.
			Mesh mesh = new Mesh();
			mesh.vertices = _vertices;
			mesh.triangles = indices;

			// Run calculations.
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();
			
			// Return mesh.
			return mesh;
		}

		public static void DrawDebug(float3[] vertices, int[] indices, Vector3 origin, Quaternion rotation, float scale, Color color)
		{
			for (int i = 0; i < indices.Length; i += 3)
			{
				int p0 = indices[i];
				int p1 = indices[i + 1];
				int p2 = indices[i + 2];

				Vector3 v0 = rotation * (vertices[p0] * scale);
				Vector3 v1 = rotation * (vertices[p1] * scale);
				Vector3 v2 = rotation * (vertices[p2] * scale);

				Debug.DrawLine(origin + v0, origin + v1, color);
				Debug.DrawLine(origin + v1, origin + v2, color);
				Debug.DrawLine(origin + v2, origin + v0, color);
			}
		}
	}
}