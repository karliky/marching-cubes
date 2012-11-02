using System;
using UnityEngine;

[Serializable]
public class MarchingCubes : BaseMarchingCubes
{
	public bool capped;


	private void OnEnable()
	{
		if (vertices == null)
			RegenerateGrid();
	}

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

			v.normal.x = metaBalls.GetVertexValue(new Vertex { pos = GetPos(x - 1, y, z) }) -
						 metaBalls.GetVertexValue(new Vertex { pos = GetPos(x + 1, y, z) });
			v.normal.y = metaBalls.GetVertexValue(new Vertex { pos = GetPos(x, y - 1, z) }) -
						 metaBalls.GetVertexValue(new Vertex { pos = GetPos(x, y + 1, z) });
			v.normal.z = metaBalls.GetVertexValue(new Vertex { pos = GetPos(x, y, z - 1) }) -
						 metaBalls.GetVertexValue(new Vertex { pos = GetPos(x, y, z + 1) });
			v.normal.Normalize();
			tangent.Normalize();
			v.tangent = Vector3.Cross(tangent, v.normal);

			//http://en.wikipedia.org/wiki/UV_mapping#Finding_UV_on_a_sphere
			v.uv.x = 0.5f - Mathf.Atan2(-v.normal.z, -v.normal.x) / (Mathf.PI * 2);
			v.uv.y = 0.5f - 2.0f * (Mathf.Asin(-v.normal.y) / (Mathf.PI * 2));

			return v;
		}
		return vertices[x, y, z];
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

		IntVector3 steps = Steps;

		for (int z = 0; z < steps.z; z++)
		{
			for (int y = 0; y < steps.y; y++)
			{
				for (int x = 0; x < steps.x; x++)
				{
					Vertex v = vertices[x, y, z];
					v.flux = metaBalls.GetVertexValue(v);
					//v.flux = Random.Range(-1.0f, 1.0f);
					if (capped && (z == 0 || x == 0 || y == 0 || x == steps.x - 1 || y == steps.y - 1 || z == steps.z - 1))
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

					vertices[x, y, z] = v;
				}
			}
		}

		for (int z = 0; z < steps.z; z++)
		{
			for (int y = 0; y < steps.y; y++)
			{
				for (int x = 0; x < steps.x; x++)
				{
					Vertex v = vertices[x, y, z];
					Vector3 tangent;

					const float multiplier = 2;
					if (x == 0)
					{
						v.normal.x = (v.flux - vertices[x + 1, y, z].flux) * multiplier;
						tangent.x = (v.pos - vertices[x + 1, y, z].pos).x * multiplier;
					}
					else if (x == steps.x - 1)
					{
						v.normal.x = (vertices[x - 1, y, z].flux - v.flux) * multiplier;
						tangent.x = (vertices[x - 1, y, z].pos - v.pos).x * multiplier;
					}
					else
					{
						v.normal.x = vertices[x - 1, y, z].flux - vertices[x + 1, y, z].flux;
						tangent.x = (vertices[x - 1, y, z].pos - vertices[x + 1, y, z].pos).x;
					}

					if (y == 0)
					{
						v.normal.y = (v.flux - vertices[x, y + 1, z].flux) * multiplier;
						tangent.y = (v.pos - vertices[x, y + 1, z].pos).y * multiplier;
					}
					else if (y == steps.y - 1)
					{
						v.normal.y = (vertices[x, y - 1, z].flux - v.flux) * multiplier;
						tangent.y = (vertices[x, y - 1, z].pos - v.pos).y * multiplier;
					}
					else
					{
						v.normal.y = vertices[x, y - 1, z].flux - vertices[x, y + 1, z].flux;
						tangent.y = (vertices[x, y - 1, z].pos - vertices[x, y + 1, z].pos).y;
					}

					if (z == 0)
					{
						v.normal.z = (v.flux - vertices[x, y, z + 1].flux) * multiplier;
						tangent.z = (v.pos - vertices[x, y, z + 1].pos).z * multiplier;
					}
					else if (z == steps.z - 1)
					{
						v.normal.z = (vertices[x, y, z - 1].flux - v.flux) * multiplier;
						tangent.z = (vertices[x, y, z - 1].pos - v.pos).z * multiplier;
					}
					else
					{
						v.normal.z = vertices[x, y, z - 1].flux - vertices[x, y, z + 1].flux;
						tangent.z = (vertices[x, y, z - 1].pos - vertices[x, y, z + 1].pos).z;
					}

					v.normal.Normalize();
					tangent.Normalize();
					v.tangent = Vector3.Cross(tangent, v.normal);

					//http://en.wikipedia.org/wiki/UV_mapping#Finding_UV_on_a_sphere
					v.uv.x = 0.5f - Mathf.Atan2(-v.normal.z, -v.normal.x) / (Mathf.PI * 2);
					v.uv.y = 0.5f - 2.0f * (Mathf.Asin(-v.normal.y) / (Mathf.PI * 2));

					vertices[x, y, z] = v;
				}
			}
		}
	}

}

[Serializable]
public struct Vertex
{
	public Vector3 pos;
	public float flux;
	public bool inside;
	public Vector3 normal;
	public Vector2 uv;
	public Vector4 tangent;
}


[Serializable]
public struct IntVector3
{
	public readonly int x, y, z;

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