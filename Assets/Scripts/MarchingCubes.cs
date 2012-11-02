using System;
using UnityEngine;

[Serializable]
public class MarchingCubes : BaseMarchingCubes
{
	public bool capped;

	protected override Vertex GetVertex(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= Steps.x || y >= Steps.y || z >= Steps.z)
		{
			Vertex v = new Vertex
			           {
			           	pos = GetPos(x, y, z),
			           };
			v.flux = metaBalls.GetVertexValue(v);
			v.inside = v.flux > iso;


			Vector3 tangent;
			tangent.x = (GetPos(x, y - 1, z) - GetPos(x + 1, y, z)).x;
			tangent.y = (GetPos(x - 1, y, z) - GetPos(x, y + 1, z)).y;
			tangent.z = (GetPos(x, y, z - 1) - GetPos(x, y, z + 1)).z;

			v.normal.x = metaBalls.GetVertexValue(new Vertex {pos = GetPos(x - 1, y, z)}) -
			             metaBalls.GetVertexValue(new Vertex {pos = GetPos(x + 1, y, z)});
			v.normal.y = metaBalls.GetVertexValue(new Vertex {pos = GetPos(x, y - 1, z)}) -
			             metaBalls.GetVertexValue(new Vertex {pos = GetPos(x, y + 1, z)});
			v.normal.z = metaBalls.GetVertexValue(new Vertex {pos = GetPos(x, y, z - 1)}) -
			             metaBalls.GetVertexValue(new Vertex {pos = GetPos(x, y, z + 1)});
			v.normal.Normalize();
			tangent.Normalize();
			v.tangent = Vector3.Cross(tangent, v.normal);

			//http://en.wikipedia.org/wiki/UV_mapping#Finding_UV_on_a_sphere
			v.uv.x = 0.5f - Mathf.Atan2(-v.normal.z, -v.normal.x) / (Mathf.PI * 2);
			v.uv.y = 0.5f - 2.0f * (Mathf.Asin(-v.normal.y) / (Mathf.PI * 2));

			return v;
		}
		return vertices[GetIndex(x, y, z)];
	}


	private void OnEnable()
	{
		if (vertices == null)
			RegenerateGrid();
	}


	[SerializeField] public MetaBalls metaBalls;

	public void ComputeMetaBalls()
	{
		if (vertices == null)
			RegenerateGrid();

		if (metaBalls == null)
		{
			metaBalls = new MetaBalls();
			metaBalls.Initialize();
		}

		metaBalls.isoValue = iso;

		for (int z = 0; z < Steps.z; z++)
		{
			for (int y = 0; y < Steps.y; y++)
			{
				for (int x = 0; x < Steps.x; x++)
				{
					Vertex v = vertices[GetIndex(x, y, z)];
					v.flux = metaBalls.GetVertexValue(v);
					//v.flux = Random.Range(-1.0f, 1.0f);
					if (capped && (z == 0 || x == 0 || y == 0 || x == Steps.x - 1 || y == Steps.y - 1 || z == Steps.z - 1))
					{
						v.flux = 0;
						if (z == 0)
						{
							//float a = metaBalls.GetVertexValue(vertices[x, y, 2]);
							//float b = metaBalls.GetVertexValue(vertices[x, y, 1]);
							//v.flux = a;
						}
						//v.inside = false;
					}
					v.inside = v.flux > iso;

					vertices[GetIndex(x, y, z)] = v;
				}
			}
		}

		for (int z = 0; z < Steps.z; z++)
		{
			for (int y = 0; y < Steps.y; y++)
			{
				for (int x = 0; x < Steps.x; x++)
				{
					Vertex v = vertices[GetIndex(x, y, z)];
					Vector3 tangent;

					const float multiplier = 2;
					if (x == 0)
					{
						v.normal.x = (v.flux - vertices[GetIndex(x + 1, y, z)].flux) * multiplier;
						tangent.x = (v.pos - vertices[GetIndex(x + 1, y, z)].pos).x * multiplier;
					}
					else if (x == Steps.x - 1)
					{
						v.normal.x = (vertices[GetIndex(x - 1, y, z)].flux - v.flux) * multiplier;
						tangent.x = (vertices[GetIndex(x - 1, y, z)].pos - v.pos).x * multiplier;
					}
					else
					{
						v.normal.x = vertices[GetIndex(x - 1, y, z)].flux - vertices[GetIndex(x + 1, y, z)].flux;
						tangent.x = (vertices[GetIndex(x - 1, y, z)].pos - vertices[GetIndex(x + 1, y, z)].pos).x;
					}

					if (y == 0)
					{
						v.normal.y = (v.flux - vertices[GetIndex(x, y + 1, z)].flux) * multiplier;
						tangent.y = (v.pos - vertices[GetIndex(x, y + 1, z)].pos).y * multiplier;
					}
					else if (y == Steps.y - 1)
					{
						v.normal.y = (vertices[GetIndex(x, y - 1, z)].flux - v.flux) * multiplier;
						tangent.y = (vertices[GetIndex(x, y - 1, z)].pos - v.pos).y * multiplier;
					}
					else
					{
						v.normal.y = vertices[GetIndex(x, y - 1, z)].flux - vertices[GetIndex(x, y + 1, z)].flux;
						tangent.y = (vertices[GetIndex(x, y - 1, z)].pos - vertices[GetIndex(x, y + 1, z)].pos).y;
					}

					if (z == 0)
					{
						v.normal.z = (v.flux - vertices[GetIndex(x, y, z + 1)].flux) * multiplier;
						tangent.z = (v.pos - vertices[GetIndex(x, y, z + 1)].pos).z * multiplier;
					}
					else if (z == Steps.z - 1)
					{
						v.normal.z = (vertices[GetIndex(x, y, z - 1)].flux - v.flux) * multiplier;
						tangent.z = (vertices[GetIndex(x, y, z - 1)].pos - v.pos).z * multiplier;
					}
					else
					{
						v.normal.z = vertices[GetIndex(x, y, z - 1)].flux - vertices[GetIndex(x, y, z + 1)].flux;
						tangent.z = (vertices[GetIndex(x, y, z - 1)].pos - vertices[GetIndex(x, y, z + 1)].pos).z;
					}

					v.normal.Normalize();
					tangent.Normalize();
					v.tangent = Vector3.Cross(tangent, v.normal);

					//http://en.wikipedia.org/wiki/UV_mapping#Finding_UV_on_a_sphere
					v.uv.x = 0.5f - Mathf.Atan2(-v.normal.z, -v.normal.x) / (Mathf.PI * 2);
					v.uv.y = 0.5f - 2.0f * (Mathf.Asin(-v.normal.y) / (Mathf.PI * 2));

					vertices[GetIndex(x, y, z)] = v;
				}
			}
		}

		Hash = UnityEngine.Random.Range(1, int.MaxValue);
	}


}

[Serializable]
public class Vertex
{
	public Vector3 pos;
	public float flux;
	public bool inside;
	public Vector3 normal;
	public Vector2 uv;
	public Vector4 tangent;
}

[Serializable]
public class IntVector3
{
	[SerializeField]
	public int x, y, z;

	public IntVector3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static implicit operator IntVector3(Vector3 v)
	{
		return new IntVector3((int) v.x, (int) v.y, (int) v.z);
	}

	public static implicit operator Vector3(IntVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static IntVector3 operator -(IntVector3 a, IntVector3 b)
	{
		return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public float SqrMagnitude
	{
		get { return x * x + y * y + z * z; }
	}

	public float Magnitude
	{
		get { return Mathf.Sqrt(SqrMagnitude); }
	}

	public bool Same(IntVector3 other)
	{
		return (this - other).SqrMagnitude <= Mathf.Epsilon;
	}
}