using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviour
{
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyPowerUp();
        }
    }

    private void DestroyPowerUp()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
