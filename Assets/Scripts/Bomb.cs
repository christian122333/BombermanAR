using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bomb : MonoBehaviourPun
{

    public GameObject Explosion;
    public LayerMask LevelMask;
    public int Power;

    private GameObject m_ExplosionInstance;
    private bool Exploded = false;
    private Vector3 Extents;
    private float UnitWidth = LevelManager.LevelWidth;

    private void Awake()
    {
        Power = 2;
    }

    // Use this for initialization
    void Start()
    {
        //Explode the bomb 2.5 seconds after instantiation
        Explosion.transform.localScale = transform.localScale / 200;
        Explosion.transform.localScale *= .95f;
        Invoke("Explode", 2.5f);
      
    }

    //Instantiate Explosion in center then Instantiate explosions in forward, back, left, and right directions 
    private void Explode()
    {
        SpawnExplosion(transform.position);
        StartCoroutine(Explosions(Vector3.forward));
        StartCoroutine(Explosions(Vector3.back));
        StartCoroutine(Explosions(Vector3.left));
        StartCoroutine(Explosions(Vector3.right));

        GetComponent<MeshRenderer>().enabled = false;
        Exploded = true;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(WaitToDestroy(0.3f));
    }

    //Create a number of explosions that go out into a certain direction
    private IEnumerator Explosions(Vector3 direction)
    {
        direction *= UnitWidth;
        for(int i = 1; i < Power; i++)
        {
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, direction, out hit, i * UnitWidth, LevelMask))
            {
                SpawnExplosion(transform.position + (i * direction));
            }
            else
                break;

            yield return new WaitForSeconds(.05f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!Exploded && other.CompareTag("Explosion"))
            {
                CancelInvoke("Explode");
                Explode();
            }      
    }

    private void SpawnExplosion(Vector3 position)
    {
        GameObject explosionInstace = PhotonNetwork.Instantiate("Fireball", position, Quaternion.identity);
        explosionInstace.transform.localScale = Explosion.transform.localScale;
    }

    IEnumerator WaitToDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DestroyBomb();
    }

    private void DestroyBomb()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
