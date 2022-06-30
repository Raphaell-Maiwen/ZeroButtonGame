using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour {

	public int gameLevelIndex;
	public Button PlayButton, ExitButton, SettingsButton, ShopButton;
	public Text BestScore;

	public Settings settings;
	public Shop shop;
	public Sound sound;

	private bool isSettings, isShop;
	private EventTrigger shopTrigger;
	private float scrollStep, nextScrollValue, touchPos, swipeDistance;
	private bool isDraging;
	private int curCharacter, coins;

	// Use this for initialization
	void Start () 
	{

		//Load best score;
		if(PlayerPrefs.HasKey("Score") && PlayerPrefs.GetFloat("Score") > 0)
		{
			BestScore.enabled = true;
			BestScore.text = "Best Score: " + PlayerPrefs.GetFloat("Score").ToString("F0");
		}
		else
			BestScore.enabled = false;

		//Assign buttons listeners to load functions we need.
		PlayButton.onClick.AddListener(LoadLevel);
		ExitButton.onClick.AddListener(ExitGame);
		SettingsButton.onClick.AddListener(GoToSettings);
		ShopButton.onClick.AddListener (GoToShop);

		settings.BackButton.onClick.AddListener(()=> GoToMenu("settings"));
		shop.backButton.onClick.AddListener (()=> GoToMenu("shop"));
		shop.buyButton.onClick.AddListener (Buy);

		//Load audiosettings
		if(!PlayerPrefs.HasKey("musicOn"))
			Prefs.SetBool("musicOn", true);
		if(!PlayerPrefs.HasKey("sfxOn"))
			Prefs.SetBool("sfxOn", true);

		settings.MusicToogle.isOn = Prefs.GetBool("musicOn");
		settings.SFXToogle.isOn = Prefs.GetBool("sfxOn");

		if(!PlayerPrefs.HasKey("musicVol"))
			PlayerPrefs.SetFloat("musicVol", 1);

		if(!PlayerPrefs.HasKey("sfxVol"))
			PlayerPrefs.SetFloat("sfxVol", 1);


		settings.MusicVolSlider.value = PlayerPrefs.GetFloat("musicVol");
		settings.SFXVolSlider.value = PlayerPrefs.GetFloat("sfxVol");

		if(sound.BackgroundMusicPlayer && sound.backGroundMusic)
		{
			sound.BackgroundMusicPlayer.loop = true;
			sound.BackgroundMusicPlayer.GetComponent<AudioSource>().clip = sound.backGroundMusic;
		}
		else if(settings.MusicToogle.isOn)
			sound.BackgroundMusicPlayer.GetComponent<AudioSource>().Play();

		swipeDistance = shop.swipeArea.rectTransform.sizeDelta.x / 3;
		shop.scrollbar.value = 0;
		scrollStep = (float)1 / (shop.chatacters.Length - 1);
		SwipeCharacters ();

		//Assign default default player prefab name for ingame instatiation;
		if(!PlayerPrefs.HasKey("Char"))
		{
			for(int i = 0; i < shop.chatacters.Length; i++)
			{
				if(shop.chatacters[i].isSelected)
					PlayerPrefs.SetString("Char", shop.chatacters[i].prefab.name);
			}
		}
	}


	void Update()
	{
		settings.SettingsPanel.SetActive(isSettings);
		shop.shopPanel.SetActive (isShop);

		settings.MusicVolSlider.interactable = settings.MusicToogle.isOn;
		sound.BackgroundMusicPlayer.volume = settings.MusicVolSlider.value;

		if(settings.MusicToogle.isOn)
		{
			if(!sound.BackgroundMusicPlayer.GetComponent<AudioSource>().isPlaying)
				sound.BackgroundMusicPlayer.GetComponent<AudioSource>().Play();
		}
		else
			sound.BackgroundMusicPlayer.GetComponent<AudioSource>().Pause();


		settings.SFXVolSlider.interactable = settings.SFXToogle.isOn;
		sound.SFXPlayer.volume = settings.SFXVolSlider.value;

		BuyInfo ();
		shop.coinsUI.text = coins.ToString();
	}

	void FixedUpdate () {
		
		if(isDraging)
			return;
		
		if(shop.scrollbar.value != nextScrollValue)
		{
			shop.scrollbar.value = Mathf.Lerp(shop.scrollbar.value, nextScrollValue, 0.35F);
		}
	}
	
	void LoadLevel()
	{
		PlaySound(sound.clickSound);
		Application.LoadLevel(gameLevelIndex);
	}

	void ExitGame()
	{
		PlaySound(sound.clickSound);
		Application.Quit();
	}

	void GoToSettings()
	{
		PlaySound(sound.clickSound);
		isSettings = !isSettings;
	}

	void GoToShop()
	{
		PlaySound(sound.clickSound);
		isShop = !isShop;
		LoadShop ();
	}

	void GoToMenu(string from)
	{
		PlaySound(sound.clickSound);
		if(from == "settings")
		{
			SaveSettings();
			isSettings = false;
		}
		else
		{
			SaveShop();
			isShop = false;
		}
	}

	void PlaySound(AudioClip ac)
	{
		if(ac && settings.SFXToogle.isOn)
		{
			GetComponent<AudioSource>().clip = ac;
			GetComponent<AudioSource>().Play();
		}
	}

	void SwipeCharacters()
	{
		shopTrigger = shop.swipeArea.gameObject.GetComponent<EventTrigger>();
		
		var t = new EventTrigger.TriggerEvent();
		t.AddListener( data => 
		              {
			var evData = (PointerEventData)data;
			data.Use();
			touchPos = evData.position.x;
			
			isDraging = true;
		});
		
		shopTrigger.triggers.Add(new EventTrigger.Entry{callback = t, eventID = EventTriggerType.PointerDown});
		
		t = new EventTrigger.TriggerEvent();
		t.AddListener( data => 
		              {
			var evData = (PointerEventData)data;
			data.Use();
			
			if(evData.position.x > touchPos + swipeDistance)
			{
				if(nextScrollValue > 0)
					nextScrollValue -= scrollStep;
				else
					nextScrollValue = 0;

				if(curCharacter > 0)
					curCharacter --;
			}
			else if(evData.position.x < touchPos - swipeDistance)
			{
				if(nextScrollValue < 1)
					nextScrollValue += scrollStep;
				else
					nextScrollValue = 1;

				if(curCharacter < shop.chatacters.Length-1)
					curCharacter ++;
			}
			
			isDraging = false;
		});
		
		shopTrigger.triggers.Add(new EventTrigger.Entry{callback = t, eventID = EventTriggerType.PointerUp});
	}

	//Changing Buy button
	void BuyInfo()
	{
		if(shop.chatacters[curCharacter].bought && shop.chatacters[curCharacter].isSelected)
		{
			shop.buyButtonText.text = "Selected";
			shop.buyButton.interactable = false;
		}
		else if(shop.chatacters[curCharacter].bought && !shop.chatacters[curCharacter].isSelected)
		{
			shop.buyButtonText.text = "Select";
			shop.buyButton.interactable = true;
		}
		else if(!shop.chatacters[curCharacter].bought)
		{
			shop.buyButtonText.text = "Unlock " + "("+shop.chatacters[curCharacter].cost+")";
			if(coins > shop.chatacters[curCharacter].cost)
				shop.buyButton.interactable = true;
			else
				shop.buyButton.interactable = false;
		}
	}

	//Buy character logic;
	void Buy()
	{
		if(shop.chatacters[curCharacter].bought && !shop.chatacters[curCharacter].isSelected)
		{
			shop.buyButtonText.text = "Select";
			for(int i = 0; i < shop.chatacters.Length; i++)
				shop.chatacters[i].isSelected = false;

			shop.chatacters[curCharacter].isSelected = true;
		}
		else if(!shop.chatacters[curCharacter].bought)
		{
			coins -= shop.chatacters[curCharacter].cost;
			shop.chatacters[curCharacter].bought = true;
			for(int i = 0; i < shop.chatacters.Length; i++)
				shop.chatacters[i].isSelected = false;
			shop.chatacters[curCharacter].isSelected = true;
		}

		PlaySound(sound.clickSound);
	}

	//Load shop settings
	void LoadShop()
	{
		if(PlayerPrefs.HasKey("Coins"))
			coins = PlayerPrefs.GetInt("Coins");

		for(int i = 0; i < shop.chatacters.Length; i++)
		{
			if(PlayerPrefs.HasKey (shop.chatacters[i].prefab.name + i + "s"))
				shop.chatacters[i].isSelected = Prefs.GetBool(shop.chatacters[i].prefab.name + i + "s");
			if(PlayerPrefs.HasKey(shop.chatacters[i].prefab.name+ i + "b"))
				shop.chatacters[i].bought = Prefs.GetBool(shop.chatacters[i].prefab.name+ i + "b");

			if (shop.chatacters[i].isSelected)
				curCharacter = i;
		}

		nextScrollValue = curCharacter * scrollStep;
	}

	//Save shop settings
	void SaveShop()
	{
		for(int i = 0; i < shop.chatacters.Length; i++)
		{
			Prefs.SetBool(shop.chatacters[i].prefab.name + i + "s", shop.chatacters[i].isSelected);
			Prefs.SetBool(shop.chatacters[i].prefab.name+ i + "b", shop.chatacters[i].bought);

			if(shop.chatacters[i].isSelected)
				PlayerPrefs.SetString("Char", shop.chatacters[i].prefab.name);
		}

		PlayerPrefs.SetInt("Coins", coins);
	}

	//Save audio settings
	void SaveSettings()
	{
		Prefs.SetBool("musicOn", settings.MusicToogle.isOn);
		Prefs.SetBool("sfxOn", settings.SFXToogle.isOn);
		PlayerPrefs.SetFloat("musicVol", settings.MusicVolSlider.value);
		PlayerPrefs.SetFloat("sfxVol", settings.SFXVolSlider.value);
	}
}

[System.Serializable]
public class Settings
{
	public GameObject SettingsPanel;
	public Toggle MusicToogle, SFXToogle;
	public Slider MusicVolSlider, SFXVolSlider;
	public Button BackButton;
}

[System.Serializable]
public class Sound
{
	public AudioSource BackgroundMusicPlayer, SFXPlayer;
	public AudioClip backGroundMusic;
	public AudioClip clickSound;
}

[System.Serializable]
public class Shop
{
	public GameObject shopPanel;
	public Text coinsUI;
	public Button backButton;
	public Image swipeArea;
	public Scrollbar scrollbar;
	public Characters[] chatacters;

	public Button buyButton;
	public Text buyButtonText;
}

[System.Serializable]
public class Characters
{
	public GameObject prefab;
	public int cost;
	public bool isSelected, bought;
}
