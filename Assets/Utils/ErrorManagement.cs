using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DevUtils
{
    public class ErrorManagement
    {
        public static void Err(string ExName, string Who, string SayWhat)
        {
            Debug.LogError($"[{Who}] threw {ExName}Exception: {SayWhat}");
        }
        public static void Err(string ExName, string SayWhat)
        {
            Debug.LogError($"{ExName}Exception: {SayWhat}");
        }
    }
}
