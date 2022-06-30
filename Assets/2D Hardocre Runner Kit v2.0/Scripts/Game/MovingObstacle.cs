using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class MovingObstacle : MonoBehaviour {
	public float moveSpeed = 3, rotationSpeed = 25;
	public List<Transform> wayPoint = new List<Transform>();
	public int point, targetPoint;
	public Vector3 moveDir;
	public float distance;
	private float newRotationSpeed;
	private Transform thisTransform;

	bool goingUp;

	public List<UnityAction> updateSawTriggers = new List<UnityAction>();

	void Start () {
		thisTransform = transform;
		point = targetPoint-1;

		if (point == 0) goingUp = true;
	}

	void FixedUpdate () 
	{
		if (point < wayPoint.Count && wayPoint.Count >= 1)
		{
			moveDir = wayPoint[point].position - transform.position;

			distance = moveDir.magnitude;

			if (distance < 0.1F)
				point++;
			else
				thisTransform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.World);
		}
		else {
			point = 0;
		}

		if ((goingUp && moveDir.y < 0) ||(!goingUp && moveDir.y > 0)) {
			goingUp = !goingUp;
			for (int i = 0; i < updateSawTriggers.Count; i++) {
				updateSawTriggers[i]();
			}
		}

		if(moveDir.x < 0)
			newRotationSpeed = rotationSpeed;
		else
			newRotationSpeed = -rotationSpeed;

		transform.Rotate(Vector3.forward * newRotationSpeed * 100 * Time.deltaTime);
	}
}
