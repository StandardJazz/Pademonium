using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Text percent;
    [SerializeField] private Image progressbar;
    private float progress;
    private AsyncOperation async;
    private float timer;
    IEnumerator Loading;

    // Start is called before the first frame update
    void Start()
    {
        percent.text = "0 %";
        timer = 0.0f;
        Loading = Loading_CO();
        StartCoroutine(Loading);
    }

    IEnumerator Loading_CO()
    {
        async = SceneManager.LoadSceneAsync("GameScene");
        async.allowSceneActivation = false;

        while (true)
        {
            timer += Time.deltaTime;

            progress = async.progress + 0.1f;
            percent.text = progress * 100.0f + "%";
            progressbar.fillAmount = progress;

            if (progress >= 1.0f && timer > 1.0f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
