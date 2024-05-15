using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScriptBehaviours : MonoBehaviour
{
    public Animator Transition;
    public float Duration = 2f;

    public void StartGame()
    {
        SceneLoadingController.Instance.LoadScene(
            "SelectLevel", Transition, Duration, "TriggerClose", "close");
    }
    public void OpenEditMode()
    {   
        SceneManager.LoadScene("EditorUI");
    }
}
