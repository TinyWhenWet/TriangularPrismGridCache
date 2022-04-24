<h1 align="center">
	<img src=".Images/PrismGrid.png" alt="Prism Grids" width="300">
	<br>
	Triangular Prism Based Grid Caching
</h1>

<p align="center">
	<a href="https://opensource.org/licenses/MIT">
		<img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License (MIT)">
	</a>
	<a href="https://discord.gg/ssMzQPXjwq">
		<img src="https://img.shields.io/discord/318871569323524096?label=Discord&logo=discord&logoColor=white" alt="Discord">
	</a>
	<a href="https://paypal.me/jameslroll">
		<img src="https://img.shields.io/badge/Donate-PayPal-green.svg?logo=paypal" alt="Donate (PayPal)">
	</a>
	<a href="https://www.buymeacoffee.com/jameslroll">
		<img src="https://img.shields.io/badge/Donate-Buy_Me_a_Coffee-green.svg?logo=buymeacoffee" alt="Donate (Buy Me a Coffee)">
	</a>
</p>

It's a common optimization practice to partion large amounts of objects into grids. Given any point or object in space, nearby objects can be found by searching only the grid partions in proximity to the point or object. A simple and popular approach for calculating the grid index utilizes squares (2D) or cubes (3D) which rounds the input position into a chunk given an arbitrary size.

While convenient, cubes are not entirely efficient. This method proposes the usage of triangles (2D) and prisms (3D) for index caching (triangular prisms are referred to as prisms throughout this project to keep things simple). A cube has 8 total points while a prism has 6. Nearby objects can be found by searching proximal partitions. It should be noted that searching all adjacent grids is redundant. It's only necessary to search immediately adjcent grids given a point. Additionally, this method caches the object directly to the 3 nearest corners of the current prism (or 2 given a triangle). Mitigating the search of nearby partitions during lookup. This trades off more memory usage and slower index caching speeds for faster index searching speeds. An object only needs to be indexed upon moving a considerable distance. Whereas searching may be a constant process.

When indexing, each grid needs a unique key to represent it for the partition. The spatial coordinates can be converted into a 32-bit integer, which limits each dimension to 1024 partitions. A 64-bit integer is used for larger spaces, limiting each dimension to 2,097,152 partitions. The spatial encoder also supports 8-bit and 16-bit for *very* small grid spaces. Each coordinate's axes are encoded into an n-bit integer using bit-shifting.

Objects are cached into the 3 nearest corners of the current prism on an infinite 3-dimensional grid using the encoded integer. Furthermore, by looking up an object's indexes in the cache, nearby objects can be discovered. This is useful for a multitude of applications where objects are interdependent.

![Objects viewed from above](.Images/ObjectsTop.gif)

![Objects viewed from the side](.Images/ObjectsSide.png)

# Documentation

### Shapes

The abstract [Shape](./Shapes/Shape.cs) class represents the basis for arbitrary shapes.

### Grids

The [Grid](./Grids/Grid.cs) class takes a shape, size, and collection of items. There are two generic parameters: the key and value. The key represents the integer type used for indexing which must be an unmanaged unsigned numerical value. The value may represent any type and is used in the grid's collection. The collection is represented by a [HashSet](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=net-6.0). This allows items to be added, removed, and searched more efficiently.

This example creates a grid with a 64-bit integer (`long`) index where each grid may contain a collection of 32-bit integer (`int`) values. The input shape is a [Prism](./Shapes/Prism.cs) instance with a size of `s = 1.5 units`. The maximum grid space can be calculated as such: `m = 2^floor(n / 3)` where `n` is the number of bits in the integer. This makes our example grid cover `m * s = 3,145,728 units` along each 3-dimensional axis.

```C#
ShapeGrid.Grid<long, int> grid = new(new Prism(), 1.5f)
```

### Items

# Dependencies

This project was built in Unity and C#. Adaptability was considered in development, so converting to usage independent of Unity should be simple. Unity specific functions are be isolated to the [Debug folder](Debug). Mathematics otherwise throughout the project utilizes the [Unity.Mathematics](https://docs.unity3d.com/Packages/com.unity.mathematics@1.1/manual/index.html) library.
