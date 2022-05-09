using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;





public class HubController : MonoBehaviour
{

    [Header("Scenes")]
    public Button puzzleScene;
    public Button drawingScene;


    [Header("Controls")]
    public Button increaseTime, decreaseTime, increaseDelay, decreaseDelay;
    public TMP_Text delayMonitor, timeMonitor;

    float vanishingTime, delayPointer;
    AppStore appStore;

    // Start is called before the first frame update
    void Start()
    {
        appStore = new AppStore();

        puzzleScene.onClick.AddListener(() => ChangeScene("puzzle"));
        drawingScene.onClick.AddListener(()=> ChangeScene("drawing"));

        increaseDelay.onClick.AddListener(() => AddDelay(true));
        decreaseDelay.onClick.AddListener(() => AddDelay(false));

        increaseTime.onClick.AddListener(() => AddTime(true));
        decreaseTime.onClick.AddListener(() => AddTime(false));


        //if(PlayerPrefs.GetFloat("drawing_delay")==0f|| PlayerPrefs.GetFloat("puzzle_delay") == 0f)
        //{
        //    PlayerPrefs.SetFloat("puzzle_delay", 2f);
        //    PlayerPrefs.SetFloat("drawing_delay", 0.2f);
        //}

       

        timeMonitor.text= string.Format("{0:0.00}", appStore.DrawingHaloVanishingTime);
        delayMonitor.text = string.Format("{0:0.00}", appStore.PointerDelay);

    }

    private void AddTime(bool add)
    {
        if (!add)
        {
            if (appStore.DrawingHaloVanishingTime > 0) { appStore.DrawingHaloVanishingTime -= 0.2f; }
        }
        else
        {
            if (appStore.DrawingHaloVanishingTime < 5) { appStore.DrawingHaloVanishingTime += 0.2f; }
        }
        timeMonitor.text = string.Format("{0:0.00}", appStore.DrawingHaloVanishingTime);
        PlayerPrefs.SetFloat("drawing_delay", appStore.DrawingHaloVanishingTime);
    }

    private void AddDelay(bool add)
    {
        if (!add)
        {
            if (appStore.PointerDelay > 0) { appStore.PointerDelay -= 1f; }
        }
        else
        {
            if (appStore.PointerDelay < 5) { appStore.PointerDelay += 1f; }
        }
       delayMonitor.text = string.Format("{0:0.00}", appStore.PointerDelay);
        PlayerPrefs.SetFloat("puzzle_delay", appStore.PointerDelay);
    }

    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
