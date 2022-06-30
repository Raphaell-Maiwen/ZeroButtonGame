using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public static class GizmoText
{

	public static void DrawTextBox(Vector3 position, Vector2 Offset, float distance, float RectSizeX, float RectSizeY, string text, int TextSize, Color color)
	{
		Handles.BeginGUI();
		Vector3 newPos = new Vector3(position.x + Offset.x, position.y + Offset.y, position.z);
		
		Vector3 guiPoint = HandleUtility.WorldToGUIPoint(newPos);
		GUIStyle skin = new GUIStyle();
		skin.fontStyle = FontStyle.Bold;
		skin.normal.textColor = color;
		skin.fontSize = TextSize;
		skin.alignment = TextAnchor.MiddleCenter;
		Rect rect = new Rect(guiPoint.x - RectSizeX/2, guiPoint.y - RectSizeY/2, RectSizeX, RectSizeY);
		
		Vector3 oncam = Camera.current.WorldToScreenPoint (newPos);
		if (oncam.x >= 0 && oncam.x <= Camera.current.pixelWidth && oncam.y >= 0 && 
		    oncam.y <= Camera.current.pixelHeight && oncam.z > 0 && oncam.z < distance)
		{
			GUI.Box(rect, text, skin);
		}
		
		Handles.EndGUI();
	}

	public static void DrawPoint(Vector3 targetTransform, float size)
	{
		Handles.DrawWireArc(targetTransform, Vector3.forward, Vector3.left, 360,  size);
	}
}