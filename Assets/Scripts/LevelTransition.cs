using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance;
    public int levelToLoad;
    public Animator levelFadeAnimator;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

            levelFadeAnimator = transform.GetComponentInChildren<Animator>();
        }
        else
        {
            Destroy(gameObject);
            if(GameObject.Find("TitleLoader")!=null)
                Destroy(GameObject.Find("TitleLoader"));
        }
    }
    public void FadeIn(){
        levelFadeAnimator.SetTrigger("FadeIn");
    }
    public void FadeOut(){
        levelFadeAnimator.SetTrigger("FadeOut");
    }
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
        FadeIn();
    }

}
