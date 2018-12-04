using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject Bomb;
  
    public bool SetUnFreeze = true;

    private float m_Speed = .3f;
    private int m_PowerAdded;

    private Rigidbody m_RigidBody;
    private Animator m_Animator;

    private Button m_UpButton;
    private Button m_DownButton;
    private Button m_LeftButton;
    private Button m_RightButton;
    private Button m_FireButton;

    private Vector3 m_BombBoundsExtent;
    private RigidbodyConstraints m_OriginalConstraints;

    private float m_NextDropTime;
    private float m_DropDelay = 0.25f;

    private void Awake()
    {
        //Freeze the characters at start. Unfreeze when all players are loadeded.
        if (photonView.IsMine)
        {
           m_OriginalConstraints = GetComponent<Rigidbody>().constraints;
           GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
    }
    // Use this for initialization
    void Start()
    {
        m_PowerAdded = 0;
        m_RigidBody = GetComponent<Rigidbody>();
        m_RigidBody.freezeRotation = true;
        m_Animator = GetComponent<Animator>();

        m_UpButton = GameObject.Find("Up").GetComponent<Button>();
        m_DownButton = GameObject.Find("Down").GetComponent<Button>();
        m_LeftButton = GameObject.Find("Left").GetComponent<Button>();
        m_RightButton = GameObject.Find("Right").GetComponent<Button>();
        m_FireButton = GameObject.Find("FireButton").GetComponent<Button>();

        //Set the bomb scale appropiately and get the extents of the bomb's bounding box
        Bomb.transform.localScale = transform.localScale * 200;

        Renderer bombRenderer = Bomb.GetComponent<Renderer>();
        m_BombBoundsExtent = bombRenderer.bounds.extents;
    }

    void FixedUpdate()
    {
            if (SetUnFreeze && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (photonView.IsMine)
                {
                    GetComponent<Rigidbody>().constraints = m_OriginalConstraints;
                    SetUnFreeze = false;
                    StartCoroutine(Unfreeze());
                }
            }
        
        if (photonView.IsMine == false)
        {
            return;
        }

        m_Animator.SetBool("IsMoving", false);

        if (m_UpButton.Pressed)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            m_Animator.SetBool("IsMoving", true);
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 1 * m_Speed);
        }

        if (m_DownButton.Pressed)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            m_Animator.SetBool("IsMoving", true);
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, -1 * m_Speed);
        }

        if (m_LeftButton.Pressed)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            m_Animator.SetBool("IsMoving", true);
            m_RigidBody.velocity = new Vector3(-1 * m_Speed, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
        }

        if (m_RightButton.Pressed)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            m_Animator.SetBool("IsMoving", true);
            m_RigidBody.velocity = new Vector3(1 * m_Speed, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
        }

        if (m_FireButton.Pressed)
        {
            if (Time.time < m_NextDropTime)
                return;
            else
            {
                m_NextDropTime = Time.time + m_DropDelay;
                DropBomb();
            }
        }
    }


    private void DropBomb()
    {
        Debug.Log("drop");
        RaycastHit hit;
        Vector3 bombPosition;

        //Get position of floor block using raycast
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            bombPosition = hit.transform.transform.position;
            //Place the bomb slighty about the floor block that was hit by the raycast
            bombPosition.y += (hit.transform.localScale.y / 2) + m_BombBoundsExtent.y;

            bombPosition = new Vector3(bombPosition.x, bombPosition.y, bombPosition.z);
            GameObject bombInstance = PhotonNetwork.Instantiate("Bomb", bombPosition, Bomb.transform.rotation) as GameObject;
            bombInstance.transform.localScale = Bomb.transform.localScale;
            bombInstance.GetComponent<Bomb>().Power += m_PowerAdded;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosion"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
        if (other.CompareTag("FireUp"))
        {
            if (photonView.IsMine)
                m_PowerAdded++;
        }
        if (other.CompareTag("SpeedUp"))
        {
            if (photonView.IsMine)
                m_Speed += 0.03f;
        }
    }

    private IEnumerator Unfreeze()
    {
        if (photonView.IsMine)
        { 
            yield return new WaitForSeconds(2f);
            GetComponent<Rigidbody>().constraints = m_OriginalConstraints;
        }
    }

}
