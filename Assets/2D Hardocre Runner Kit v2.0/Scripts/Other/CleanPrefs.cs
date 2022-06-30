using UnityEngine;
using System.Collections;

public class CleanPrefs : MonoBehaviour {

	public void Clean()
	{
		PlayerPrefs.DeleteAll ();
	}

	public void AddCoins()
	{
		int coins = PlayerPrefs.GetInt ("Coins");
		coins += 100;
		PlayerPrefs.SetInt ("Coins", coins);
	}
}
