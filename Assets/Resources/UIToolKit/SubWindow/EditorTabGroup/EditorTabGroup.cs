using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorScene
{
	public class EditorTabGroup : VisualElement
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<EditorTabGroup> { }
		private const string styleResource = "UIToolKit/SubWindow/EditorTabGroup/MainStyle";

		public static List<string> CatelogList = new List<string>() {
				"Generator","Bullet"
			};

		VisualElement LableWindow;
		private void SetObjectName()
		{
			foreach (string label in CatelogList)
			{
				Label title = new Label(label);
				title.AddToClassList("tab");
				title.RegisterCallback<ClickEvent>(TabOnClick);
				LableWindow.Add(title);
			}
		}

		private void TabOnClick(ClickEvent evt)
		{
			Label clickedTab = evt.currentTarget as Label;
			if (!clickedTab.ClassListContains("SelectedTab"))
			{
				_GetAllTab().Where(
					(tab) => tab != clickedTab && tab.ClassListContains("SelectedTab")
				).ForEach(_UnselectTab);
				_SelectTab(clickedTab);
			}
		}

		private UQueryBuilder<Label> _GetAllTab()
		{
			return LableWindow.Query<Label>(className: "tab");
		}

		private void _UnselectTab(Label tab)
		{
			tab.RemoveFromClassList("SelectedTab");
			VisualElement content = FindContent(tab);
			content.AddToClassList("UnSelectContent");
		}
		private void _SelectTab(Label tab)
		{
			tab.AddToClassList("SelectedTab");
			VisualElement content = FindContent(tab);
			content.RemoveFromClassList("UnSelectContent");
		}

		public EditorTabGroup()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
			AddToClassList("MainContainer");
			LableWindow = new VisualElement();
			hierarchy.Add(LableWindow);
			LableWindow.AddToClassList("LabelRow");
			SetObjectName();
			foreach(string CatName in CatelogList)
			{
				ListView Content = new ListView();
				Content.name = CatName;
				Content.AddToClassList("Content");
				Content.AddToClassList("UnSelectContent");
				hierarchy.Add(Content);
				if(CatName == "Generator")
				{
					foreach(Type type in GeneratorManager.GetBulletManager().GetAllGeneratorTypes())
					{
						ContentBlock block = new ContentBlock();
						block.RegisterCallback<ClickEvent>(evt =>
						{
							stageManager.CreateNewGenerator();
						});
						block.SetObjectName(type.ToString());
						Content.hierarchy.Add(block);
					}
				}
				else if(CatName == "Bullet")
				{
					foreach (Type type in BulletManager.GetBulletManager().GetAllBulletTypes())
					{
						ContentBlock block = new ContentBlock();
						block.SetObjectName(type.ToString());
						Content.hierarchy.Add(block);
					}
				}
				else
				{
					for (int i = 0; i < 3; i++)
					{
						ContentBlock block = new ContentBlock();
						block.SetObjectName(CatName);
						Content.hierarchy.Add(block);
					}
				}
			}
			
		}

		private VisualElement FindContent(Label tab)
		{
			return this.Q(tab.text);
		}
		StageManager stageManager;
		public void SetStageManager(StageManager manager)
		{
			stageManager = manager;
		}
	}
	
	public class ContentBlock:VisualElement
	{
		private const string styleResource = "UIToolKit/SubWindow/EditorTabGroup/MainStyle";

		Label label;
		string ObjectName;
		public ContentBlock()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
			AddToClassList("ContentBlock");
			label = new Label($"Temp Block");
			hierarchy.Add(label);
		}
		public void SetObjectName(string  CatName)
		{
			ObjectName = CatName;
			label.text = $"{CatName}";
		}
	}
}
