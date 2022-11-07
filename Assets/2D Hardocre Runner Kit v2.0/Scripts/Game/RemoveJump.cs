using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveJump : MonoBehaviour
{
    public GameObject jumpCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jump")) {
            jumpCollider.GetComponent<BoxCollider2D>().enabled = false;
            jumpCollider.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
