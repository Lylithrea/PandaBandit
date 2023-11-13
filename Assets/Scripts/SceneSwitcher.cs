using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class SceneSwitcher : MonoBehaviour
{

    [BoxGroup("Always")] public string[] persistentScenes = { "Permanent" };  // Name of the scene to keep loaded persistently.
    [BoxGroup("GameView")] public string[] persistentGameScenes = { "PlayerInterface" };  // Name of the scene to keep loaded persistently.

    private static SceneSwitcher instance;
    [SerializeField] private SceneState state = SceneState.MainMenu;

    public void loadSceneSingle(string sceneName)
    {
        StartCoroutine(DeloadScene(sceneName));
    }

    private IEnumerator DeloadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("Scene loaded!");

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        //loop through all active scenes
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            bool unload = true;
            //check if active scene is allowed to be unloaded
            foreach (string persistentScene in persistentScenes)
            {
                if (SceneManager.GetSceneAt(i).name == persistentScene)
                {
                    Debug.Log("Found a scene that does not want to be unloaded");
                    unload = false;
                    break;
                }
            }
            if (unload)
            {
                Debug.Log("Found a scene that should be unloaded: " + SceneManager.GetSceneAt(i).name);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
            }

        }

        yield return null;
    }



    // Property to access the Singleton instance.
    public static SceneSwitcher Instance
    {
        get
        {
            return instance;
        }
    }

    // Ensure that the Singleton object persists between scenes.
    void Awake()
    {
        singletonCreation();
    }

    public void SetState(SceneState state)
    {
        this.state = state;
        updateScenes();
    }

    private void updateScenes()
    {
        StartCoroutine(loadPermanentScenes());
        if (state == SceneState.Game)
        {
            StartCoroutine(loadGameScenes());
        }
        else
        {
            StartCoroutine(deloadGameScenes());
        }
    }

    private void Start()
    {
        updateScenes();
    }

    private IEnumerator deloadGameScenes()
    {
        foreach (string persistentScene in persistentGameScenes)
        {
            if (SceneManager.GetSceneByName(persistentScene).isLoaded)
            {
                Debug.Log("Found a game scene that should be unloaded");
                SceneManager.UnloadSceneAsync(persistentScene);
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    private IEnumerator loadGameScenes()
    {
        foreach (string persistentScene in persistentGameScenes)
        {
            if (!SceneManager.GetSceneByName(persistentScene).isLoaded)
            {
                Debug.Log("Found a game scene that does need to be loaded");
                SceneManager.LoadSceneAsync(persistentScene, LoadSceneMode.Additive);
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    private IEnumerator loadPermanentScenes()
    {
        foreach (string persistentScene in persistentScenes)
        {
            if (!SceneManager.GetSceneByName(persistentScene).isLoaded)
            {
                Debug.Log("Found a scene that does not want to be unloaded");
                SceneManager.LoadSceneAsync(persistentScene, LoadSceneMode.Additive);
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    void singletonCreation()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }




}
public enum SceneState
{
    MainMenu,
    Game
}