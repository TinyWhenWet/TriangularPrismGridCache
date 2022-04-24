using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

namespace ShapeGrid
{
	public sealed class MeshVerticesExample : MonoBehaviour
	{
		public float gridSize = 0.3f;

		private ShapeGrid.Grid<uint, int> _grid;
		private ShapeGrid.Prism _shape = new();
		private List<Vector3> _vertices = new();
		[SerializeField, HideInInspector] private Transform _pivot;
		private Stopwatch _stopwatch = new();

		private void OnEnable()
		{
			_grid = new(_shape, gridSize);

			if (_stopwatch.IsRunning)
			{
				_stopwatch.Restart();
			}
			else
			{
				_stopwatch.Start();
			}

			// Cache vertices from meshes.
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

			foreach (SkinnedMeshRenderer renderer in renderers)
			{
				Transform _transform = renderer.transform;
				
				Mesh mesh = new();
				renderer.BakeMesh(mesh);

				foreach (Vector3 vertex in mesh.vertices)
				{
					Vector3 _vertex = transform.InverseTransformPoint(_transform.TransformPoint(vertex));
					_vertices.Add(_vertex);
				}
			}

			Debug.Log($"Mesh took {_stopwatch.ElapsedMilliseconds}ms to bake");

			// Cache vertices into grids.
			_stopwatch.Restart();

			for (int i = 0; i < _vertices.Count; i++)
			{
				_grid.Add(_vertices[i], i);
			}

			Debug.Log($"Caching {_vertices.Count} vertices took {_stopwatch.ElapsedMilliseconds}ms");

			// Create debug pivot.
			if (_pivot == null)
			{
				_pivot = new GameObject("PIVOT").transform;
				_pivot.SetParent(transform, false);
			}
		}

		private void OnDisable()
		{
			_vertices.Clear();
			_grid = null;
		}

		private void Update()
		{
			if (_pivot == null)
			{
				return;
			}

			Vector3 pivot = transform.InverseTransformPoint(_pivot.position);
			int3 gridPosition = _shape.GetGrid(pivot, gridSize);
			float3[] corners = _shape.GetCorners(pivot, gridPosition, gridSize);
			List<Vector3> vertices = new();

			_stopwatch.Restart();
			
			// Cache vertices.
			foreach (float3 corner in corners)
			{
				Debug.DrawLine(transform.TransformPoint((Vector3)corner), _pivot.position, Color.red);

				uint gridIndex = _grid.GetIndex(corner);
				
				if (_grid.Items.TryGetValue(gridIndex, out HashSet<int> set))
				{
					foreach (int vertexIndex in set)
					{
						Vector3 vertex = _vertices[vertexIndex];
						vertices.Add(vertex);
					}
				}
			}

			Debug.Log($"Searching took {_stopwatch.ElapsedMilliseconds}ms");

			// Draw vertices.
			foreach (Vector3 vertex in vertices)
			{
				Debug.DrawLine(_pivot.position, transform.TransformPoint(vertex), Color.cyan);
			}
		}
	}
}