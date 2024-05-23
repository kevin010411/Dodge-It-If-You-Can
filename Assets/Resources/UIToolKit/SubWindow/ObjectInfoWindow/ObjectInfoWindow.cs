using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UtilComponent;

namespace EditorScene
{
	public class ObjectInfoWindow : VisualElement
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<ObjectInfoWindow> { }
		// Start is called before the first frame update
		private const string styleResource = "UIToolKit/SubWindow/ObjectInfoWindow/MainStyle";
		private ScrollView root;
		private Button RecoverButton = null;
		private Label TitleName;
		private EditorCamera Camera;
		public enum PosDiscribe { GeneratorPos,AbsolutePos };

		public ObjectInfoWindow() 
		{
			styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
			root = new ScrollView(ScrollViewMode.Vertical);
			root.verticalPageSize = 200;
			root.mouseWheelScrollSize = 1400;
			root.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
			root.AddToClassList("ScrollContainer");
			AddToClassList("Container");
			Add(root);

			TitleName = new Label("Generator Info");
			TitleName.AddToClassList("title");
			root.Add(TitleName);

			
			Button HideButton = new Button();
			HideButton.AddToClassList("HideButton");
			HideButton.RegisterCallback<ClickEvent>(evt =>
			{
				float rootWidth = layout.width;
				style.left = new(-rootWidth);
				if (RecoverButton == null)
				{
					RecoverButton = new Button();
					RecoverButton.style.left = rootWidth + 2;
					RecoverButton.AddToClassList("RecoverButton");
					RecoverButton.RegisterCallback<ClickEvent>(evt =>
					{
						style.left = new(5);
						RecoverButton.RemoveFromHierarchy();
						RecoverButton = null;
					});
					Add(RecoverButton);
				}
			});
			Add(HideButton);
			_GetEditorCamera();
		}

		public void _GetEditorCamera()
		{
			Camera = GameObject.Find("/Main Camera").GetComponent<EditorCamera>();
			if (Camera == null)
				Debug.LogError("為找到Main Camera");
		}

		public TextField AddGeneratorParamsRow(string ParamsName ,string ParamsValue)
		{
			TextField newRow = new TextField(ParamsName+":");
			newRow.maxLength = 20;
			newRow.value = ParamsValue;
			newRow.multiline = true;
			newRow.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
			newRow.AddToClassList("paramRow");
			root.Add(newRow);
			return newRow;
		}
		
		public void AddBulletFoldOut(SaveData.BulletInfo bulletInfo,int insertIndex = -1)
		{
			Foldout BulletInfoContainer = GetBulletInfoContainer();
			Foldout container = new Foldout();
			container.text = bulletInfo.ClassName;
			container.AddToClassList("paramFoldout");
			Button RemoveButton = new Button(() =>
			{
				UpdateBulletInfo.Invoke(BulletInfoContainer.IndexOf(container), null);
				container.RemoveFromHierarchy();
			});
			RemoveButton.text = "Delete";
			RemoveButton.AddToClassList("RemoveButton");
			container.Q<Toggle>("").Q<VisualElement>("").Add(RemoveButton);

			Type testType = BulletManager.GetBulletManager().GetBulletEnum();
			Enum BulletClassEnum = (Enum)Enum.Parse(testType, bulletInfo.ClassName);
			EnumField BulletRow = new EnumField("ClassName:", BulletClassEnum);
			BulletRow.RegisterCallback<ChangeEvent<Enum>>(evt =>
			{
				Type newBulletClass = Type.GetType(evt.newValue.ToString());
				MethodInfo BuildInitParam = newBulletClass.GetMethod("CreateInitBulletInfo", BindingFlags.Static | BindingFlags.Public);
				SaveData.BulletInfo InitInfo = (SaveData.BulletInfo)BuildInitParam.Invoke(null,null);
				Foldout BulletInfoContainer = GetBulletInfoContainer();
				int index = BulletInfoContainer.IndexOf(container);
				UpdateBulletInfo.Invoke(index, InitInfo);
				container.RemoveFromHierarchy();
				AddBulletFoldOut(InitInfo, index);
			});
			BulletRow.AddToClassList("paramRow");
			container.Add(BulletRow);

			_AddDoubleField("Start:", bulletInfo.start, container);
			_AddDoubleField("End:", bulletInfo.end, container);
			_AddDoubleField("Interval:", bulletInfo.interval, container);

			foreach (KeyValuePair<string,string> param in bulletInfo.BulletsParams.ToDictionary())
				_AddBulletParamRow(container, param.Key, param.Value);

			if (insertIndex == -1)
				insertIndex = BulletInfoContainer.childCount - 1;
			BulletInfoContainer.Insert(insertIndex, container);
		}

		private DoubleField _AddDoubleField(string title, double value, Foldout container,
			int insertIndex = -1, bool updateAncestors = false)
		{
			DoubleField doubleField = new DoubleField(title);
			doubleField.value = value;
			doubleField.maxLength = 20;
			doubleField.AddToClassList("paramRow");
			doubleField.RegisterCallback<ChangeEvent<double>>(evt =>
			{
				if (updateAncestors)
					_UpdateBullet((Foldout)container.parent);
				else
					_UpdateBullet(container);
			});
			if (insertIndex == -1)
				container.Add(doubleField);
			else
				container.Insert(insertIndex, doubleField);
			return doubleField;
		}

		private void _AddBulletParamRow(Foldout container,string Key,string Value)
		{
			if (Key == "SpritePath" || Key == "subSpriteName")
				return;
			if (Key == "posDescribe")
			{
				PosDiscribe initDiscribe;
				if (!Enum.TryParse(Value, out initDiscribe))
					initDiscribe = PosDiscribe.GeneratorPos;
				EnumField paramRow = new EnumField("Bullet Type:", initDiscribe);
				DoubleField xRow = null;
				DoubleField yRow = null;
				paramRow.RegisterCallback<ChangeEvent<Enum>>((evt) =>
				{
					if(evt.newValue.ToString() == "AbsolutePos")
					{
						int nowIndex = container.IndexOf(paramRow);
						xRow = _AddDoubleField("x:", 0, container, nowIndex+1);
						yRow = _AddDoubleField("y:", 0, container, nowIndex+1);
					}
					else
					{
						xRow.RemoveFromHierarchy();
						yRow.RemoveFromHierarchy();
					}
					_UpdateBullet(container);
				});
				container.Add(paramRow);
				if(initDiscribe==PosDiscribe.AbsolutePos)
				{
					xRow = _AddDoubleField("x:",0,container);
					yRow = _AddDoubleField("y:",0,container);
				}
			}
			else if(Key == "Speed" || Key == "Duration")
			{
				_AddDoubleField(Key+":", double.Parse(Value), container);
			}
			else if(Key == "Direction")
			{
				Foldout DirectionFoldout = new Foldout();
				DirectionFoldout.text = "Bullet Direction";
				int pos = Value.IndexOf(":");
				float dirX = float.Parse(Value.Substring(Value.IndexOf("X:") + 2, Value.IndexOf(",") - pos - 1));
				float dirY = float.Parse(Value.Substring(Value.IndexOf("Y:") + 2, Value.IndexOf("}") - Value.IndexOf(":", pos + 1) - 1));
				double angle = Math.Atan(dirY / dirX) * Mathf.Rad2Deg;
				DoubleField yRow = _AddDoubleField("Direction y:", dirY, DirectionFoldout,updateAncestors:true);
				DoubleField xRow = _AddDoubleField("Direction x:", dirX, DirectionFoldout, updateAncestors: true);
				xRow.focusable = false;
				yRow.focusable = false;
				DoubleField angleRow = _AddDoubleField("Angle :", angle, DirectionFoldout, updateAncestors: true);
				angleRow.RegisterCallback<ChangeEvent<Double>>(evt =>
				{
					xRow.value = Math.Round(Math.Cos(evt.newValue * Mathf.Deg2Rad),4);
					yRow.value = Math.Round(Math.Sin(evt.newValue * Mathf.Deg2Rad),4);
				});
				Button DectectAngleButton = new Button(() =>
				{
					//Debug.Log("Clicked");	
					Camera.ChangeEditAngleMode();
					Camera.AngleClick.AddListener((angle) =>
						{
							angleRow.value = angle;
						}
					);
				});
				DectectAngleButton.text = "Detect Angle";
				DectectAngleButton.AddToClassList("DectectAngleButton");
				DirectionFoldout.Q<Toggle>("").Q<VisualElement>("").Add(DectectAngleButton);
				container.Add(DirectionFoldout);
			}
			else
			{
				TextField paramRow = new TextField(Key);
				paramRow.value = Value;
				paramRow.maxLength = 20;
				paramRow.RegisterCallback<ChangeEvent<string>>(evt =>
				{
					_UpdateBullet(container);
				});
				paramRow.AddToClassList("paramRow");
				container.Add(paramRow);
			}
		}

		public Foldout GetBulletInfoContainer()
		{
			Foldout BulletInfoContainer = root.Q<Foldout>("BulletContainer");
			if (BulletInfoContainer == null)
			{
				BulletInfoContainer = new Foldout();
				BulletInfoContainer.AddToClassList("paramFoldout");
				BulletInfoContainer.name = "BulletContainer";
				BulletInfoContainer.text = "Bullet Info";
				SwitchToggle toggle = new SwitchToggle();
				toggle.AddToClassList("SwitchToggle");
				toggle.setText("捲起", "展開");
				toggle.RegisterCallback<ClickEvent>(evt =>
				{
					foreach(VisualElement child in BulletInfoContainer.Children())
					{
						if(child is Foldout foldout)
						{
							if (toggle.textLabel.text == "捲起")
								foldout.value = false;
							else
								foldout.value = true;
						}
					}
				});
				BulletInfoContainer.Q<Toggle>("").Q<VisualElement>("").Add(toggle);
				root.Add(BulletInfoContainer);
				_AddCreateBulletInfoButton(BulletInfoContainer);
			}
			return BulletInfoContainer;
		}

		private void _AddCreateBulletInfoButton(Foldout parent)
		{
			Button button = new Button(() =>
			{
				BulletInfo initBullet = DurationBullet.CreateInitBulletInfo();
				UpdateBulletInfo.Invoke(parent.childCount-1, initBullet); 
				AddBulletFoldOut(initBullet);
			});
			button.text = "Create New Bullet";
			button.AddToClassList("RemoveButton");
			Foldout BulletInfoContainer = GetBulletInfoContainer();
			BulletInfoContainer.Add(button);
		}

		public UnityEvent<int,SaveData.BulletInfo> UpdateBulletInfo = new UnityEvent<int,SaveData.BulletInfo>();

		private SaveData.BulletInfo _GetBulletInfo(int index)
		{
			Foldout BulletInfoContainer = GetBulletInfoContainer();
			Foldout BulletInfo = (Foldout)BulletInfoContainer.ElementAt(index);
			/*
			* 目前是使用硬編碼，希望後面可以改進
			* 1. ClassName
			* 2. Start
			* 3. End
			* 4. interval
			* 5之後是BulletParams
			* 5.Speed
			* 6.BulletDirection
			* 7.Duration
			* 8.posDescribe
			* 
			*/
			string ClassName = ((EnumField)BulletInfo.ElementAt(0)).value.ToString();

			double start = ((DoubleField)BulletInfo.ElementAt(1)).value;
			double end = ((DoubleField)BulletInfo.ElementAt(2)).value;
			double interval = ((DoubleField)BulletInfo.ElementAt(3)).value;
			Type ClassType = Type.GetType(ClassName);
			MethodInfo createInitParams = ClassType.GetMethod("CreateInitBulletParam", BindingFlags.Static | BindingFlags.Public);
			Dictionary<string,string> bulletParams = (Dictionary<string, string>)createInitParams.Invoke(null,null);
			for(int i = 4;i< BulletInfo.childCount; ++i)
			{
				VisualElement field = BulletInfo.ElementAt(i);
				if (field is EnumField) //posDescribe
				{
					EnumField enumField = (EnumField)field;
					bulletParams["posDescribe"] = enumField.value.ToString();
					if (enumField.value.ToString() == "AbsolutePos")
					{
						++i;
						double pos_x = ((DoubleField)BulletInfo.ElementAt(i++)).value;
						double pos_y = ((DoubleField)BulletInfo.ElementAt(i++)).value;
						bulletParams["SpawnPos"] = $"{{X:{pos_x},Y:{pos_y}}}";
					}
				}
				else if(field is DoubleField) //Speed or Duration
				{
					DoubleField doubleField = (DoubleField)field;
					string Name = doubleField.label;
					Name = Name.Substring(0, Name.Length-1);
					bulletParams[Name] = doubleField.value.ToString();
				}
				else if(field is Foldout) //Direction
				{
					Foldout foldout = (Foldout)field;
					Debug.Assert(foldout.childCount==3,
						$"{foldout.name}=>Direction下面有{foldout.childCount}child");
					double yDir = ((DoubleField)foldout.ElementAt(0)).value;
					double xDir = ((DoubleField)foldout.ElementAt(1)).value;
					double angle = ((DoubleField)foldout.ElementAt(2)).value;
					bulletParams["Direction"] = $"{{X:{xDir},Y:{yDir}}}";
				}
			}

			return new BulletInfo(ClassName, bulletParams, start,end,interval);
		}

		private void _UpdateBullet(Foldout container)
		{
			Foldout BulletInfoContainer = GetBulletInfoContainer();
			int index = BulletInfoContainer.IndexOf(container);
			SaveData.BulletInfo newData = _GetBulletInfo(index);
			UpdateBulletInfo.Invoke(index, newData);
		}
	}
}