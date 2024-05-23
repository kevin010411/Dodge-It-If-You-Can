using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

[assembly: GeneratorManager.RegisterGenerator(typeof(BulletGenerator))]
public class BulletGenerator : MonoBehaviour
{
	//[HideInInspector]
	public double start;
	//[HideInInspector]
	public double end;
	public int Index;
	public UIManager editorUI = null;
	protected List<SaveData.FirePoint> content;
	protected List<SaveData.BulletInfo> bulletInfo;
	public Dictionary<string, string> GeneratorParams;
	private int BullletIndex = 0;
	public void InitData(StageFragment fragment)
	{
		start = fragment.generatorInfo.start;
		end = fragment.generatorInfo.end;
		GeneratorParams = fragment.generatorInfo.GeneratorParams.ToDictionary();
		GetComponent<Transform>().position = new Vector2(fragment.generatorInfo.position.x,
														fragment.generatorInfo.position.y);
		content = new List<SaveData.FirePoint>();
		bulletInfo = fragment.content;
		foreach (SaveData.BulletInfo bullet in fragment.content)
			content.AddRange(bullet.SplitToPoint());
		content.Sort();
		BullletIndex = 0;
	}

	public void ResetIndex()
	{
		BullletIndex = 0;
	}
	public void SetUIManager(UIManager uIManager)
	{
		editorUI = uIManager;
	}
	public void Operation(double timeStamp)
	{
		while (CheckBulletSpawn(timeStamp))
		{
			CreateBullet(content[BullletIndex].FireTime);
			BullletIndex++;
		}
	}
	private bool CheckBulletSpawn(double timeStamp)
	{
		if (BullletIndex >= content.Count)
			return false;
		if (BullletIndex < content.Count && content[BullletIndex].FireTime < timeStamp)
			return true;
		return false;
	}
	private void CreateBullet(double spawnTime)
	{
		GameObject tempObj = new GameObject($"Bullet-{BullletIndex}");
		tempObj.tag = "bullet";
		Transform tempTransform = tempObj.transform;
		string posDescribe = content[BullletIndex].BulletsParams["posDescribe"];
		if (posDescribe == "GeneratorPos")
			tempTransform.position = transform.position;
		SpriteRenderer renderer = tempObj.AddComponent<SpriteRenderer>();
		CircleCollider2D circleCollider2D = tempObj.AddComponent<CircleCollider2D>();
		circleCollider2D.isTrigger = true;
		circleCollider2D.radius = 0.22f;
		try
		{
			Type type = Type.GetType(content[BullletIndex].ClassName);
			// TODO這邊是使用DurationBullet當作最高的父節點
			DurationBullet bullet = (DurationBullet)tempObj.AddComponent(type);
			bullet.Init(content[BullletIndex].BulletsParams, spawnTime, transform.position);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			Debug.LogError($"{content[BullletIndex].ClassName}有問題，請去確認所有ClassName");
		}
	}
	public UnityEvent<int,SaveData.GeneratorInfo> UpdateInfoEvent = new UnityEvent<int,SaveData.GeneratorInfo>();
	
	public void UpdateGenerator()
	{
		SaveData.GeneratorInfo newInfo = new SaveData.GeneratorInfo("BulletGenerator",
			transform.position, start, end, GeneratorParams);
		
		UpdateInfoEvent.Invoke(Index,newInfo);
	}

	private void _ShowObjectInfoWindow()
	{
		EditorScene.ObjectInfoWindow window = editorUI.CreateObjectInfoWindow();
		TextField StartRow = window.AddGeneratorParamsRow("GeneratorStart", start.ToString());
		TextField EndRow = window.AddGeneratorParamsRow("GeneratorEnd", end.ToString());
		List<TextField> lines = new List<TextField>();
		foreach (KeyValuePair<string, string> param in GeneratorParams)
			lines.Add(window.AddGeneratorParamsRow(param.Key, param.Value));
		StartRow.RegisterCallback<FocusOutEvent>(evt =>
		{
			start = Double.Parse(StartRow.value);
			UpdateGenerator();
		});
		EndRow.RegisterCallback<FocusOutEvent>(evt =>
		{
			end = Double.Parse(EndRow.value);
			UpdateGenerator();
		});
		foreach(TextField field in lines)
			field.RegisterCallback<FocusOutEvent>(evt =>
			{
				GeneratorParams[field.name] = field.value;
				UpdateGenerator();
			});

		foreach (SaveData.BulletInfo info in bulletInfo)
			window.AddBulletFoldOut(info);
		if (bulletInfo.Count == 0)
			window.GetBulletInfoContainer();
		window.UpdateBulletInfo.AddListener(UpdataBulletInfo);
	}

	private void UpdataBulletInfo(int BulletIndex,SaveData.BulletInfo newData)
	{
		if (newData == null)
		{
			//Debug.Log($"Remove{BulletIndex}");
			bulletInfo.RemoveAt(BulletIndex);
		}
		else if(BulletIndex == bulletInfo.Count)
		{
			//Debug.Log($"Add{BulletIndex}");
			bulletInfo.Add(newData);
		}
		else
		{
			//Debug.Log($"Edit{BulletIndex}");
			bulletInfo[BulletIndex] = newData;
		}

		// TODO 改進這個沒動腦的浪費
		content.Clear();
		foreach (SaveData.BulletInfo bullet in bulletInfo)
			content.AddRange(bullet.SplitToPoint());
		content.Sort();
		ResetIndex();
		//Debug.Log(content.Count());
	}

	public UnityEvent<int> RemoveGeneratorEvent = new UnityEvent<int>();
	public void RemoveGenerator()
	{
		RemoveGeneratorEvent.Invoke(Index);
	}
	private EventSystem eventSystem;
	public void Start()
	{
		eventSystem = EventSystem.current;
	}

	public void OnDestroy()
	{
		if(editorUI != null)
			editorUI.CloseObjectInfoWindow();
	}

	public void OnMouseUp()
	{
		if (eventSystem.IsPointerOverGameObject())
			return;
		UpdateGenerator();
		if (editorUI != null)
			_ShowObjectInfoWindow();
	}
	private float _MousePressTime = 0;
	public void OnMouseDown()
	{
		if (eventSystem.IsPointerOverGameObject())
			return;
		if (editorUI.IsRemoveMode)
		{
			//Debug.Log($"Remove{this.gameObject.name}");
			RemoveGenerator();
			Destroy(this.gameObject);
		}
		_MousePressTime = Time.time;
	}

	public float DragThershold = 0.3f;
	public void OnMouseDrag()
	{
		if (eventSystem.IsPointerOverGameObject())
			return;
		if (Time.time - _MousePressTime > DragThershold)
		{
			Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPos.z = 0;
			transform.position = newPos;
		}
	}
}
