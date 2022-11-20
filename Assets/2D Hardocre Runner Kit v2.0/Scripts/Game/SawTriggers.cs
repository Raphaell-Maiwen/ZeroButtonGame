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

    public GameObject jumpTrigger;

    private void Start()
    {
        if (verticalSaw) {
            movingObstacleScript.updateSawTriggers.Add(ToggleJumpTrigger);
        }
    }

    private void Update()
    {
        FollowSaw();
    }

    public void ToggleJumpTrigger() {
        ColliderActive = !ColliderActive;
        jumpTrigger.SetActive(ColliderActive);
    }

    public void FollowSaw() {
        Vector3 nextPosition = SawToFollow.position;
        nextPosition.x += xModifier;
        nextPosition.y += yModifier;

        this.transform.position = nextPosition;
    }
}
