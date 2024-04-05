using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevUtils {
    /// <summary>
    /// For inverse of control. Use this class when you need to broadcast or
    /// send message by a specific method name. All possibly used method names
    /// are stored in this class.
    /// </summary>
    public class FunctionNames
    {
        static public string ResetStage = "ResetStageStatus";
        static public string TriggerPlayerDead = "TriggerPlayerDead";
        static public string TriggerTrackFinished = "TriggerTrackFinished";
        static public string ApplyDamageToPlayer = "ApplyDamageToPlayer";
        static public string ApplyMultiTimesDamageToPlayer = "ApplyMultiTimesDamageToPlayer";
        static public string ApplyHealEffectToPlayer = "ApplyHealEffectToPlayer";
        static public string GetStar = "GetStar";
        static public string ReceiveCurrentTimePoint = "ReceiveCurrentTimePoint";
    }
}
