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

    public void ActivateBomb(Vector3 startPos, Vector3 targetPos)
    {
        this.gameObject.SetActive(true);
        
        startPoint = startPos;
        transform.position = startPoint;

        targetPoint = targetPos;
        smudge.SetActive(false);
        bomb.SetActive(true);
        timer = 0.0f;
    }
}
