using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;

public class Block : MonoBehaviourPunCallbacks
{

    public GameObject[] powerUps;   //Array of powerUps
    private string[] powers = new string[2];

    // Use this for initialization
    void Start () {
        powers[0] = "FireUp";
        powers[1] = "SpeedUp";
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Explosion"))
        {
            if(photonView.IsMine)
                PlacePowerUpAtRandom();
            DestroyCrate();
        }
    }

    private void PlacePowerUpAtRandom()
    {
        int randomChance = Random.Range(0, 10);
        
        //If less than or equal to 4, than instantiate a random gameObject from the powerUps Array
        if (randomChance <= 2)
        {
            int randomIndex = Random.Range(0, powerUps.Length);
            SpawnPowerUp(randomIndex);
        }
    }

    private void SpawnPowerUp( int randomIndex)
    {
        GameObject powerUp = PhotonNetwork.Instantiate(powers[randomIndex], gameObject.transform.position, Quaternion.identity) as GameObject;
        powerUp.transform.localScale = gameObject.transform.localScale;
    }

    private void DestroyCrate()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

}
