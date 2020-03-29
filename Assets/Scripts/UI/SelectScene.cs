using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectScene : MonoBehaviour
{
    [SerializeField] private Image selectArrow = null;
    [SerializeField] private Text heroname = null;


    void Start()
    {
        TypeInGame.heroname = "FallenAngel";
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
                TypeInGame.heroname = "GoblinHunter";
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
