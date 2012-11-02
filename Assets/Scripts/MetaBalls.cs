using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetaBalls
{
	public List<MetaPoint> metaPoints;
	public float isoValue;
	public bool isInitialized;

	public void Initialize()
	{
		if (isInitialized)
			return;

		metaPoints = new List<MetaPoint>
		             {
		             	new MetaPoint
		             	{
		             		power = 1.5f,
		             		pos = new Vector3(0, -2, 0)
		             	},
		             	new MetaPoint
		             	{
		             		power = 1.5f,
		             		pos = new Vector3(-4.0f, 2.0f, 4.0f)
		             	}
		             };
	}

	public void MoveBall(int idx, Vector3 delta)
	{
		metaPoints[idx].pos = metaPoints[idx].pos + delta;
	}

	public float GetVertexValue(Vertex v)
	{
		float flux = 0;

		foreach (MetaPoint metaPoint in metaPoints)
		{
			Vector3 length = metaPoint.pos - v.pos;

			flux += Mathf.Abs(metaPoint.power) * metaPoint.power /
			        (length.x * length.x + length.y * length.y + length.z * length.z + 1);
		}

		return flux;
	}
}