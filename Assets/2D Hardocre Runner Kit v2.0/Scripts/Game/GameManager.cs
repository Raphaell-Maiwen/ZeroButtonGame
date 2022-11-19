using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour 
{
	public Transform[] PlayerPrefabs;
	public Transform PlayerSpawnPoint;
	public Transform levelPart;										//Level part prefab; Level prefab should have box collider to count its width for instatiation positioning;
	public Transform levelStartPoint;								//The point where the level starts gerenate;
	public int preloadLevelParts = 6;								//How much level parts load from start;
	public int preloadObstaclesMultiplier = 3;
	public List<obstacles> Obstacles = new List<obstacles>();		//Obstacles settings class;					
	
	public List<obstacles> ObstaclesTest = new List<obstacles>();
	public List<obstacles> ObstaclesTest2 = new List<obstacles>();
	public bool completeTest;

	public ui UI;													//UI class;
	public float scorePerUnit = 1.0F;								//score per unit passed;
	public AudioClip backGroundMusic;								//Background music if you want one;

	private Transform Player;
	private int obstaclesPreloadCount;
	private bool hasAdded;
	private float distanceInterval;
	private float nextDistance;
	private BoxCollider2D col;
	private Transform[] level;
	private Vector3 newObstaclePos;
	private int rnd;
	private int passed;
	private RunController playerControls;
	private float score, musicVol, sfxVol;
	private Color startHintColor;
	private int wholeCointsCount;

	[HideInInspector]public bool musicOn, sfxOn;
	[HideInInspector]public int collectedCoins;

	TutorialStep[] tutorialSteps;
	int tutorialStepIndex = 0;

	bool tutorialOver = false;

	public enum ActionDone { 
		Jump,
		DoubleJump,
		Roll,
	}

	struct TutorialStep {
		public ActionDone action;
		public string text;

		public TutorialStep(ActionDone newAction, string newText) {
			this.action = newAction;
			this.text = newText;
		}
	}

	void Awake()
	{
		gameObject.tag = "GameController";
	}

	// Use this for initialization
	void Start () {
		tutorialSteps = new TutorialStep[] { new TutorialStep(ActionDone.Jump, "Left-click to jump."),
			new TutorialStep(ActionDone.DoubleJump, "Left-click twice to double-jump."), new TutorialStep(ActionDone.Roll, "Right-click to roll.")};

		if (completeTest) ObstaclesTest = ObstaclesTest2;

		GetComponent<AudioSource>().volume = 0;
		obstaclesPreloadCount = ObstaclesTest.Count*preloadObstaclesMultiplier;
		level = new Transform[preloadLevelParts];

		musicOn = Prefs.GetBool("musicOn");
		sfxOn = Prefs.GetBool("sfxOn");

		musicVol = PlayerPrefs.GetFloat("musicVol");
		sfxVol = PlayerPrefs.GetFloat("sfxVol");

		if(PlayerPrefs.HasKey("Coins"))
			wholeCointsCount = PlayerPrefs.GetInt("Coins");

		if(!Player)
		{
			PlayerPrefs.SetString("Char", "RobotDefault");
			Debug.Log(PlayerPrefs.GetString("Char"));
			for (int i = 0; i < PlayerPrefabs.Length; i++)
			{
				if (PlayerPrefabs[i].name == PlayerPrefs.GetString("Char")) {
					Player = (Transform)Instantiate(PlayerPrefabs[i].transform, PlayerSpawnPoint.position, Quaternion.identity);
					break;
				}
			}
		}

		playerControls = Player.GetComponent<RunController>();
		playerControls.ProgressTutorial = NextStepTutorial;
		
		if(levelPart.GetComponent<Collider2D>() != null && levelPart.GetComponent<BoxCollider2D>() == null)
		{
			col = levelPart.gameObject.AddComponent<BoxCollider2D>();
			col.enabled = false;
		}
		else
			col = levelPart.GetComponent<BoxCollider2D>();
		
		distanceInterval = col.size.x;					//distance interval depends on the level part collider size X;
		
		LoadLevel();									//Load level parts;
		//StartCoroutine ("LoadObstacles");				//Load obstacles;


		UI.restrartButton.onClick.AddListener(RestartLevel);
		UI.exitButton.onClick.AddListener(ExitGame);
		UI.menuButton.onClick.AddListener(GoToMenu);

		startHintColor = UI.startHintUI.color;
	}

	void Update () {

		UI.scoreUI.text = "Score:" + score.ToString("F0");
		//UI.coinsUI.text = collectedCoins.ToString ();
		if(Input.GetKey(KeyCode.Escape))Application.Quit();

		if(!playerControls.gameOver && playerControls.play && backGroundMusic && musicOn)
		{
			GetComponent<AudioSource>().clip = backGroundMusic;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, musicVol, 0.3F * Time.deltaTime);

			if (!GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Play();
		}

		//Score settings
		if(playerControls.gameOver)
		{
			if(GetComponent<AudioSource>().clip == backGroundMusic && GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().loop = false;
				GetComponent<AudioSource>().volume = sfxVol;
			}
            else
                GetComponent<AudioSource>().volume = sfxVol;

			if(!PlayerPrefs.HasKey("Score"))
			{
				UI.scoreUI.enabled = false;
				UI.newRecordUI.enabled = true;
				UI.newRecordUI.text = "New record:" + score.ToString("F0");
				PlayerPrefs.SetFloat("Score", score);
			}

			if(PlayerPrefs.GetFloat("Score") < score)
			{
				UI.scoreUI.enabled = false;
				UI.newRecordUI.enabled = true;
				UI.newRecordUI.text = "New record:" + score.ToString("F0");
				PlayerPrefs.SetFloat("Score", score);
			}
			
			UI.restrartButton.gameObject.SetActive(true);
			UI.exitButton.gameObject.SetActive(true);
			UI.menuButton.gameObject.SetActive(true);
		}
		else
		{
			UI.scoreUI.enabled = true;
			UI.newRecordUI.enabled = false;
			UI.restrartButton.gameObject.SetActive(false);
			UI.exitButton.gameObject.SetActive(false);
			UI.menuButton.gameObject.SetActive(false);
		}

		//Level and obstacles reloading depending on player's position;

		
		for (int i = 0; i < level.Length; i++)
		{
			if (Player.position.x > level[i].position.x + distanceInterval * preloadLevelParts / 2)
			{
				level[i].position = new Vector3(levelStartPoint.position.x + nextDistance, levelStartPoint.position.y, levelStartPoint.position.z);
				nextDistance += distanceInterval;
			}
		}

		if (tutorialOver)
		{
			foreach (obstacles obs in ObstaclesTest)
			{
				for (int i = 0; i < obs.preloadedObstacles.Count; i++)
				{
					if (obs.preloadedObstacles[i].position.x <= Player.position.x - 10.0F && !obs.passedObstacles.Contains(obs.preloadedObstacles[i]))
					{
						obs.passedObstacles.Add(obs.preloadedObstacles[i]);
						passed++;
					}
				}

				for (int i = 0; i < obs.passedObstacles.Count; i++)
				{
					//if obstacle type is coins, we getting all child coins, and setting it to active;
					if (obs.passedObstacles[i].childCount > 0)
					{
						for (int c = 0; c < obs.passedObstacles[i].childCount; c++)
							obs.passedObstacles[i].GetChild(c).gameObject.SetActive(true);
					}

					rnd = Random.Range(0, ObstaclesTest.Count);
					for (int x = 0; x < ObstaclesTest[rnd].passedObstacles.Count; x++)
					{
						if (passed == obstaclesPreloadCount / 2 && ObstaclesTest[rnd].passedObstacles.Count == obs.passedObstacles.Count)
						{
							newObstaclePos.y = levelStartPoint.position.y + Random.Range(ObstaclesTest[rnd].Positions.min.y, ObstaclesTest[rnd].Positions.max.y);
							ObstaclesTest[rnd].passedObstacles[i].position = levelStartPoint.position + newObstaclePos;
							ObstaclesTest[rnd].passedObstacles[i].gameObject.SetActive(true);
							newObstaclePos.x += Random.Range(ObstaclesTest[rnd].Positions.min.x, ObstaclesTest[rnd].Positions.max.x);
							ObstaclesTest[rnd].passedObstacles.RemoveAt(i);
							passed--;
						}
					}
				}
			}
		}
		else 
		{
			if (UI.startHintUI)
			{
				if (!playerControls.play)
				{
					UI.startHintUI.enabled = true;

					if (!GetComponent<AudioSource>().isPlaying)
						GetComponent<AudioSource>().volume = 0;
				}
				else
				{
					UI.startHintUI.enabled = false;
					ShowTutorialText();
				}
			}
		}
	}

	void FixedUpdate()
	{
		//adjust score
		if(playerControls.play && !playerControls.gameOver)
			score = (int)Mathf.Abs(PlayerSpawnPoint.position.x - Player.position.x) * scorePerUnit;

		if(!playerControls.play)
			startHintColor.a = Mathf.PingPong(5*Time.time, 1.0F);

		UI.startHintUI.color = startHintColor;
	}

	void RestartLevel()
	{
		PlaySound(UI.clickSound);
		SaveCoins ();
		StartCoroutine("Restart");
	}
	
	void ExitGame()
	{
		PlaySound(UI.clickSound);
		SaveCoins ();
		Application.Quit();
	}

	void GoToMenu()
	{
        PlaySound(UI.clickSound);
		SaveCoins ();
		Application.LoadLevel(0);
	}


	void SaveCoins()
	{
		wholeCointsCount +=  collectedCoins;
		PlayerPrefs.SetInt ("Coins", wholeCointsCount);
	}
	
	//Load level function;
	private void LoadLevel()
	{
		{
			nextDistance = 0.0F;
			for (int i = 0; i < preloadLevelParts; i++)
			{
				nextDistance += distanceInterval;
				level[i] = (Transform)Instantiate(levelPart, new Vector3 (levelStartPoint.position.x + nextDistance, levelStartPoint.position.y, levelStartPoint.position.z), Quaternion.identity);
			}
		}
	}
	
	//Reload level function, we'll use it after level restart; 
	private void ReloadLevel()
	{
		nextDistance = 0.0F;
		for (int i = 0; i < level.Length; i++)
		{
			nextDistance += distanceInterval;
			level[i].position = new Vector3(levelStartPoint.position.x + nextDistance, levelStartPoint.position.y, levelStartPoint.position.z);
		}
		ReloadObstacles ();
	}

	private void ShowTutorialText() {
		UI.tutorialsText.text = tutorialSteps[tutorialStepIndex].text;
	}

	//TODO: Check if tutorial already done
	public void NextStepTutorial(ActionDone action) {
		if (tutorialStepIndex >= tutorialSteps.Length) return;

		if (action == tutorialSteps[tutorialStepIndex].action) {
			tutorialStepIndex++;

			if (tutorialStepIndex >= tutorialSteps.Length)
			{
				UI.tutorialsText.enabled = false;
				tutorialOver = true;
				ReloadLevel();
				Player.position = PlayerSpawnPoint.position;
			}
			else {
				ShowTutorialText();
			}
		}
	}

	//Load obstacles function;
	private IEnumerator LoadObstacles()
	{
		for (int x = 0; x < obstaclesPreloadCount; x++)
		{
			rnd = Random.Range(0, ObstaclesTest.Count);
			newObstaclePos.y = levelStartPoint.position.y + Random.Range(ObstaclesTest[rnd].Positions.min.y, ObstaclesTest[rnd].Positions.max.y);
			ObstaclesTest[rnd].preloadedObstacles.Add((Transform)Instantiate (ObstaclesTest[rnd].obstaclePrefab, levelStartPoint.position
				+ newObstaclePos, Quaternion.identity));
			newObstaclePos.x += Random.Range(ObstaclesTest[rnd].Positions.min.x, ObstaclesTest[rnd].Positions.max.x);
			yield return new WaitForSeconds(0.1F);
		}
	}
	
	//Reload obstacles function for restarting game;
	private void ReloadObstacles()
	{
		passed = 0;
		newObstaclePos.x = 0.0F;
		foreach (obstacles obs in ObstaclesTest)
		{
			for (int i = 0; i < obs.preloadedObstacles.Count; i++)
				Destroy(obs.preloadedObstacles[i].gameObject);

			obs.preloadedObstacles.Clear();
			obs.passedObstacles.Clear();
		}

		StartCoroutine ("LoadObstacles");
	}
	
	//Restart function;
	IEnumerator Restart()
	{
		yield return new WaitForSeconds(0.1F);
		Player.position = PlayerSpawnPoint.position;
		ReloadLevel();
		score = 0.0F; collectedCoins = 0;
		playerControls.gameOver = false;
		playerControls.Reset();
	}
	
	void PlaySound(AudioClip ac)
	{
		if(ac && sfxOn)
		{
			GetComponent<AudioSource>().clip = ac;
			GetComponent<AudioSource>().Play();
		}
	}
}


[System.Serializable]
public class obstacles
{
	public Transform obstaclePrefab;	//Obstacle prefab;
	public positions Positions;			//Obstacle positions class;
	
	[HideInInspector]
	public List<Transform> preloadedObstacles = new List<Transform>();
	[HideInInspector]
	public List<Transform> passedObstacles = new List<Transform>();
	public bool isCoin;
}

//Obstacle position class, min and max avalible obstacle position in X and Y axes;
[System.Serializable]
public class positions
{
	public Vector2 min; 
	public Vector2 max;
}

//UI class;
[System.Serializable]
public class ui
{
	public Text scoreUI, newRecordUI, startHintUI, coinsUI, tutorialsText;
	public Button restrartButton, exitButton, menuButton;
	public AudioClip clickSound;
}