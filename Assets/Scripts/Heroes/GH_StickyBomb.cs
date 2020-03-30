using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GH_StickyBomb : MonoBehaviour
{
    private Vector3 targetPoint = Vector3.zero;
    private Vector3 startPoint = Vector3.zero;

    GameObject smudge = null;
    GameObject bomb = null;

    private bool startSmudge = false;
    private float timer = 0.0f;
    private float keepTime = 3.0f;

    private float per = 0.0f;

    void Awake()
    {
        smudge = transform.GetChild(0).gameObject;
        bomb = transform.GetChild(1).gameObject;
        
        smudge.SetActive(false);
        bomb.SetActive(true);

        this.gameObject.SetActive(false);
    }

    void Start()
    {
        timer = 0.0f;
        keepTime = 3.0f;
    }


    void Update()
    {
        float dist = Vector3.Distance(targetPoint, transform.position);

        if(dist < 0.5f)
        {
            bomb.SetActive(false);
            transform.rotation = Quaternion.identity;

            if(timer < keepTime)
            {
                smudge.SetActive(true);

                timer += Time.deltaTime;
            }
            else
            {
                smudge.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            transform.Rotate(transform.right, 3.0f);
            transform.position += (targetPoint - startPoint).normalized *0.25f;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(per > 0.01f)
        {
            per = 0.0f;
            if (col.tag == "Creep")
            {
                col.gameObject.GetComponent<ACreeps>().GetHit(0.1f, CROWD_CONTROL_TYPE.SLOW, 0.2f);
                print("sticky bommb");
            }
        }
        per += Time.deltaTime;
    }

    public void ActivateBomb(Vector3 startPos, Vector3 targetPos)
    {
        this.gameObject.SetActive(true);
        
        startPoint = startPos;
        transform.position = startPoint;

        targetPoint = targetPos;
        smudge.SetActive(false);
        bomb.SetActive(true);
        timer = 0.0f;
        per = 0.0f;
    }
}
