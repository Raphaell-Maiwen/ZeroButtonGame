using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MovingObstacle))]
public class _MovingObstacle : Editor {

	MovingObstacle myTarget;
	private bool showWaypoints = true;
	
	void OnEnable()
	{
		myTarget = (MovingObstacle) target;

		if(myTarget.transform.parent == null)
		{
			string Name = myTarget.name.ToUpper();
			GameObject parent = new GameObject(Name);
			parent.transform.position = myTarget.transform.position;
			myTarget.transform.parent = parent.transform;
			myTarget.transform.localPosition = Vector3.zero;
		}
	}

	void OnSceneGUI()
	{
		if(myTarget.wayPoint.Count != 0)
		{
			for(int i = 0; i < myTarget.wayPoint.Count; i++)
			{
				GizmoText.DrawPoint(myTarget.wayPoint[i].position, 0.2F);
				GizmoText.DrawTextBox(myTarget.wayPoint[i].position, Vector2.zero, 35, 30, 30, (i+1).ToString(), 15, Color.white);
			}
		}
	}


	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginVertical("Box");
			GUILayout.Space(5);
			
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);
				GUILayout.Space(10);
				
				showWaypoints = EditorGUILayout.Foldout(showWaypoints,"WayPoints");
				var foldoutStyle = new GUIStyle(EditorStyles.toolbarButton);
				foldoutStyle.fontSize = 10;

				GUILayout.FlexibleSpace();
				
				GUIContent addContent;
				var addIcon = "+";
				addContent = new GUIContent(addIcon, "Add new level");

				if(GUILayout.Button(addContent, foldoutStyle, GUILayout.Width(35)))
				{
					CreateWayPoint();
					showWaypoints = true;
				}
			EditorGUILayout.EndHorizontal();

			for(int i = 0; i < myTarget.wayPoint.Count; i++)
			{
				if(showWaypoints)
				{
					EditorGUILayout.BeginHorizontal();
						GUILayout.Space(10);
						EditorGUILayout.BeginVertical("Box");

							myTarget.wayPoint[i].name = (i+1) + ".WayPoint";
							EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);
								EditorGUILayout.ObjectField("", myTarget.wayPoint[i].gameObject, typeof(GameObject), true, GUILayout.Width(120));
								GUILayout.FlexibleSpace();

								addIcon = "X";
								addContent = new GUIContent(addIcon, "Delete Waypoint");
								if(GUILayout.Button(addContent, foldoutStyle, GUILayout.Width(35)))
								{
									DestroyImmediate(myTarget.wayPoint[i].gameObject);
									myTarget.wayPoint.RemoveAt(i);
								}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
								GUILayout.Label("Waypoint " +(i+1)+ " Position", EditorStyles.boldLabel);
								GUILayout.FlexibleSpace();
							EditorGUILayout.EndHorizontal();
							
							Vector2 pos = EditorGUILayout.Vector2Field("", myTarget.wayPoint[i].localPosition);
							if (GUI.changed)
							{
								Undo.RecordObject(myTarget, "Transform Change");
								myTarget.wayPoint[i].localPosition = WayPointPosition(pos);
							}
						EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
				}
			}
			if(myTarget.wayPoint.Count > 0)
			{
				GUILayout.Space(5);
				myTarget.moveSpeed = EditorGUILayout.FloatField("Move Speed", myTarget.moveSpeed);
				myTarget.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", myTarget.rotationSpeed);
				myTarget.targetPoint = EditorGUILayout.IntField("Start Waypoint", myTarget.targetPoint);
				myTarget.targetPoint = Mathf.Clamp(myTarget.targetPoint, 1, myTarget.wayPoint.Count);
			}
			GUILayout.Space(5);
		EditorGUILayout.EndVertical();

		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}


	void CreateWayPoint()
	{
		GameObject waypoint = new GameObject();
		waypoint.transform.parent = myTarget.transform.parent;
		waypoint.transform.localPosition = Vector3.zero;
		myTarget.wayPoint.Add(waypoint.transform);
	}
	


	
	private Vector2 WayPointPosition(Vector2 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		return v;
	}
}
