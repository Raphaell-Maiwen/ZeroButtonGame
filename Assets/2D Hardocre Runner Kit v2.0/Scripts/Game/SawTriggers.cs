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
    public bool canDisableTriggers;

    public bool ColliderActive;

    public GameObject rollTrigger;
    public GameObject bunchOfTriggers;

    public GameObject masterTrigger;

    private void Start()
    {
        FollowSaw();
        if (verticalSaw) {
            movingObstacleScript.updateRollTriggers.Add(EnableRollTrigger);
            movingObstacleScript.disableRollTriggers.Add(DisableRollTrigger);
            movingObstacleScript.updateBunchTriggers.Add(EnableBunchTriggers);
            movingObstacleScript.disableBunchTriggers.Add(DisableBunchTriggers);
        }
    }

    private void Update()
    {
        FollowSaw();
    }

    public void EnableRollTrigger() {
        if(rollTrigger)
            rollTrigger.SetActive(true);
    }

    public void DisableRollTrigger() {
        if (rollTrigger)
            rollTrigger.SetActive(false);
    }

    public void EnableBunchTriggers() {
        if (bunchOfTriggers) {
            rollTrigger.SetActive(false);
            bunchOfTriggers.SetActive(true);
        }
        
    }

    public void DisableBunchTriggers() {
        if(bunchOfTriggers)
            bunchOfTriggers.SetActive(false);
    }

    public void FollowSaw() {
        Vector3 nextPosition = SawToFollow.position;
        nextPosition.x += xModifier;
        nextPosition.y += yModifier;

        this.transform.position = nextPosition;
    }

    public void DisableTriggers() {
        Debug.Log("Disable Triggers");
        SawTriggers masterTriggerScript = masterTrigger.GetComponent<SawTriggers>();
        masterTriggerScript.verticalSaw = false;
        masterTriggerScript.rollTrigger.SetActive(false);
        masterTriggerScript.bunchOfTriggers.SetActive(false);
    }
}
