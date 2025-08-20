using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{

    public GameObject PlatformMove;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlatformMove.GetComponent<Animator>().Play("Platform");
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
