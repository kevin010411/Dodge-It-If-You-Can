using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EditorCamera : MonoBehaviour
{
	private UIManager EditorUIManager = null;
	// Start is called before the first frame update
	void Start()
    {
		try
		{
			EditorUIManager = GameObject.Find("/EditorUI").GetComponent<UIManager>();
			Debug.Log("Editor Mode");
		}
		catch { Debug.Log("Play Mode"); }
	}
	
	public float cameraMoveSpeed = 1f;
	public float zoomSpeed = 1f;

	private void _UpdateCurrentMode()
	{
		if (_CurrentMode == Mode.FindAngle)
		{
			return;
		}
		else if (EventSystem.current.IsPointerOverGameObject() || Input.GetMouseButtonUp(0))
		{
			_ResetCurrentMode();
		}
		else if (Input.GetMouseButtonDown(0) && _CurrentMode == Mode.Default)
		{
			touchedObject = _GetHitObject(Input.mousePosition);
			if (touchedObject != null && touchedObject.tag == "generator")
			{
				_CurrentMode = Mode.DraggingGenerator;
				EditorUIManager.ClickPasueButton();
			}
			else
				_CurrentMode = Mode.DraggingCamera;
		}
	}
	private void _ResetCurrentMode()
	{
		if(_CurrentMode == Mode.DraggingGenerator)
		{
			_ObjectPivot = touchedObject.transform.position;
		}
		_CurrentMode = Mode.Default;
	}

	// For DraggingGenerator Mode
	private Vector2 _ObjectPivot;
	private GameObject touchedObject;
	// For FindAngle Mode
	private GameObject line;

	private enum Mode { Default,FindAngle,DraggingGenerator,DraggingCamera};
	private Mode _CurrentMode = Mode.Default;
	
	public void ChangeEditAngleMode()
	{
		switch (_CurrentMode)
		{
			case Mode.Default:
				_CurrentMode = Mode.FindAngle;
				line = new GameObject("Arrow");
				DrawArrow Arrow = line.AddComponent<DrawArrow>();
				Arrow.tipLength = 0.5f;
				Arrow.tipWidth = 1f;
				Arrow.stemWidth = 0.5f;
				break;
			case Mode.FindAngle:
				_CurrentMode = Mode.Default;
				Destroy(line);
				break;
		}
	}

	public UnityEvent<float> AngleClick = new UnityEvent<float>();

	private void _ComputeAngle()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 0f;
		line.transform.position = _ObjectPivot;
		Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		Vector2 direction = mouseWorldPosition - _ObjectPivot;
		line.GetComponent<DrawArrow>().stemLength = direction.magnitude;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		line.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
		{
			if(Input.GetMouseButtonDown(0))
				AngleClick.Invoke(angle);
			Destroy(line);
			_CurrentMode = Mode.Default;
			return;
		}
	}
	
	private void _MoveCamera()
	{
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		if(mouseX > 2) { mouseX = 2; }
		if(mouseY > 2) { mouseY = 2; }
		transform.Translate(-mouseX * cameraMoveSpeed, -mouseY * cameraMoveSpeed, 0);
	}

	private void _MouseClick()
	{
		_UpdateCurrentMode();
		switch (_CurrentMode)
		{
			case Mode.DraggingCamera:
				_MoveCamera();
				break;
			case Mode.FindAngle:
				_ComputeAngle();
				break;
		}
	}
	
	private GameObject _GetHitObject(Vector2 Position)
	{
		Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Position);
		Collider2D collision = Physics2D.OverlapPoint(worldPosition);
		if (collision == null)
			return null;
		return collision.gameObject;
	}
	
	private void _ChangeSightDistance()
	{
		float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize -= scrollAmount * zoomSpeed;
	}
	
	private void _MouseScroll()
	{
		switch (_CurrentMode)
		{
			case Mode.FindAngle:
				break;
			default:
				_ChangeSightDistance();
				break;
		}
	}

	void Update()
    {
		_MouseClick();
		_MouseScroll();
	}
}
