using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private GameObject go_Player = null;

    delegate bool PlayerDead();
    PlayerDead IsPlayerDead = null;

    private Camera myCam;

    private Transform player_Transform = null;
    [SerializeField] LayerMask layerMask = 0;

    private Vector3 targetPoint;

    private Vector3 mousePos;

    private float camSpeed;
    private float zoomSpeed;
    private float camSlope;
    private float currentDistance;
    private float minDistance;
    private float maxDistance;

    //Dying
    private const float DyingTime = 2.0f;
    private float lerpTime = 0.0f;

    private float grayscaleLerpVal = 0.0f;
    private Material img_effect = null;

    private bool doOnce;
    private LayerMask layer;

    void Awake()
    {
        img_effect = new Material(Shader.Find("Shader/Grayscale_PP"));
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (grayscaleLerpVal == 0.0f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, img_effect);
    }
    
    void Start()
    {
        layer = ~(1 << 12 | 1<< 13);
        go_Player = GameManager.player;

        camSlope = this.transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
        
        myCam = this.gameObject.GetComponent<Camera>();
        player_Transform = go_Player.GetComponent<Transform>();
        IsPlayerDead = go_Player.GetComponent<AHeroes>().IsDead;
        camSpeed = 7.5f;
        zoomSpeed = 1.0f;
        minDistance = 5.0f;
        maxDistance = 16.5f;
        currentDistance = 16.5f;
        this.transform.position = new Vector3(player_Transform.position.x, +Mathf.Sin(camSlope) * currentDistance, -Mathf.Cos(camSlope) * currentDistance);

        doOnce = false;
        StartCoroutine(Zoom());
    }

    IEnumerator Zoom()
    {
        while (true)
        {
            float scrollVal = Input.GetAxis("Mouse ScrollWheel");

            if(scrollVal != 0 )
            {
                Vector3 camPos = this.transform.position;
                float distance = Vector3.Distance(camPos, player_Transform.position);   //16.5,5

                bool isLimit = false;

                if (scrollVal > 0)
                {
                    camPos = Vector3.MoveTowards(camPos, camPos + this.transform.forward, zoomSpeed);
                    if (distance < minDistance) isLimit = true;
                }
                else
                {
                    camPos = Vector3.MoveTowards(camPos, camPos - this.transform.forward, zoomSpeed);
                    if (distance > maxDistance) isLimit = true;
                }


                if (!isLimit)
                {
                    currentDistance = Vector3.Distance(camPos, player_Transform.position);

                    this.transform.position = camPos;
                }
            }

            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float y = + Mathf.Sin(camSlope) * currentDistance;
            float z = - Mathf.Cos(camSlope) * currentDistance;

            transform.position = Vector3.Lerp(transform.position, player_Transform.position + new Vector3(0.0f, y, z), 0.1f);
        }
        else
        {
            MoveByMouseShift();

        }

        DyingPostEffect();
    }

    private void DyingPostEffect()
    {
        if (IsPlayerDead())
        {
            doOnce = true;

            lerpTime += Time.deltaTime * 0.75f;
            float ratio = lerpTime / DyingTime;

            if (ratio < 1.0f)
            {
                grayscaleLerpVal = ratio;
                img_effect.SetFloat("_LerpVal", ratio);
            }
        }
        else
        {
            if (doOnce)
            {
                lerpTime = 0.0f;
                grayscaleLerpVal = 0.0f;
                doOnce = !doOnce;
            }
        }
    }


    private void MoveByMouseShift()
    {
        mousePos = myCam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 direction = Vector3.zero;

        if (mousePos.x < 0.1f)
        {
            direction = transform.position.x < 10.0f ? Vector3.zero : direction - Vector3.right;
        }
        else if (mousePos.x > 0.9f)
        {
            direction = transform.position.x > 40.0f ? Vector3.zero : direction + Vector3.right;
        }

        if (mousePos.y < 0.1f)
        {
            direction = transform.position.z < -5.0f ? Vector3.zero : direction - Vector3.forward;
        }
        else if (mousePos.y > 0.9f)
        {
            direction = transform.position.z > 30.0f ? Vector3.zero : direction + Vector3.forward;
        }

        if (direction == Vector3.zero) return;


        direction = direction.normalized * camSpeed * Time.deltaTime;

        transform.position += direction;
    }

    public Vector3 GetMousePoint()
    {
        Ray rayCam = myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(rayCam, out hitInfo, 1000.0f,layer))
        {
            targetPoint = hitInfo.point;
        }

        return targetPoint;
    }


}
