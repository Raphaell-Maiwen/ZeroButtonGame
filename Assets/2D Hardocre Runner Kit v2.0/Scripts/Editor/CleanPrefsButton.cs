using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CleanPrefs))]

public class CleanPrefsButton : Editor {

	public override void OnInspectorGUI()
	{
		CleanPrefs cleaner = (CleanPrefs)target;

		if(GUILayout.Button("Clean Player Prefs"))
			cleaner.Clean();

		if(GUILayout.Button("Add 100 coins"))
			cleaner.AddCoins();
	}
}
