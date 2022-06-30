using UnityEngine;
using System.Collections;

public class BackGroundChanger : MonoBehaviour {

	public Color startColor;

	private Color newColor;
	private float time;
	private RunController playerController;

	void Start () {
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<RunController>();
		GetComponent<Camera>().backgroundColor = startColor;
		newColor = startColor;
		InvokeRepeating("ChangeColor", 3, 10);
	}
	
	// Update is called once per frame
	void Update () {

		if(GetComponent<Camera>().backgroundColor != newColor && !playerController.gameOver && playerController.play)
		{
			time += 0.001F * Time.deltaTime;
			GetComponent<Camera>().backgroundColor = Color.Lerp(GetComponent<Camera>().backgroundColor, newColor, time);
		}
		else
			time = 0;
	
	}


	Color GenerateRandomColor()
	{
		Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
		return newColor;
	}

	void ChangeColor()
	{
		newColor = GenerateRandomColor();
	}
}
