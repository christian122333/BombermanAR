using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{

    public GameObject LocalPlayer;

    private List<Vector3> m_PlayerSpawnPoints = new List<Vector3>();
    private List<Vector3> m_CrateSpawnPoints = new List<Vector3>();
    private List<Vector3> m_FloorSpawnPoints = new List<Vector3>();
    private List<Vector3> m_BlockSpawnPoints = new List<Vector3>();

    public GameObject floor;
    public GameObject block;
    public GameObject crate;
    public GameObject crateInstance;
    public GameObject blockInstance;
    public GameObject floorInstance;
    public TextAsset levelData;

    public static float LevelWidth;

    private Vector3 m_LocalSize;
    private GameObject m_Anchor;
    private float m_LevelSize = 1f;


    public void CreateLevel(Transform m_Anchor)
    {
        this.m_Anchor = PhotonNetwork.InstantiateSceneObject("Anchor", new Vector3(0f, 0f), Quaternion.identity);
        this.m_Anchor.transform.parent = m_Anchor;
        ReadLevelText();
    }

    public IEnumerator LoadAnchor()
    {
        while (FindObjectsOfType<PhotonView>().Length <= 1)
            yield return new WaitForSeconds(3f);

        m_Anchor = PhotonView.Find(2).gameObject;
        ReadLevelText();
    }


    public void ReadLevelText()
    {
        //The multi-dimensional list will read all the values from the text file
        List<List<int>> tileData = new List<List<int>>();

        try
        {
            string line;
            StreamReader streamReader = new StreamReader(new MemoryStream(levelData.bytes));
            using (streamReader)
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    List<int> row = new List<int>();
                    string[] tiles = line.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string t in tiles)
                    {
                        row.Add(System.Int32.Parse(t));
                    }
                    tileData.Add(row);
                }
                streamReader.Close();
                if (tileData.Count > 0)
                    Initialize(tileData, m_LevelSize);
            }
        }
        catch (IOException e)
        {
            Debug.Log("" + e.Message);
        }
    }

    // Go through the tileData and initiate the gameObject depending on the number
    private void Initialize(List<List<int>> tileData, float levelSize)
    {
        int rows = tileData.Count;
        int columns = tileData[0].Count;

        float unit_width = levelSize / (float)columns;
        float unit_length = levelSize / (float)rows;

        LevelWidth = unit_width;

        Vector3 size = new Vector3(unit_width, (unit_width + unit_length) / 2, unit_length);
        block.transform.localScale = size;
        crate.transform.localScale = size;
        m_LocalSize = size;
            //Add each object to the objects list depending on the number that is give by the tile data list
            for (int z = 0; z < rows; z++)
            {
                for (int x = 0; x < columns; x++)
                {              
                    Vector3 floorPosition = new Vector3(m_Anchor.transform.position.x + (unit_width * x), -.7f, m_Anchor.transform.position.z + (unit_length * z));
                    m_FloorSpawnPoints.Add(floorPosition);
                    Vector3 otherPosition = new Vector3(floorPosition.x, (size.y / 2) + floorPosition.y + (size.y / 2), floorPosition.z);
   
                    if (tileData[z][x] == 1)
                        m_BlockSpawnPoints.Add(otherPosition);
                    else if (tileData[z][x] == 2)
                        m_CrateSpawnPoints.Add(otherPosition);
                    else if (tileData[z][x] == 3)
                    { 
                        Vector3 playerPosition = new Vector3(floorPosition.x, (size.y / 2) + floorPosition.y + (size.y / 2), floorPosition.z);
                        m_PlayerSpawnPoints.Add(playerPosition);
                    }
                }
            }

        placeInScene();
    }

    public void placeInScene()
    {
        Color color;
        bool changeColor = true;
        foreach (Vector3 floorSpawnPoint in m_FloorSpawnPoints)
        {
            //Alternate Colors in the floor blocks before adding them
            if (!changeColor)
            {
                color = new Color(.49f, .92f, .13f, 1);
                changeColor = true;
            }
            else
            {
                color = new Color(.31f, .59f, .07f, 1);
                changeColor = false;
            }
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().material.color = color;
            cube.transform.parent = m_Anchor.transform;
            cube.transform.localScale = m_LocalSize;
            cube.transform.position = floorSpawnPoint;
        }
    
        foreach (Vector3 blockSpawnPoint in m_BlockSpawnPoints)
            blockInstance = Instantiate(block, blockSpawnPoint, Quaternion.identity, m_Anchor.transform);
        
        //Only the master client will spawn networked objects like crates
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Vector3 crateSpawnPoint in m_CrateSpawnPoints)
            {
                crateInstance = PhotonNetwork.InstantiateSceneObject("Crate", crateSpawnPoint, Quaternion.identity);
                crateInstance.transform.parent = m_Anchor.transform;
                crateInstance.transform.localScale = m_LocalSize;
            }

            LocalPlayer = PhotonNetwork.Instantiate("Network Player", m_PlayerSpawnPoints[0], Quaternion.identity);
            LocalPlayer.transform.parent = m_Anchor.transform;
            LocalPlayer.transform.localScale = m_LocalSize;
        }
        else
        {
            LocalPlayer = PhotonNetwork.Instantiate("Network Player 2", m_PlayerSpawnPoints[1], Quaternion.identity);
            LocalPlayer.transform.parent = m_Anchor.transform;
            LocalPlayer.transform.localScale = m_LocalSize;
        }

    }

}


