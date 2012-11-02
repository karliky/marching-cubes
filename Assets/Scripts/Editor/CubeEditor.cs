using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CubeTest))]
public class CubeEditor : Editor
{
	private bool showWire;

	private CubeTest CubeTest
	{
		get { return target as CubeTest; }
	}

	// Use this for initialization
	private void Start() {}

	private void OnEnable()
	{
		EditorUtility.SetSelectedWireframeHidden(CubeTest.renderer, !showWire);
		ToolsSupport.Hidden = CubeTest.cubes!=null;
	}

	private void OnDisable()
	{
		ToolsSupport.Hidden = false;
	}


	public void OnSceneGUI()
	{
		if(CubeTest.cubes==null)
			return;
		if (Event.current.type == EventType.ValidateCommand)
		{
			if (Event.current.commandName == "UndoRedoPerformed")
			{
				CubeTest.cubes.ComputeMetaBalls();
				EditorUtility.SetDirty(CubeTest.cubes);
			}
		}

		if (CubeTest.cubes.metaBalls != null)
		{
			Color color = Handles.color;
			switch (Tools.current)
			{
				case Tool.Move:
					//MetaBalls.MetaPoint point = CubeTest.cubes.metaBalls.metaPoints[1];
					foreach (MetaPoint point in CubeTest.cubes.metaBalls.metaPoints)
					{
						Vector3 newPos = point.pos;
						Handles.color = Color.green;
						newPos =
							CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos), Vector3.up));
						Handles.color = Color.red;
						newPos =
							CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos), Vector3.right));
						Handles.color = Color.blue;
						newPos =
							CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos),
							                                                        Vector3.forward));


						if ((point.pos - newPos).magnitude > 0.0001f)
						{
							Undo.RegisterUndo(CubeTest.cubes, "Move Metaball");
							point.pos = newPos;
							CubeTest.cubes.ComputeMetaBalls();
							EditorUtility.SetDirty(CubeTest.cubes);
						}
					}
					break;
				case Tool.Scale:
					foreach (MetaPoint point in CubeTest.cubes.metaBalls.metaPoints)
					{
						Vector3 pos = CubeTest.transform.TransformPoint(point.pos);
						float size = HandleUtility.GetHandleSize(pos);
						float power = point.power;
						Handles.color = Color.red;
						power = Handles.ScaleSlider(power, pos, Vector3.right, Quaternion.identity, size, 1.0f);
						Handles.color = Color.green;
						power = Handles.ScaleSlider(power, pos, Vector3.up, Quaternion.identity, size, 1.0f);
						Handles.color = Color.blue;
						power = Handles.ScaleSlider(power, pos, Vector3.forward, Quaternion.identity, size, 1.0f);
						if (Mathf.Abs(point.power - power) > 0.0001f)
						{
							Undo.RegisterUndo(CubeTest.cubes, "Scale Metaball");
							point.power = power;
							CubeTest.cubes.ComputeMetaBalls();
							EditorUtility.SetDirty(CubeTest.cubes);
						}
					}
					break;
			}
			Handles.color = color;

			for (int i = 0; i < CubeTest.cubes.metaBalls.metaPoints.Count; i++)
			{
				MetaPoint point = CubeTest.cubes.metaBalls.metaPoints[i];
				Vector3 pos = CubeTest.transform.TransformPoint(point.pos);
				if (Handles.Button(pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) / 4,
				                   HandleUtility.GetHandleSize(pos) / 8,
				                   Handles.SphereCap))
				{
					Undo.RegisterUndo(CubeTest.cubes, "Delete Metaball");
					CubeTest.cubes.metaBalls.metaPoints.RemoveAt(i);
					i--;
					CubeTest.cubes.ComputeMetaBalls();
					EditorUtility.SetDirty(CubeTest.cubes);
				}
			}
		}
		{
			Vector3 min = CubeTest.cubes.Min;
			Vector3 size = CubeTest.cubes.Size;
			Vector3[] a = new Vector3[5];
			a[0] = min;
			a[1] = min + new Vector3(size.x, 0, 0);
			a[2] = min + new Vector3(size.x, size.y, 0);
			a[3] = min + new Vector3(0, size.y, 0);
			a[4] = a[0];

			Vector3[] b = new Vector3[4];
			b[0] = a[2];
			b[1] = a[2] + new Vector3(0, 0, size.z);
			b[2] = a[2] + new Vector3(-size.x, 0, size.z);
			b[3] = a[3];

			Vector3[] c = new Vector3[4];
			c[0] = b[1];
			c[1] = b[1] + new Vector3(0, -size.y, 0);
			c[2] = b[1] + new Vector3(-size.x, -size.y, 0);
			c[3] = b[2];

			for (int i = 0; i < a.Length; i++)
				a[i] = CubeTest.transform.TransformPoint(a[i]);
			for (int i = 0; i < b.Length; i++)
				b[i] = CubeTest.transform.TransformPoint(b[i]);
			for (int i = 0; i < c.Length; i++)
				c[i] = CubeTest.transform.TransformPoint(c[i]);

			Handles.DrawPolyLine(a);
			Handles.DrawPolyLine(b);
			Handles.DrawPolyLine(c);
			Handles.DrawLine(c[1], a[1]);
			Handles.DrawLine(c[2], a[0]);
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (CubeTest.cubes == null)
			return;
		if (GUILayout.Button("New Metaball"))
		{
			Undo.RegisterUndo(CubeTest.cubes, "Add Metaball");
			CubeTest.cubes.metaBalls.metaPoints.Add(new MetaPoint { pos = Vector3.zero, power = 1.0f });
			CubeTest.cubes.ComputeMetaBalls();
			EditorUtility.SetDirty(CubeTest.cubes);
		}

		EditorGUI.BeginChangeCheck();
		showWire = EditorGUILayout.Toggle("Show Wireframe", showWire);
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetSelectedWireframeHidden(CubeTest.renderer, !showWire);
			EditorUtility.SetDirty(CubeTest.cubes);
		}
		bool newCap = EditorGUILayout.Toggle("Cap", CubeTest.cubes.capped);
		if (newCap != CubeTest.cubes.capped)
		{
			Undo.RegisterUndo(CubeTest.cubes, "Toggle Capping");
			CubeTest.cubes.capped = newCap;
			CubeTest.cubes.ComputeMetaBalls();
			EditorUtility.SetDirty(CubeTest.cubes);
		}

		IntVector3 steps = EditorGUILayout.Vector3Field("Cube Steps", CubeTest.cubes.Steps);
		if (!steps.Same(CubeTest.cubes.Steps))
		{
			Undo.RegisterUndo(CubeTest.cubes, "Change Step Size");
			CubeTest.cubes.Steps = steps;
			CubeTest.cubes.ComputeMetaBalls();
			EditorUtility.SetDirty(CubeTest.cubes);
		}

		Vector3 min = EditorGUILayout.Vector3Field("Cube Min", CubeTest.cubes.Min);
		if ((min - CubeTest.cubes.Min).sqrMagnitude > Mathf.Epsilon)
		{
			Undo.RegisterUndo(CubeTest.cubes, "Change Cube Dimensions");
			CubeTest.cubes.Min = min;
			CubeTest.cubes.ComputeMetaBalls();
			EditorUtility.SetDirty(CubeTest.cubes);
		}

		Vector3 max = EditorGUILayout.Vector3Field("Cube Max", CubeTest.cubes.Max);
		if ((max - CubeTest.cubes.Max).sqrMagnitude > Mathf.Epsilon)
		{
			Undo.RegisterUndo(CubeTest.cubes, "Change Cube Dimensions");
			CubeTest.cubes.Max = max;
			CubeTest.cubes.ComputeMetaBalls();
			EditorUtility.SetDirty(CubeTest.cubes);
		}
	}

	// Update is called once per frame
	private void Update() {}
}

public class ToolsSupport
{
	public static bool Hidden
	{
		get
		{
			Type type = typeof (Tools);

			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);

			return field != null && ((bool) field.GetValue(null));
		}

		set
		{
			Type type = typeof (Tools);

			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);

			if (field != null) field.SetValue(null, value);
		}
	}
}