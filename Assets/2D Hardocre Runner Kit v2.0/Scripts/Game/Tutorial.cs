using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject GameManager;
    public Transform[] PlayerPrefabs;
    public Transform PlayerSpawnPoint;

    private Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        if (!Player)
        {
            PlayerPrefs.SetString("Char", "RobotDefault");
            Debug.Log(PlayerPrefs.GetString("Char"));
            for (int i = 0; i < PlayerPrefabs.Length; i++)
            {
                if (PlayerPrefabs[i].name == PlayerPrefs.GetString("Char"))
                {
                    Player = (Transform)Instantiate(PlayerPrefabs[i].transform, PlayerSpawnPoint.position, Quaternion.identity);
                    break;
                }
            }
        }

        SetUp();
    }

    void SetUp() {
        Player.position = PlayerSpawnPoint.position;
        //playerControls.gameOver = false;
        //playerControls.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
