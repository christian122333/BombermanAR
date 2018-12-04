using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviourPunCallbacks {


    // Use this for initialization
    void Start () {
        //Destroy after 3 seconds
        StartCoroutine(WaitToDestroy(3f));
    }
	
    IEnumerator WaitToDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DestroyExplosion();
    }

    //[PunRPC]
    private void DestroyExplosion()
    {
        if (photonView.IsMine) 
            PhotonNetwork.Destroy(gameObject);
    }
}
