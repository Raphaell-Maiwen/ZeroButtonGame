using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSaw : MonoBehaviour
{
    public GameObject jumpCollider;
    public Transform SawToFollow;

    bool ColliderActive = true;

    private void Start()
    {
        //Debug.Log("Starting Saw");
        MovingObstacle movingObstacle = SawToFollow.gameObject.GetComponent<MovingObstacle>();
        movingObstacle.updateSawTriggers.Add(UpdateSawTrigger);

        /*if (movingObstacle.point == 1)
        {
            jumpCollider.GetComponent<BoxCollider2D>().enabled = false;
            jumpCollider.GetComponent<SpriteRenderer>().enabled = false;
            ColliderActive = false;
        }
        else
        {*/
            jumpCollider.GetComponent<BoxCollider2D>().enabled = true;
            jumpCollider.GetComponent<SpriteRenderer>().enabled = true;
            ColliderActive = true;
        //}
    }

    public void UpdateSawTrigger()
    {
        /*Debug.Log("GettingCalled");
        Debug.Log(ColliderActive);*/
        ColliderActive = !ColliderActive;

        jumpCollider.GetComponent<BoxCollider2D>().enabled = ColliderActive;
        jumpCollider.GetComponent<SpriteRenderer>().enabled = ColliderActive;
    }
}
