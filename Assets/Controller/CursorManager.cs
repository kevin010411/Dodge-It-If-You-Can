using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
	private static Texture2D RemoveCuror = Resources.Load<Texture2D>("Material/Cursor/Cross");
	public static void SetRemoveCursor()
	{
		Cursor.SetCursor(RemoveCuror, new Vector2(RemoveCuror.width / 2, RemoveCuror.height / 2), CursorMode.Auto);
	}
	public static void SetDefaltCursor()
	{
		Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
	}
}
