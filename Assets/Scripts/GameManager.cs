using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public Texture2D[] cursors = new Texture2D[2];
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public static GameObject player = null;

    private bool attackReady = false;
    
    void Awake()
    {
        //GameObject temp = Instantiate( Resources.Load(TypeInGame.heroname) as GameObject, new Vector3(5.0f,1.0f,5.0f),Quaternion.identity);
        GameObject temp = Instantiate( Resources.Load("FallenAngel") as GameObject, new Vector3(5.0f,1.0f,5.0f),Quaternion.identity);
        player = temp;
        if (player == null) SceneManager.LoadScene("SelectScene");
    }


    void Start()
    {
        Invoke("MyCursor", 0.01f);
    }


    void MyCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursors[0], hotSpot, cursorMode);
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (Input.GetKeyDown(KeyCode.A))
        {
            attackReady = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            attackReady = false;
        }

        if(attackReady)
        {
            if (Input.GetMouseButtonDown(0))
                attackReady = false;
        }

        Cursor.SetCursor(attackReady ? cursors[1] : cursors[0], hotSpot, cursorMode);
    }

}
