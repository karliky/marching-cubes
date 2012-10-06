using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetaBalls
{

	public List<MetaPoint> metaPoints;
	public float isoValue;
	public bool isInitialized = false;

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

	public Vertex Interpolate(Vertex a, Vertex b)
	{
		float diff = (isoValue - a.flux)/(b.flux - a.flux);

		return new Vertex
			       {
				       pos = a.pos + (b.pos - a.pos)*diff,
					   flux = a.flux + (b.flux - a.flux) * diff,
				       normal = a.normal + (b.normal - a.normal)*diff,
					   uv = a.uv + (b.uv-a.uv) * diff,
					   tangent = a.tangent+(b.tangent-a.tangent)*diff
			       };
	}

	public float GetVertexValue(Vertex v)
	{
		float flux = 0;

		foreach (MetaPoint metaPoint in metaPoints)
		{
			Vector3 length = metaPoint.pos - v.pos;

			flux += Mathf.Abs(metaPoint.power)*metaPoint.power/(length.x*length.x + length.y*length.y + length.z*length.z + 1);
		}

		return flux;
	}
}