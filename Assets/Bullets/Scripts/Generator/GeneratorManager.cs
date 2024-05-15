using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;

public class GeneratorManager
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterGeneratorAttribute : Attribute
	{
		public RegisterGeneratorAttribute(Type typeOfWeapon)
		{
			TypeOfWeapon = typeOfWeapon;
		}

		public Type TypeOfWeapon { get; }
	}

	private GeneratorManager() 
	{
		AllGeneratorType = FindAllGeneratorTypes();
	}

	private static GeneratorManager Instance;
	public static GeneratorManager GetBulletManager()
	{
		if (Instance == null)
		{
			Instance = new GeneratorManager();
		}
		return Instance;
	}
	private Type[] AllGeneratorType;
	private Type[] FindAllGeneratorTypes()
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetCustomAttributes(false)
					  .OfType<RegisterGeneratorAttribute>()
				  )
				  .Select(att => att.TypeOfWeapon)
				  .ToArray();
	}
	public Type[] GetAllGeneratorTypes()
	{
		return AllGeneratorType;
	}

	public Type GetGeneratorEnum()
	{
		AssemblyName name = new AssemblyName("GeneratorEnums");
		AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name);

		// Define a public enumeration with the name "MyEnum" and an underlying type of Integer.
		EnumBuilder myEnum = moduleBuilder.DefineEnum("EnumeratedTypes.GeneratorEnum",
								 TypeAttributes.Public, typeof(int));

		for (int i = 0; i < AllGeneratorType.Length; ++i)
		{
			myEnum.DefineLiteral(AllGeneratorType[i].ToString(), i);
		}

		// Create the enum
		Type EnumType = myEnum.CreateTypeInfo().AsType();

		return EnumType;
	}
}
