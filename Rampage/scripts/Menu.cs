using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject bg, bg2;
    public float bgWidth, bgMovingSpeed;
    float timeToMove = 0;
    public Image blackScreen;
    bool screenGoBlack, screenGoNormal;
    float blackScreenOpasity = 0; 
    public float musicVolume;
    public AudioSource BGM;
    void Update()
    {
        timeToMove += Time.deltaTime * bgMovingSpeed;
        bg.transform.position -= new Vector3(bgMovingSpeed * Time.deltaTime, 0, 0);
        bg2.transform.position -= new Vector3(bgMovingSpeed * Time.deltaTime, 0, 0);
        if (timeToMove > bgWidth)
        {
            timeToMove = -bgWidth;
            bg.transform.position -= new Vector3(-100, 0, 0);
            bg2.transform.position -= new Vector3(-100, 0, 0);
        }
    }
    private void Start()
    {
        StartCoroutine(TransitionBetweenScenesEnd());
    }

    private void FixedUpdate()
    {
        if(screenGoBlack)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreenOpasity += 0.02f;
            BGM.volume -= 0.03f;
            blackScreen.color = new(0f, 0f, 0f, blackScreenOpasity);
        }
        if (screenGoNormal)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreenOpasity -= 0.02f;
            BGM.volume += 0.03f;
            BGM.volume = Mathf.Clamp(BGM.volume, 0, musicVolume);
            blackScreen.color = new(0f, 0f, 0f, blackScreenOpasity);
        }
    }
    public void LoadScene(int index)
    {
        StartCoroutine(TransitionBetweenScenes(index));
    }
    public IEnumerator TransitionBetweenScenes(int index)
    {
        screenGoBlack = true;
        yield return new WaitForSeconds(1);
        screenGoBlack = false;
        SceneManager.LoadScene(index);
        yield return null;
    }
    public IEnumerator TransitionBetweenScenesEnd()
    {
        BGM.volume = 0;
        blackScreen.color = new(0f, 0f, 0f, 0f);
        blackScreenOpasity = 1f;
        screenGoNormal = true;
        Debug.Log(1);
        yield return new WaitForSeconds(1);
        blackScreen.gameObject.SetActive(false);
        screenGoNormal = false;
        yield return null;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
