using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class RunController : MonoBehaviour
{


	public Animator animator;                               //Player animator if we using one; Animator is preconfigured for using with idle,run,jump and death animations;
	public enum Controls { JUMP_ONLY, JUMP_AND_SLIDE };     //Input types
	public Controls controls;
	public bool tapToStart;                                 //Tap to start playing;
	public float minSpeed = 2.5F;                           //Player default speed;
	public float maxSpeed = 3.5F;                           //Player max speed;
	public float speedIncreaseFactor = 0.03F;               //Player speed increasing factor;
	public float gravity = 3.0F;                            //Gravity value;
	public float jumpForce = 5.0F;                          //Jump power;
	public bool limitJumps = true;                          //Uncheck this to jump infinite number of times;
	public int maxJumpCount;                                //How much jumps we can do;
	public AudioClip jumpSFX;                               //Jump sound effect if you need one;
	public float RollDuration;                              //How long we will roll in seconds;
	public float RollSpeedDifference;                       //roll speed increase factor;
	public float rollColliderCenter, rollColliderSize;      //collider changes while rolling;
	public AudioClip rollSFX;

	public enum OnDeath { Freeze, DropDown };                   //What to do if we are dead
	public OnDeath onDeath;
	public AudioClip deathSFX;                              //Death sound effect if you need one;
	public DeathEffect deathEffect;
	private Image deathEffectImg;                           //Death effect image;                 

	[HideInInspector]
	public bool play, gameOver;

	private Vector3 velocity;
	private BoxCollider2D mainCollider;
	[SerializeField]
	private float speed, rollDuration, moveSpeed;
	private Rigidbody2D thisRigidbody;
	private bool grounded, sfxOn;
	private int jumpCounter;
	private bool jump, roll, jumpTriggered, rollTriggered, noInputAllowed;
	private bool enteredActionTrigger;
	private bool holdRoll;
	private float defaultGravity, defaultolliderCenter, defaultColliderSize;
	private float hitCount;
	private Color deathEffectColor;

	public delegate void TutorialDelegate(GameManager.ActionDone action);
	public TutorialDelegate ProgressTutorial;

	public float inputDelay = 0.2f;
	public float lastInputTimeStamp;
	public bool lastInputComputer;
	public bool jumpDisabledIsh;

	public class ObservedSaw {
		public GameObject sawGO = null;
		public SawTriggers sawScript = null;
	}

	ObservedSaw observedSaw;

	void Start()
	{
		observedSaw = new ObservedSaw();

		sfxOn = Prefs.GetBool("sfxOn");
		GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfxVol");

		if (!tapToStart)
			play = true;
		else
			play = false;

		speed = minSpeed;
		gameObject.tag = "Player";
		thisRigidbody = GetComponent<Rigidbody2D>();
		thisRigidbody.gravityScale = gravity;
		defaultGravity = gravity;

		if (deathEffect.enable)
		{
			RectTransform canvas = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
			deathEffectImg = new GameObject("DeathEffect", typeof(Image)).GetComponent<Image>();
			deathEffectImg.transform.SetParent(canvas, false);
			deathEffectImg.rectTransform.sizeDelta = new Vector2(canvas.rect.width, canvas.rect.height);

			deathEffectColor = deathEffect.color;
			deathEffectColor.a = 0;
			deathEffectImg.color = deathEffectColor;
		}

		mainCollider = GetComponent<BoxCollider2D>();
		defaultolliderCenter = mainCollider.offset.y;
		defaultColliderSize = mainCollider.size.y;
	}

	void Update()
	{
		if (Time.time - lastInputTimeStamp >= inputDelay)
		{
			bool temp = jumpDisabledIsh;
			jumpDisabledIsh = false;
		}

		//Platform depending controls;

		//TODO: Stuff delay
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8

		foreach (Touch touch in Input.touches)
		{
			if(controls == Controls.JUMP_AND_SLIDE)
			{
				jump = touch.phase == TouchPhase.Began && touch.position.x < Screen.width/2;
				roll = touch.phase == TouchPhase.Began && touch.position.x > Screen.width/2 && rollDuration == 0;
			}
			else
				jump = touch.phase == TouchPhase.Began;
		}
#endif

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER || UNITY_EDITOR
		//TODO: Stuff delay
		jump = false;
		if (!jumpDisabledIsh || !lastInputComputer) {
			jump = Input.GetMouseButtonDown(0) && jumpCounter < maxJumpCount;
		}
			
		roll = Input.GetMouseButtonDown(1) && rollDuration == 0;
#endif

		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);


		if (noInputAllowed)
		{
			jump = false;
			roll = false;
		}

		if(observedSaw.sawGO != null) ObserveSawDistance();

		if (jump)
		{
			jumpDisabledIsh = true;
			lastInputComputer = false;
			lastInputTimeStamp = Time.time;
		}
		else jump = jumpTriggered;

		if (!roll && rollDuration <= 0) roll = rollTriggered;
		if (!roll) roll = holdRoll;

		if (jump)
		{
			rollDuration = 0;
			mainCollider.size = new Vector2(mainCollider.size.x, defaultColliderSize);
			mainCollider.offset = new Vector2(mainCollider.offset.x, defaultolliderCenter);
			jumpTriggered = false;
		}

		//Jumping
		if (!play && (jump || roll))
			play = true;
		else if (jump && jumpCounter < maxJumpCount && !gameOver && play)
		{
			grounded = false;
			moveSpeed = speed;
			if (jumpSFX && sfxOn)
			{
				GetComponent<AudioSource>().clip = jumpSFX;
				GetComponent<AudioSource>().Play();
			}
			thisRigidbody.velocity = new Vector2(thisRigidbody.velocity.x, 0.0F);
			if (limitJumps) jumpCounter++;
			thisRigidbody.AddForce(new Vector2(0.0F, jumpForce * 100));
		}
		else if (play && roll && grounded)
		{
			if (rollSFX && sfxOn)
			{
				GetComponent<AudioSource>().clip = rollSFX;
				GetComponent<AudioSource>().Play();
			}
			rollDuration = RollDuration;
			rollTriggered = false;
			ProgressTutorial(GameManager.ActionDone.Roll);
		}

		//Moving
		if (!gameOver)
		{
			if (play)
			{
				speed += speedIncreaseFactor * Time.deltaTime;
				velocity = new Vector2(moveSpeed, thisRigidbody.velocity.y);
				thisRigidbody.gravityScale = defaultGravity;

				if (rollDuration > 0)
				{
					rollDuration -= 1 * Time.deltaTime;
					mainCollider.size = new Vector2(mainCollider.size.x, rollColliderSize);
					mainCollider.offset = new Vector2(mainCollider.offset.x, rollColliderCenter);
					moveSpeed = speed + RollSpeedDifference;
				}
				else
				{
					rollDuration = 0;
					mainCollider.size = new Vector2(mainCollider.size.x, defaultColliderSize);
					mainCollider.offset = new Vector2(mainCollider.offset.x, defaultolliderCenter);
					moveSpeed = speed;
				}
			}
			else
			{
				velocity = Vector2.zero;
				thisRigidbody.gravityScale = 0.0F;
			}
		}
		else
		{
			if (onDeath == OnDeath.Freeze)
			{
				velocity = Vector2.zero;
				thisRigidbody.gravityScale = 0.0F;
			}
			else
			{
				velocity = new Vector2(0.0F, thisRigidbody.velocity.y);
			}
		}

		//Setting up animator;
		if (animator)
		{
			animator.SetBool("GameOver", gameOver);
			animator.SetBool("Grounded", grounded);
			animator.SetFloat("Speed", Mathf.Abs(thisRigidbody.velocity.x));
			animator.SetBool("Jump", jump);
			animator.SetFloat("RollDuration", rollDuration);
		}

		deathEffectImg.gameObject.SetActive(deathEffectImg.color.a > 0.1F);
	}

	void FixedUpdate()
	{
		thisRigidbody.velocity = velocity;

		if (deathEffect.enable)
		{
			deathEffectImg.color = deathEffectColor;
			if (deathEffectColor.a > 0)
				deathEffectColor.a -= deathEffect.deathEffectSpeed * Time.deltaTime;
		}
	}



	void OnCollisionEnter2D(Collision2D col)
	{
		if (jumpCounter == 1) ProgressTutorial(GameManager.ActionDone.Jump);
		else if (jumpCounter == 2) ProgressTutorial(GameManager.ActionDone.DoubleJump);

		jumpCounter = 0;    //reset jump counter;

		if (col.gameObject.CompareTag("Obstacle") && !gameOver)
		{
			//Draw death effect;
			deathEffectColor.a = 1;

			if (animator) animator.SetTrigger("Death");
			if (deathSFX && sfxOn) { GetComponent<AudioSource>().clip = deathSFX; GetComponent<AudioSource>().Play(); }
			gameOver = true;
			Debug.Log("Death");
		}
		grounded = true;
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		/*if (col.gameObject.CompareTag("NoInput"))
		{
			noInputAllowed = true;
		}*/
		if (!enteredActionTrigger)
		{
			if (col.gameObject.CompareTag("Jump"))
			{
				if (!jumpDisabledIsh || lastInputComputer)
				{
					jumpTriggered = true;
					jumpDisabledIsh = true;
					lastInputComputer = true;
					lastInputTimeStamp = Time.time;

					enteredActionTrigger = true;
				}
			}
			else if (col.gameObject.CompareTag("Roll"))
			{
				rollTriggered = true;
				enteredActionTrigger = true;
			}
			else if (col.gameObject.CompareTag("VerticalSaw"))
			{
				Debug.Log("On est dans le vertical saw");
				
				observedSaw.sawGO = col.gameObject;
				observedSaw.sawScript = col.gameObject.GetComponent<SawTriggers>();

				float distanceX = observedSaw.sawGO.transform.position.x - gameObject.transform.position.x;
				float distanceY = observedSaw.sawGO.transform.position.y - gameObject.transform.position.y;

				Debug.Log("Distance X " + (distanceX));
				Debug.Log("Distance Y " + (distanceY));
				rollTriggered = true;
			}


			if (col.gameObject.CompareTag("HoldRoll"))
			{
				holdRoll = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Jump") || col.gameObject.CompareTag("Roll")) enteredActionTrigger = false;
		else if (col.gameObject.CompareTag("NoInput")) noInputAllowed = false;
		else if (col.gameObject.CompareTag("VerticalSaw")) {
			enteredActionTrigger = false;
			observedSaw.sawGO = null;
		}
		//else if (col.gameObject.CompareTag("HoldRoll")) holdRoll = false;
		holdRoll = false;
	}

	void ObserveSawDistance()
	{
		float distanceX = observedSaw.sawGO.transform.position.x - gameObject.transform.position.x;
		float distanceY = observedSaw.sawGO.transform.position.y - gameObject.transform.position.y;
		

		if (distanceX < 0 || !observedSaw.sawScript.ColliderActive)
		{
			Debug.Log("Bye saw");
			observedSaw.sawGO.SetActive(false);
			observedSaw.sawGO = null;
		}

		//direction vers le bas + un range de distanceY distanceY < 0.39f distanceY > 0.337f
		//Y'a un saute qui a bien fonctionné à 0.7463 :D + un autre
		 //+un à 0.7472
		//0.2852 qui a mal fonctionné :(
		//J'avais essayé à <0.75 et ça avait fail à 0.746363
		else if (distanceY < 0.75f && distanceY > 0.286f) //(distanceX < 0.95f && distanceY < 0.5f && distanceY >0f && distanceX + distanceY > 1.2f)
		{
			Debug.Log("Saute");
			jumpTriggered = true;
			observedSaw.sawGO.SetActive(false);
			Debug.Log("Distance X " + (distanceX));
			Debug.Log("Distance Y " + (distanceY));
		}
		//Ca a marché à 0.751
		//747989
		else if (distanceY < 0.8f && distanceY > 0.25f)//(distanceX < 0.85f && distanceY < 1)
		{
			Debug.Log("Ca roule ma poule");
			rollTriggered = true;
			observedSaw.sawGO.SetActive(false);
			Debug.Log("Distance X " + (distanceX));
			Debug.Log("Distance Y " + (distanceY));
		}

	}


	//Reset speed after restart; we will call it from GameManager Restart function;
	public void Reset()
	{
		speed = minSpeed;
		if (!tapToStart)
			play = true;
		else
			play = false;
	}
}

[System.Serializable]
public class DeathEffect
{
	public bool enable;                     //Enable or disable death effect
	public Color color;                     //Death effect color;
	public float deathEffectSpeed = 3.0F;   //How fast death effect texture should fade out;
}
