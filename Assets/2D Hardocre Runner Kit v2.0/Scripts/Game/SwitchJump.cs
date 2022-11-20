using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchJump : MonoBehaviour
{
    
    public GameObject jumpCollider;
    public bool turningOn;

    public GameObject sawGO;
    private int sawId;

    private void Start()
    {
        sawId = sawGO.GetInstanceID();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.GetInstanceID() == sawId)
        {
            jumpCollider.GetComponent<BoxCollider2D>().enabled = turningOn;
            //jumpCollider.GetComponent<SpriteRenderer>().enabled = turningOn;
        }
    }
}
