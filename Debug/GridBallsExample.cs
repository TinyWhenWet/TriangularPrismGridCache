using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

namespace ShapeGrid
{
	public sealed class GridBallsExample : MonoBehaviour
	{
		// Public fields.
		public Material material;
		public Transform pivot;
		public GameObject prefab;
		public bool enableSpawning;

		// Private fields.
		private const float k_size = 8f;

		private Prism _prism = new();
		private ShapeGrid.Grid<uint, GameObject> _grid = new(new Prism(), k_size);
		private Texture2D _tex;
		private float _lastSpawn;
		private Queue<Item<uint, GameObject>> _instances = new();

		// Public methods.
		public void Generate()
		{
			if (!gameObject.TryGetComponent<MeshFilter>(out MeshFilter filter))
			{
				filter = gameObject.AddComponent<MeshFilter>();

				MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
				renderer.sharedMaterial = material;
			}

			filter.sharedMesh = ShapeTools.GenerateMesh(_prism.Vertices, _prism.Indices, k_size);

			_tex = new(Screen.height / 2, Screen.height / 2);
			float scale = 0.02f;

			for (int x = 0; x < _tex.width; x++)
			{
				for (int y = 0; y < _tex.height; y++)
				{
					int3 grid = _prism.GetGrid(new float3(x * scale, 0, y * scale));
					Random.InitState(grid.x * _tex.width + grid.y + grid.z);
					_tex.SetPixel(x, y, new Color(Random.value, Random.value, Random.value, 1f));
				}
			}

			_tex.Apply();
		}

		// Private methods.
		private void OnEnable()
		{
			CollisionCheck(32);
			Generate();
		}

		private void OnDisable()
		{
			if (gameObject.TryGetComponent<MeshFilter>(out MeshFilter filter) && filter.sharedMesh != null)
			{
				Destroy(filter.sharedMesh);
			}

			if (_tex != null)
			{
				Destroy(_tex);
			}
		}

		private void Update()
		{
			int width = 8;
			int height = 4;
			int depth = 3;
			int resolution = width * height;

			// Get the pivot transform in grid space.
			Vector3 pivotPosition = transform.InverseTransformPoint(pivot.position);

			// Get the grid position of the pivot.
			int3 pivotGrid = _prism.GetGrid(pivotPosition, k_size);

			// Get the grid center of the pivot's grid.
			Vector3 pivotCenter = _prism.GetPosition(pivotGrid, k_size);

			// Get the nearest corners from our shape.
			float3[] corners = _prism.GetCorners(pivotPosition, pivotGrid, k_size);

			// Draw the grid.
			for (int i = 0; i < resolution * depth; i++)
			{
				int x = i % resolution / width - height / 2;
				int y = i / resolution - depth / 2;
				int z = i % resolution % width - width / 2;
				
				Color color = pivotGrid.x == x && pivotGrid.y == y && pivotGrid.z == z ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 1f, 0f, 0.05f);

				DrawGrid(new(x, y, z), k_size, color);
			}

			// Draw the pivot to the center of the grid.
			Debug.DrawLine(transform.TransformPoint(pivotCenter), pivot.position, Color.green);

			// Draw the nearest corners.
			Debug.DrawLine(transform.TransformPoint(corners[0]), pivot.position, Color.magenta);
			Debug.DrawLine(transform.TransformPoint(corners[1]), pivot.position, Color.magenta);
			Debug.DrawLine(transform.TransformPoint(corners[2]), pivot.position, Color.magenta);

			// Spawn test instances.
			if (enableSpawning && prefab != null && Time.time - _lastSpawn > 0.05f)
			{
				_lastSpawn = Time.time;
				Spawn();
			}

			// Update test instances - a crude example of how to use the grid.
			foreach (Item<uint, GameObject> instance in _instances)
			{
				GameObject gameObject = instance.Value;
				
				if (gameObject == null)
				{
					continue;
				}

				if (Random.value < 0.2f)
				{
					Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
					rigidbody.velocity += new Vector3(Random.value * 2f - 1f, Random.value * 2f - 1f, Random.value * 2f - 1f);
				}

				// Set the item position to the instance position.
				instance.Position = gameObject.transform.position;

				// Test finding nearby items.
				HashSet<GameObject> drawn = new();

				foreach (uint corner in instance.Corners)
				{
					if (!_grid.Items.TryGetValue(corner, out HashSet<GameObject> items))
					{
						continue;
					}

					float3 cornerPosition = _grid.GetPosition(corner);

					Debug.DrawLine(gameObject.transform.position, (Vector3)cornerPosition, Color.green);

					foreach (GameObject item in items)
					{
						if (item == null || drawn.Contains(item))
						{
							continue;
						}

						Debug.DrawLine(gameObject.transform.position, item.transform.position, Color.blue);

						drawn.Add(item);
					}
				}
			}
		}

		private void OnGUI()
		{
			if (_tex != null)
			{
				GUI.DrawTexture(new Rect(0f, 0f, _tex.width * 2, _tex.height * 2), _tex);
			}
		}

		private void DrawGrid(int3 grid, float size, Color color)
		{
			// Get the grid position.
			Vector3 position = transform.TransformPoint(_prism.GetPosition(grid, size));

			// Rotate alternating.
			Quaternion rotation = transform.rotation * Quaternion.AngleAxis((grid.x % 2 * 180f) + (grid.z % 2 * 180f), Vector3.up);

			// Draw the grid.
			ShapeTools.DrawDebug(_prism.Vertices, _prism.Indices, position, rotation, size, color);
		}

		private void Spawn()
		{
			// Prevent spawning too many instances.
			if (_instances.Count > 100)
			{
				// Get the oldest instance.
				Item<uint, GameObject> _instance = _instances.Dequeue();

				// Make sure the instance still exists.
				if (_instance.Value != null)
				{
					// Destroy the instance.
					Destroy(_instance.Value);
				}

				// Dispose the item.
				_instance.Dispose();
			}

			// Gets a random position.
			Vector3 position = pivot.position + new Vector3(Random.value * 2f - 1f, Random.value, Random.value * 2f - 1f) * 10f;

			// Create a GameObject to represent the instance.
			GameObject instance = Instantiate(prefab);
			instance.transform.position = position;

			Rigidbody rigidbody = instance.GetComponent<Rigidbody>();
			rigidbody.velocity = new Vector3(Random.value * 2f - 1f, Random.value * 2f - 1f, Random.value * 2f - 1f) * 10f;

			// Create and cache the item instance.
			_instances.Enqueue(new(_grid, position, instance));
		}

		private void CollisionCheck(int size)
		{
			HashSet<uint> test = new();

			Stopwatch stopwatch = new();
			stopwatch.Start();

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					for (int z = 0; z < size; z++)
					{
						float3 position = new float3(x, y, z) * k_size - new float3(size, size, size) * k_size * 0.5f;
						uint index = _grid.GetIndex(position);

						// Debug.Log($"{index} - {position}");

						if (test.Contains(index))
						{
							Debug.Log($"Duplicate index: {index} at {position}");
							return;
						}

						test.Add(index);
					}
				}
			}

			Debug.Log($"Done searching {Mathf.Pow(size, 3)} grids for collisions in {stopwatch.ElapsedMilliseconds}ms");
		}
	}
}
