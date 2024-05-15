using System;
using System.Linq;
using System.Reflection.Emit;
using System.Data;
using System.Reflection;
using UnityEngine;
public class BulletManager
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterBulletAttribute : Attribute
	{
		public RegisterBulletAttribute(Type typeOfWeapon)
		{
			TypeOfWeapon = typeOfWeapon;
		}

		public Type TypeOfWeapon { get; }
	}

	private BulletManager() 
	{
		AllBulletTypes = FindAllBulletTypes();
	}

	private static BulletManager Instance;
	public static BulletManager GetBulletManager()
	{
		if (Instance == null)
		{
			Instance = new BulletManager();
		}
		return Instance;
	}
	private Type[] AllBulletTypes;
	private Type[] FindAllBulletTypes()
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetCustomAttributes(false)
					  .OfType<RegisterBulletAttribute>()
				  )
				  .Select(att => att.TypeOfWeapon)
				  .ToArray();
	}
	public Type[] GetAllBulletTypes()
	{
		return AllBulletTypes;
	}
	public Type GetBulletEnum()
	{
		AssemblyName name = new AssemblyName("BulletEnums");
		AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name);

		// Define a public enumeration with the name "MyEnum" and an underlying type of Integer.
		EnumBuilder myEnum = moduleBuilder.DefineEnum("EnumeratedTypes.BulletEnum",
								 TypeAttributes.Public, typeof(int));

		for (int i = 0;i< AllBulletTypes.Length;++i)
		{
			myEnum.DefineLiteral(AllBulletTypes[i].ToString(), i);
		}

		// Create the enum
		Type EnumType = myEnum.CreateTypeInfo().AsType();

		return EnumType;
	}
}
