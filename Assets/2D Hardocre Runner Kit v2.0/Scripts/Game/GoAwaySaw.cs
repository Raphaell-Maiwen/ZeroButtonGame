using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAwaySaw : MonoBehaviour
{
    public MovingObstacle sawScript;
    public GameObject jumpCollider;
    public GameObject sawGO;
    public bool sawInCollider = false;
    public bool playerInCollider = false;

    //TODO: Maybe use this for performance issues
    //public int sawId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == sawGO)
        {
            sawInCollider = true;
            if (playerInCollider && sawScript.goingUp)
            {
                Debug.Log("Let's goooo");
                sawScript.goingUp = !sawScript.goingUp;
                sawScript.point = 1;
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            playerInCollider = true;
            if (sawInCollider && sawScript.goingUp)
            {
                Debug.Log("Let's goooo");
                sawScript.goingUp = !sawScript.goingUp;
                sawScript.point = 1;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == sawGO)
        {
            sawInCollider = false;
            jumpCollider.GetComponent<BoxCollider2D>().enabled = false;
            jumpCollider.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            playerInCollider = false;
        }
    }
}
