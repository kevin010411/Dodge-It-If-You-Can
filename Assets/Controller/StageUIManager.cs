using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageUIManager : MonoBehaviour
{
    public static StageUIManager Instance;
    public List<GameObject>UITemplateList = new List<GameObject>();
    private Dictionary<string, GameObject> UITemplateDict = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject _GameObject in UITemplateList)
        {
            UITemplateDict.Add(_GameObject.name, _GameObject);
            Debug.Log($"gameObjectName=[{_GameObject.name}]");
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleEvent(DialogEvent TargetEvent)
    {
        GameObject SpecifiedTemplate = UITemplateDict[TargetEvent.UITemplateName];
        Canvas TemplateCanvas = SpecifiedTemplate.GetComponent<Canvas>();
        TemplateCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        TemplateCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        GameObject InstantiatedObject = Instantiate(SpecifiedTemplate);
        TextMeshProUGUI DialogContent = InstantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
        DialogContent.text = TargetEvent.DialogText;
        StartCoroutine(HandleDialogLifeCycle(InstantiatedObject));
    }

    private IEnumerator HandleDialogLifeCycle(GameObject TargetGameObject)
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Destroy(TargetGameObject);
    }
}
