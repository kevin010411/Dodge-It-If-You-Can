using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UtilFunction 
{
	/// <summary>
	/// 如同numpy的argsort，會回傳順序的list
	/// ex:		[1,2,6,5]
	///	return: [0,1,3,2]
	/// </summary>
	/// <param name="list">要排序的List</param>
	/// <returns name="ArgList">sort後的順序</returns>
	public static List<int> ArgSort<T>(List<T> list)
	{
		var indexedList = list.Select((value, index) => (value, index)).ToList();

		// 按值排序元组列表，并返回排序后索引的列表
		var sortedIndices = indexedList.OrderBy(item => item.value)
									   .Select(item => item.index)
									   .ToList();
		return sortedIndices;
	}
	public static string ListToString<T>(List<T> list)
	{
		return string.Join(", ", list);
	}
}
