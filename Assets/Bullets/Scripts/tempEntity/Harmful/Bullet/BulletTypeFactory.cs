using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Sigleton
 * 
 */
public class BulletTypeFactory
{
    private static BulletTypeFactory instance;
    private Dictionary<string,DurationBulletType> DurationType;
    private Dictionary<string,InverseBulletType> InverseType;
    private BulletTypeFactory()
	{
        DurationType = new Dictionary<string, DurationBulletType>();
		InverseType = new Dictionary<string, InverseBulletType>();
	}
	public DurationBulletType GetDurationBulletType(Sprite image, float speed,
		float duration, Vector2 direction, string posDescribe)
    {
        string TypeName = image.name + speed.ToString() + duration.ToString() + direction.ToString() + posDescribe;
        if(!DurationType.ContainsKey(TypeName))
        {
            DurationBulletType newType = DurationBulletType.CreateInstance<DurationBulletType>();
            newType.Init(image, speed, duration, direction, posDescribe);
            DurationType[TypeName] = newType;
        }
        return DurationType[TypeName];
    }
	public InverseBulletType GetInverseBulletType(Sprite image, float speed,
		float duration, Vector2 direction, string posDescribe)
	{
		string TypeName = image.name + speed.ToString() + duration.ToString() + direction.ToString() + posDescribe;
		if (!InverseType.ContainsKey(TypeName))
		{
			InverseBulletType newType = InverseBulletType.CreateInstance<InverseBulletType>();
			newType.Init(image, speed, duration, direction, posDescribe);
			InverseType[TypeName] = newType;
		}
		return InverseType[TypeName];
	}
	public static BulletTypeFactory GetBulletTypeFactory() 
    {
        if (instance == null)
            instance = new BulletTypeFactory();
        return instance;
    }
}
