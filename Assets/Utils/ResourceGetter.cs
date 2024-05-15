using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevUtils
{
    public class ResourceGetter
    {
        private static string PathSFX = "SFXs";
        public static AudioClip[] LoadAllSFXs()
        {
            return Resources.LoadAll<AudioClip>(PathSFX);
        }
        public static UnityEngine.Object LoadResourceCustomized(string FolderName, string FileName)
        {
            Debug.Log($"[Resource Getter's info]: Check Path ==> [Resource/{FolderName}/{FileName}]");
            return Resources.Load<UnityEngine.Object>($"{FolderName}/{FileName}");
        }
    }
}
