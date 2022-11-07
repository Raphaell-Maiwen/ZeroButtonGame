using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSaw : MonoBehaviour
{
    public GameObject jumpCollider;
    public Transform SawToFollow;

    bool ColliderActive = true;

    /*private void Start()
    {
        MovingObstacle movingObstacle = SawToFollow.gameObject.GetComponent<MovingObstacle>();
        movingObstacle.updateSawTriggers.Add(UpdateSawTrigger);

        jumpCollider.GetComponent<BoxCollider2D>().enabled = true;
        jumpCollider.GetComponent<SpriteRenderer>().enabled = true;
        ColliderActive = true;
    }
    */
    public void SwitchJump()
    {
        ColliderActive = !ColliderActive;

        //jumpCollider.GetComponent<BoxCollider2D>().enabled = ColliderActive;
        //jumpCollider.GetComponent<SpriteRenderer>().enabled = ColliderActive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SwitchJump"))
        {
            SwitchJump();
        }
    }
}
