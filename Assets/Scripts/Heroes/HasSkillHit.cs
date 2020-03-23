using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSkillHit : MonoBehaviour
{
    [SerializeField] private GameObject ownerObject = null;
    private bool isActualHitTime;
    private bool doOnce;
    private float skilldmg;
    private Collider myCollider = null;

    List<int> IDs = new List<int>();

    //스킬이 켜졌을때, 꺼졌을때 호출
    public void SetActualHitTime(bool val)
    {
        if (isActualHitTime == val) return;

        isActualHitTime = val;
        doOnce = true;
    }

    public void SetSkillDmg(float dmg)
    {
        skilldmg = dmg;
    }


    void Start()
    {
        isActualHitTime = false;
        myCollider = GetComponent<Collider>();
        doOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(doOnce)
        {
            if (isActualHitTime)
            {
                myCollider.enabled = true;
            }
            else
            {
                myCollider.enabled = false;
                if(IDs.Count !=0 )IDs.Clear();
            }
            doOnce = false;
        }
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Creep"))
        {
            bool bCheck = false;
            int currentID = col.gameObject.GetInstanceID();

            if (IDs.Count != 0)
            {
                foreach (int id in IDs)
                {
                    if (id == currentID)
                    {
                        bCheck = true;
                        break;
                    }
                }
            }

            if (!bCheck)
            {
                IDs.Add(currentID);
                ACreeps acreep = null;
                if(col.gameObject.TryGetComponent<ACreeps>(out acreep)) acreep.GetHit(ownerObject, skilldmg);
            }
        }
    }
}
