using System;
using UnityEngine;

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