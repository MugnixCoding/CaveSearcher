using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    //dummy class object for coroutine
    //private class LoadingMonoBehaviour : MonoBehaviour { }

    private static Action OnLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    [SerializeField]
    private Animator trainsitionAnimator;
    [SerializeField]
    private float animationTime=1f;

    private const string start_animation = "Crossfade_Start";
    private const string end_animation = "Crossfade_End";

    
    public enum Scene
    {
        DungeonScene,
        FailScene,
        LoadingScene,
        MenuScene,
        SuccessScene
    }
    public static Loader Instance;
    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        trainsitionAnimator.gameObject.SetActive(false);
    }
    public void Load(Scene scene)
    {
        OnLoaderCallBack = () =>
        {
            StartCoroutine(LoadSceneAsync(scene,true));
        };
        trainsitionAnimator.gameObject.SetActive(true);
        StartCoroutine(StartLoadingScene());
    }
    public void Load(Scene scene, bool IsShowLoadingScene)
    {
        OnLoaderCallBack = () =>
        {
            StartCoroutine(LoadSceneAsync(scene, IsShowLoadingScene));
        };
        trainsitionAnimator.gameObject.SetActive(true);
        StartCoroutine(StartLoadingScene());
    }
    private IEnumerator StartLoadingScene()
    {
        trainsitionAnimator.Play(start_animation);
        yield return new WaitForSeconds(animationTime);
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    private IEnumerator LoadSceneAsync(Scene scene,bool IsShowLoadingScene)
    {
        if (IsShowLoadingScene)
        {
            trainsitionAnimator.Play(end_animation);
            yield return new WaitForSeconds(animationTime);
            trainsitionAnimator.Play(start_animation);
        }
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
        trainsitionAnimator.Play(end_animation);
        yield return new WaitForSeconds(animationTime);
        trainsitionAnimator.gameObject.SetActive(false);
    }
    public void LoaderCallBack()
    {
        if (OnLoaderCallBack!=null)
        {
            OnLoaderCallBack();
            OnLoaderCallBack = null;
        }
    }
    public float GetLoadingProgress()
    {
        if (loadingAsyncOperation!=null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
}
