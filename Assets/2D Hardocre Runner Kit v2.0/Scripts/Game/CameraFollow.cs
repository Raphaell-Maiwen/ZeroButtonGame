using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	
	public enum FollowAxes{ X, XY }
	public FollowAxes followAxes;
	public Vector2 offset;
	
	private Transform thisTransform, playerTransform;
	private RunController playerController;
	
	// Use this for initialization
	void Start () 
	{
		thisTransform = transform;
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<RunController>();
		playerTransform = playerController.transform;
	}
	
	void Update () 
	{
		if(!playerController.gameOver)
		{
			if(followAxes == FollowAxes.X) 
				thisTransform.position = new Vector3(playerTransform.position.x + offset.x, offset.y, thisTransform.position.z);
			else 
				thisTransform.position = new Vector3(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y, thisTransform.position.z);
		}
	}
}
