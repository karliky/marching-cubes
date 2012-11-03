using System;
using UnityEngine;

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