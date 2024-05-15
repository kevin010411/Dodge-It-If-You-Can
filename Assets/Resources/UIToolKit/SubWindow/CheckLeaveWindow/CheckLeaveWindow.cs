using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorScene
{
	public class CheckLeaveWindow : VisualElement
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<CheckLeaveWindow> { }

		private const string styleResource = "UIToolKit/SubWindow/CheckLeaveWindow/PopupStyle";
		public CheckLeaveWindow() 
		{
			styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
			AddToClassList("popup_container");
			VisualElement window = new VisualElement();
			hierarchy.Add(window);
			window.AddToClassList("popup_window");

			VisualElement Content = new VisualElement();
			Content.AddToClassList("horizontal_container");
			window.Add(Content);

			Label Title = new Label();
			Title.text = "有變更內容尚未儲存請問是否要繼續離開?";
			Title.AddToClassList("popup_content");
			Content.Add(Title);

			VisualElement ButtonGroup = new VisualElement();
			ButtonGroup.AddToClassList("horizontal_container");
			window.Add(ButtonGroup);

			Button ConfirmButton = new Button() { text = "確定" };
			ConfirmButton.AddToClassList("popup_button");
			ConfirmButton.AddToClassList("confirm_button");
			ButtonGroup.Add(ConfirmButton);
			Button CancelButton = new Button() { text = "取消" };
			CancelButton.AddToClassList("popup_button");
			CancelButton.AddToClassList("cancel_button");
			ButtonGroup.Add(CancelButton);

			ConfirmButton.clicked += _OnConfirm;
			CancelButton.clicked += _OnCancel;

		}

		public event Action Confirmed;
		public event Action Canceled;

		private void _OnConfirm()
		{
			//Debug.Log("Confirm");
			Confirmed.Invoke();
		}

		private void _OnCancel()
		{
			//Debug.Log("Cancel");
			Canceled.Invoke();
		}
	}


}

