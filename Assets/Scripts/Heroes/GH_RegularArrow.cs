using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GH_RegularArrow : MonoBehaviour
{
    GameObject targetObj = null;
    Vector3 startPos;
    float dmg;
    GameObject owner = null;


   
    // Start is called before the first frame update
    void Start()
    {
        owner = GameManager.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if(targetObj == null  || targetObj.tag != "Creep" || targetObj.GetComponent<ACreeps>().IsDead())
        {
            this.gameObject.SetActive(false);
            return;
        }

        Vector3 temp = targetObj.transform.position;
        temp.y = startPos.y;

        transform.forward = (temp - startPos).normalized;
        transform.position += (temp - startPos).normalized * 0.35f;
    }
   

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject == targetObj)
        {
            targetObj.GetComponent<ACreeps>().GetHit(GameManager.player, dmg);
            this.gameObject.SetActive(false);
        }
    }

    public void SetTarget(Vector3 startPoint, GameObject targetObj, float attackDamage)
    {
        this.gameObject.SetActive(true);
        startPos = startPoint;
        this.targetObj = targetObj;
        transform.position = startPos;
    
        dmg = attackDamage;
    }
}
