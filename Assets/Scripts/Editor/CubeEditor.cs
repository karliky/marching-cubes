using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubeTest))]
public class CubeEditor : Editor
{
	private bool showWire;
	private CubeTest CubeTest
	{
		get { return target as CubeTest; }
	}
	// Use this for initialization
	void Start () {
	
	}

	private void OnEnable()
	{
		EditorUtility.SetSelectedWireframeHidden(CubeTest.renderer, !showWire);
		ToolsSupport.Hidden = true;
	}

	private void OnDisable()
	{
		ToolsSupport.Hidden = false;
	}
	

	public void OnSceneGUI()
	{
		if(Event.current.type==EventType.ValidateCommand) {
			if (Event.current.commandName=="UndoRedoPerformed") {
				CubeTest.ReDraw();
				EditorUtility.SetDirty(CubeTest);
			}
		}

        if (CubeTest.cubes.metaBalls!=null)
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
						newPos = CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos), Vector3.up));
						Handles.color = Color.red;
						newPos = CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos), Vector3.right));
						Handles.color = Color.blue;
						newPos = CubeTest.transform.InverseTransformPoint(Handles.Slider(CubeTest.transform.TransformPoint(newPos), Vector3.forward));
						

						if ((point.pos - newPos).magnitude > 0.0001f)
						{
							Undo.RegisterUndo(CubeTest, "Move Metaball");
							point.pos = newPos;
							CubeTest.ReDraw();
							EditorUtility.SetDirty(CubeTest);
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
								Undo.RegisterUndo(CubeTest, "Scale Metaball");
								point.power = power;
								CubeTest.ReDraw();
								EditorUtility.SetDirty(CubeTest);
							}
						}
				    break;
			}
			Handles.color = color;
			
		}
		
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if(GUILayout.Button("New Metaball"))
		{
			Undo.RegisterUndo(CubeTest,"Add Metaball");
			CubeTest.cubes.metaBalls.metaPoints.Add(new MetaPoint {pos=Vector3.zero,power = 1.0f});
			CubeTest.ReDraw();
			EditorUtility.SetDirty(CubeTest);
		}

		EditorGUI.BeginChangeCheck();
		showWire = EditorGUILayout.Toggle("Show Wireframe", showWire);
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetSelectedWireframeHidden(CubeTest.renderer, !showWire);
			EditorUtility.SetDirty(CubeTest);
		}
		bool newCap = EditorGUILayout.Toggle("Cap",CubeTest.cubes.capped);
		if(newCap!=CubeTest.cubes.capped) {
			Undo.RegisterUndo(CubeTest,"Toggle Capping");
			CubeTest.cubes.capped = newCap;
			CubeTest.ReDraw();
			EditorUtility.SetDirty(CubeTest);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}

public class ToolsSupport {

 

    public static bool Hidden {

        get {

            Type type = typeof (Tools);

            FieldInfo field = type.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);

            return field != null && ((bool) field.GetValue (null));

        }

        set {

            Type type = typeof (Tools);

            FieldInfo field = type.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);

	        if (field != null) field.SetValue (null, value);
        }

    }

}