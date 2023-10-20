using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuManager : MonoBehaviour
{
    public void loadSceneSingle(string sceneName)
    {
        SceneSwitcher switcher = SceneSwitcher.Instance;
        switcher.loadSceneSingle(sceneName);
    }
}
