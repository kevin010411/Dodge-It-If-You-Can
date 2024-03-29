using System;
using UnityEngine;


[Serializable][CreateAssetMenu]
public class DurationBulletType : ScriptableObject
{
	public Sprite Image;
	public float Speed;
	public float Duration;
	public Vector2 Direction;
	public string PosDescribe;
	public void Init(Sprite image, float speed, float duration, Vector2 direction, string posDescribe)
	{
		Image = image;
		Speed = speed;
		Duration = duration;
		Direction = direction;
		PosDescribe = posDescribe;
	}
}