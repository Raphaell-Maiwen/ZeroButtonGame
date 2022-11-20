using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTriggers : MonoBehaviour
{
    public Transform SawToFollow;
    public float xModifier;
    public float yModifier;
    public bool verticalSaw;

    public bool ColliderActive;

    private void Update()
    {
        FollowSaw();
    }

    public void FollowSaw() {
        Vector3 nextPosition = SawToFollow.position;
        nextPosition.x += xModifier;
        nextPosition.y += yModifier;

        this.transform.position = nextPosition;
    }
}
