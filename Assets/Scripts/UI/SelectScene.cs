using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectScene : MonoBehaviour
{
    int heroSelectIndex = 0;
    [SerializeField]private Image[] cards;
    [SerializeField] private Image selectArrow;
    [SerializeField] private Text heroname;


    void Start()
    {
        TypeInGame.heroname = "FallenAngel";
        heroSelectIndex = 0;
        heroname.text = TypeInGame.heroname;
    }

    public void SelectHero(int index)
    {
        switch(index)
        {
            case 0:
                TypeInGame.heroname = "FallenAngel";
                break;
            case 1:
                TypeInGame.heroname = "hero2";
                break;
            case 2:
                TypeInGame.heroname = "hero3";
                break;
            case 3:
                TypeInGame.heroname = "hero4";
                break;
            default:
                break;
        }

        heroname.text = TypeInGame.heroname;
        selectArrow.GetComponent<RectTransform>().localPosition = new Vector3(-375 + index * 250.0f, 200.0f, 0.0f);
    }

    public void ChangeToLoadingScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
