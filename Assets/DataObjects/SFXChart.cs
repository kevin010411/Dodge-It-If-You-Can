using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXChart
{
    public List<SFXLaunchCommand> SFXLaunchCommands;
    public SFXChart() { SFXLaunchCommands = new List<SFXLaunchCommand>(); }

    public void CreateExampleChart()
    {
        SFXLaunchCommand ExA = new SFXLaunchCommand();
        ExA.LaunchTime = 10.0f;
        ExA.SFXName = "blast1";

        SFXLaunchCommand ExB = new SFXLaunchCommand();
        ExB.LaunchTime = 15.0f;
        ExB.SFXName = "in1";

        SFXLaunchCommands.Add(ExA);
        SFXLaunchCommands.Add(ExB);
    }
}
