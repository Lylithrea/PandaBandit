using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string[] persistentScenes = { "Permanent" };  // Name of the scene to keep loaded persistently.

    private static SceneSwitcher instance;


    public void loadSceneSingle(string sceneName)
    {
        //loop through all active scenes
        for(int i = 0; i < SceneManager.loadedSceneCount; i++)
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
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
            }

        }

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }


    // Property to access the Singleton instance.
    public static SceneSwitcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneSwitcher>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneSwitcher");
                    instance = singletonObject.AddComponent<SceneSwitcher>();
                }
            }
            return instance;
        }
    }

    // Ensure that the Singleton object persists between scenes.
    void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

}
