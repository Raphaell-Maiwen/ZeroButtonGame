using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTriggers : MonoBehaviour
{
    public MovingObstacle movingObstacleScript;

    public Transform SawToFollow;
    public float xModifier;
    public float yModifier;
    public bool verticalSaw;

    public bool ColliderActive;
    bool bunchTriggersActive;

    public GameObject rollTrigger;
    public GameObject bunchOfTriggers;

    private void Start()
    {
        FollowSaw();
        if (verticalSaw) {
            movingObstacleScript.updateSawTriggers.Add(EnableRollTrigger);
            movingObstacleScript.updateBunchTriggers.Add(EnableBunchTriggers);
            movingObstacleScript.disableBunchTriggers.Add(DisableBunchTriggers);
        }
    }

    private void Update()
    {
        FollowSaw();
    }

    public void EnableRollTrigger() {
        rollTrigger.SetActive(true);
    }

    public void EnableBunchTriggers() {
        //if (!bunchTriggersActive) {
            rollTrigger.SetActive(false);
        //    bunchTriggersActive = !bunchTriggersActive;
            bunchOfTriggers.SetActive(true);
        //}
    }

    public void DisableBunchTriggers() {
        //if (bunchTriggersActive)
        //{
        //    bunchTriggersActive = !bunchTriggersActive;
            bunchOfTriggers.SetActive(false);
        //}
    }

    public void FollowSaw() {
        Vector3 nextPosition = SawToFollow.position;
        nextPosition.x += xModifier;
        nextPosition.y += yModifier;

        this.transform.position = nextPosition;
    }
}
