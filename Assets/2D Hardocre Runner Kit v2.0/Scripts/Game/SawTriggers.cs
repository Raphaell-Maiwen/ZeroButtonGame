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
    private bool initialVerticalSaw;
    public bool canDisableTriggers;

    public bool ColliderActive;

    public GameObject rollTrigger;
    public GameObject bunchOfTriggers;

    public GameObject masterTrigger;

    private void Start()
    {
        initialVerticalSaw = verticalSaw;
        FollowSaw();
        if (verticalSaw) {
            movingObstacleScript.updateRollTriggers.Add(EnableRollTrigger);
            movingObstacleScript.disableRollTriggers.Add(DisableRollTrigger);
            movingObstacleScript.updateBunchTriggers.Add(EnableBunchTriggers);
            movingObstacleScript.disableBunchTriggers.Add(DisableBunchTriggers);
            movingObstacleScript.resetInitialState.Add(ResetInitialState);
        }
    }

    private void Update()
    {
        FollowSaw();
    }

    public void ResetInitialState() {
        verticalSaw = initialVerticalSaw;
    }

    public void EnableRollTrigger() {
        if (rollTrigger && verticalSaw) {
            rollTrigger.SetActive(true);
            rollTrigger.transform.position = this.transform.position;
        }
    }

    public void DisableRollTrigger() {
        if (rollTrigger && verticalSaw)
            rollTrigger.SetActive(false);
    }

    public void EnableBunchTriggers() {
        if (bunchOfTriggers && verticalSaw) {
            rollTrigger.SetActive(false);
            bunchOfTriggers.SetActive(true);
        }
        
    }

    public void DisableBunchTriggers() {
        if(bunchOfTriggers && verticalSaw)
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
