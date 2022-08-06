using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTriggers : MonoBehaviour
{
    public Transform SawToFollow;
    public float xModifier;
    public float yModifier;
    public bool verticalSaw;

    public GameObject rollTrigger;
    public GameObject jumpTrigger;

    public bool ColliderActive;

    private void Start()
    {
        if (verticalSaw) {
            MovingObstacle movingObstacle = SawToFollow.gameObject.GetComponent<MovingObstacle>();
            movingObstacle.updateSawTriggers.Add(UpdateSawTrigger);

            if (movingObstacle.point == 0)
            {
                this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                ColliderActive = false;
            }
            else
            {
                this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                ColliderActive = true;
            }
        }
    }

    private void Update()
    {
        FollowSaw();
    }

    public void UpdateSawTrigger() {
        ColliderActive = !ColliderActive;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = ColliderActive;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = ColliderActive;
    }

    public void FollowSaw() {
        Vector3 nextPosition = SawToFollow.position;
        nextPosition.x += xModifier;
        nextPosition.y += yModifier;

        this.transform.position = nextPosition;
    }

    public void DisableTrigger() {
        /*if (verticalSaw && rollTrigger) rollTrigger.SetActive(false);
        else if (jumpTrigger) jumpTrigger.SetActive(false);*/
    }
}
