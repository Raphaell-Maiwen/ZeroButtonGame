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

	public bool goingUp;

	public bool isVerticalSaw;

	public List<UnityAction> updateRollTriggers = new List<UnityAction>();
	public List<UnityAction> disableRollTriggers = new List<UnityAction>();
	public List<UnityAction> updateBunchTriggers = new List<UnityAction>();
	public List<UnityAction> disableBunchTriggers = new List<UnityAction>();

	public float previousX;

	void Start () {
		thisTransform = transform;
		point = targetPoint-1;

		if (point == 0) goingUp = true;

		previousX = this.transform.position.x;
	}

	//Why FixedUpdate and not Update?
	void Update () 
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

		if (isVerticalSaw) {
			if (previousX != this.transform.position.x) {
				previousX = this.transform.position.x;

				ResetTriggers();
			}

			if (!goingUp && moveDir.y > 0) goingUp = !goingUp;
			else if ((goingUp && moveDir.y < 0))
			{
				goingUp = !goingUp;
				for (int i = 0; i < updateRollTriggers.Count; i++)
				{
					updateRollTriggers[i]();
				}
			}

			if (!goingUp && this.transform.position.y < 1.5)
			{
				for (int i = 0; i < updateBunchTriggers.Count; i++)
				{
					updateBunchTriggers[i]();
				}
			}
			else if (goingUp && this.transform.position.y > 1)
			{
				for (int i = 0; i < disableBunchTriggers.Count; i++)
				{
					disableBunchTriggers[i]();
				}
			}
		}
		else if ((goingUp && moveDir.x > 0) || (!goingUp && moveDir.x < 0))
		{
			goingUp = !goingUp;
			for (int i = 0; i < updateRollTriggers.Count; i++)
			{
				updateRollTriggers[i]();
			}
		}

		if (moveDir.x < 0)
			newRotationSpeed = rotationSpeed;
		else
			newRotationSpeed = -rotationSpeed;

		transform.Rotate(Vector3.forward * newRotationSpeed * 100 * Time.deltaTime);
	}





	void ResetTriggers() {
		for (int i = 0; i < disableBunchTriggers.Count; i++)
		{
			disableBunchTriggers[i]();
			disableRollTriggers[i]();
		}

		if (!goingUp && moveDir.y < 0) {
			if (this.transform.position.y < 1.5)
			{
				for (int i = 0; i < updateBunchTriggers.Count; i++)
				{
					updateBunchTriggers[i]();
				}
			}
			else {
				for (int i = 0; i < updateRollTriggers.Count; i++)
				{
					updateRollTriggers[i]();
				}
			}
		}
	}
}
