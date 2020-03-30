using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum CROWD_CONTROL_TYPE
{
    NONE = 1 << 0  ,SLOW = 1 << 1,STUN = 1 << 2 
}


public class GameManager : MonoBehaviour
{
    public Texture2D[] cursors = new Texture2D[2];
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public static GameObject player = null;
    public static GameObject spell_Indicator = null;

    public Canvas mainCanvas = null;

    private bool attackReady = false;

    private float spawnTimer = 0.0f;

    
    void Awake()
    {
        TypeInGame.heroname = "GoblinHunter";
        player = Instantiate( Resources.Load(TypeInGame.heroname) as GameObject, new Vector3(5.0f,1.0f,5.0f),Quaternion.identity);
        spell_Indicator = Instantiate(Resources.Load("Hero_Spell_Indicator") as GameObject, new Vector3(5.0f, 5.5f, 5.0f), Quaternion.identity);

        if (player == null) SceneManager.LoadScene("SelectScene");

        player.GetComponent<AHeroes>().SetSpellIndicator(spell_Indicator.GetComponent<Spell_Indicator>());

        for(int i = 4; i <15; i++)
        {
            if((i-4) % 3 < 2)
            {
                int id = (i - 4) / 3 + 1;
                print("Sprites/Icons/" + TypeInGame.heroname + "_" + id +" i");
                mainCanvas.transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/" + TypeInGame.heroname + "_" + id);
            }
        }
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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()
    {
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
