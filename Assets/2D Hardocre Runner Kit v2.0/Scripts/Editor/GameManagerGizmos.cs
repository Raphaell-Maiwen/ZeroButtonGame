using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(GameManager))]

public class GameManagerGizmos : Editor {
	

	void OnSceneGUI()
	{
		GameManager myTarget = (GameManager)target;

		float gizmosOffset = 0;

		for (int i = 0; i < myTarget.ObstaclesTest.Count; i++)
		{
			string minText = myTarget.ObstaclesTest[i].obstaclePrefab.name + " Min: " + (myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.min.y).ToString() + ";";
			string maxText = myTarget.ObstaclesTest[i].obstaclePrefab.name + " Max: " + (myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.max.y).ToString() + ";";

				

			Vector3 maxPos = new Vector3(myTarget.levelStartPoint.position.x, myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.max.y,
			                             myTarget.levelStartPoint.position.z);
			Vector3 minPos = new Vector3(myTarget.levelStartPoint.position.x + 0, myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.min.y,
			                             myTarget.levelStartPoint.position.z);

			if(Mathf.Abs (myTarget.ObstaclesTest[i].Positions.max.y - myTarget.ObstaclesTest[i].Positions.min.y) > 0.05)
				GizmoText.DrawTextBox(maxPos, new Vector2(gizmosOffset, 0), 50, 100, 20, maxText, 7, Color.green);
			else
				minText = myTarget.ObstaclesTest[i].obstaclePrefab.name + " Min-Max: " + "\n" + (myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.min.y).ToString() + " - " + (myTarget.levelStartPoint.position.y + myTarget.ObstaclesTest[i].Positions.max.y).ToString() + ";";

			GizmoText.DrawTextBox(minPos, new Vector2(gizmosOffset, 0), 50, 100, 20, minText, 7, Color.green);

			gizmosOffset += 3.5F;
		}

		Vector2 offset = new Vector2(myTarget.levelPart.GetComponent<BoxCollider2D>().size.x, -0.2F);
		GizmoText.DrawPoint(new Vector3(myTarget.levelStartPoint.position.x + offset.x, myTarget.levelStartPoint.position.y, myTarget.levelStartPoint.position.z), 0.05F);
		GizmoText.DrawTextBox(myTarget.levelStartPoint.position, offset, 50, 150, 20,
		                      "Level and obstacles start point", 8, Color.white);

		GizmoText.DrawTextBox(myTarget.PlayerSpawnPoint.position, Vector2.zero, 50, 150, 20,
		                      "SPAWN POINT", 8, Color.white);
	}	
}
