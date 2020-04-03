using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreepsManager : MonoBehaviour
{
    [SerializeField] private Vector3[] SpawnPointList = new Vector3[5];
    List<GameObject> creeps = new List<GameObject>();
    

    void Awake()
    {

        for(int i = 0; i <10; i++) SpawnCreep();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnCreep()   //크립 타입 정할 것. 
    {
        int random = (int)Random.Range(0.0f, 4.99f);
        GameObject temp = null;

        Vector3 sampPos = Vector3.zero;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(SpawnPointList[random], out hit, 10.0f, NavMesh.AllAreas))
        {
            temp = Instantiate(Resources.Load<GameObject>("cr_DummyBot"), hit.position, Quaternion.identity);

        }
        else
            print("fail");

        creeps.Add(temp);
    }


    public List<GameObject> GetCreepList()
    {
        return creeps;
    }
}
