using EditorScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UtilComponent
{
	public class SwitchToggle : Button
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<SwitchToggle> { }
		private const string styleResource = "UIToolKit/UtilComponent/SwitchToggle/MainStyle";
		public Label textLabel;
		//private float animationDuration = 0.5f;
		private string[] ShowMsg = new string[] { "On", "Off" };
		public SwitchToggle()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
			AddToClassList("switch");
			
			VisualElement slider = new VisualElement();
			slider.pickingMode = PickingMode.Ignore;
			slider.AddToClassList("slider");
			slider.AddToClassList("Off");
			Add(slider);
			textLabel = new Label("Off");
			textLabel.pickingMode = PickingMode.Ignore;
			textLabel.AddToClassList("sliderText");
			Add(textLabel);
			RegisterCallback<ClickEvent>(evt =>
			{
				if(slider.ClassListContains("On"))
				{
					style.backgroundColor = new Color(0, 255, 171,1);
					slider.RemoveFromClassList("On");
					slider.AddToClassList("Off");
					textLabel.text = ShowMsg[1];
				}
				else
				{
					style.backgroundColor = new Color(255, 0, 0,1);
					slider.AddToClassList("On");
					slider.RemoveFromClassList("Off");
					textLabel.text = ShowMsg[0];
				}
			});
		}

		public void setText(string OnText,string OffText)
		{
			ShowMsg = new string[] { OnText, OffText };
			textLabel.text = ShowMsg[1];
		}
	}

}
