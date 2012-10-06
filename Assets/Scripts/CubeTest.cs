using System;
using System.Collections.Generic;
//using System.Threading;
//using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshFilter), typeof (MeshRenderer))]
public class CubeTest : MonoBehaviour
{
	public MarchingCubes cubes = new MarchingCubes(new Vector3(-10.5f, -10.5f, -10.5f), new Vector3(10.5f, 10.5f, 10.5f),
	                                               new Vector3(30, 30, 30), 0.2f);


	public float movement;
	public float iso = 0.2f;

	private void OnEnable()
	{
        if(renderer.sharedMaterial==null)
            renderer.sharedMaterial = new Material(Shader.Find("Custom/TriPlanar"));

		movement = 0;

		ReDraw();
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			movement += Time.deltaTime;
			if(cubes.metaBalls.metaPoints.Count>0)
				cubes.metaBalls.MoveBall(0, new Vector3(Mathf.Cos(movement), Mathf.Sin(movement), Mathf.Cos(movement * 2)) * 0.1f);
			if (cubes.metaBalls.metaPoints.Count > 1)
				cubes.metaBalls.MoveBall(1, new Vector3(Mathf.Sin(movement), Mathf.Cos(movement), Mathf.Sin(movement*2))*0.1f);

			ReDraw();
		}
		if (Math.Abs(iso - cubes.iso) > 0.0001f)
		{
			cubes.iso = iso;
			ReDraw();
		}
		//if(taskQueued&&taskDone)
		//{
		//	taskQueued = false;
		//}
	}

	private void Mesher()
	{
		//while (meshThread!=null)
		{
			//if(taskQueued&&!taskDone)
			{
				List<Vector3> vertices = new List<Vector3>();
				List<Vector3> normals = new List<Vector3>();
				List<Vector4> tangents = new List<Vector4>();
				List<Vector2> uvs = new List<Vector2>();
				cubes.ComputeMetaBalls();
				cubes.Draw(vertices, normals, uvs, tangents);


				//int childCount = transform.childCount;
				//for (int i = childCount-1; i >= 0; i--)
				//{
				//	DestroyImmediate(transform.GetChild(i).gameObject);
				//}

				const int curIndex = 0;
				//int index = 0;

				const int maxCount = 60000;
				if(curIndex<=vertices.Count)
				//EditorApplication.delayCall += () =>
				{
				    GameObject nextChild = gameObject;// new GameObject("mesh " + index);

					int count = Mathf.Min(vertices.Count - curIndex, maxCount);

					int[] indices = new int[count];
					for (int i = 0; i < count; i++)
					{
						indices[i] = i;
					}

                    if(nextChild.GetComponent<MeshFilter>().sharedMesh)
                        DestroyImmediate(nextChild.GetComponent<MeshFilter>().sharedMesh);

					nextChild.GetComponent<MeshFilter>().sharedMesh = new Mesh
						                                        {
																	vertices = vertices.GetRange(curIndex,count).ToArray(),
							                                        triangles = indices,
																	normals = normals.GetRange(curIndex, count).ToArray(),
																	tangents = tangents.GetRange(curIndex, count).ToArray(),
																	uv = uvs.GetRange(curIndex, count).ToArray()
						                                        };

                    //nextChild.GetComponent<MeshRenderer>().sharedMaterial = renderer.sharedMaterial;
					//nextChild.transform.parent = transform;

					//index++;
					//curIndex += maxCount;
					//taskDone = true;
				} //;
			}
			//Thread.Sleep(0);
		}
	}

	//private bool taskQueued, taskDone;

	//private Thread meshThread;

	public void ReDraw()
	{
		Mesher();
		//if(meshThread==null)
		//{
		//meshThread = new Thread(Mesher);
		//meshThread.Start();
		//}

		//if(!taskQueued)
		//{
		//	taskDone = false;
		//	taskQueued = true;
		//}
	}
}