using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAwaySaw : MonoBehaviour
{
    public MovingObstacle sawScript;
    public GameObject jumpCollider;
    private bool sawInCollider = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            sawInCollider = true;
        }
        //TODO: À compléter parce que c'est con
        else if (collision.gameObject.CompareTag("Player"))
        {
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
        //Debug.Log("Ca arrive");
        if (other.gameObject.CompareTag("Obstacle"))
        {
            sawInCollider = false;
            jumpCollider.GetComponent<BoxCollider2D>().enabled = false;
            jumpCollider.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
