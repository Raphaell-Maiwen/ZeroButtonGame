using UnityEngine;
using System.Collections;

public class CoinCollector : MonoBehaviour {


	public int moneyPerCoin = 1;
	public AudioClip collectSFX;
	private AudioSource SFXSource;

	private GameManager GM;

	void Start()
	{
		GM = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ();

		if(!SFXSource)
			SFXSource = new GameObject("Collect_AS", typeof(AudioSource)).GetComponent<AudioSource>();

		SFXSource.volume = PlayerPrefs.GetFloat("sfxVol");
	}

	void OnTriggerEnter2D(Collider2D coin) 
	{
		if(coin.CompareTag("Coin"))
		{
			if(GM.sfxOn && SFXSource && collectSFX)
			{
				SFXSource.GetComponent<AudioSource>().clip = collectSFX;
				SFXSource.Play();
			}

			GM.collectedCoins += moneyPerCoin;
	//		Destroy(coin.gameObject);
			coin.gameObject.SetActive(false);
		}
	}
}
