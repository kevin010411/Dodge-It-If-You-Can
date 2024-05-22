using Kino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    public static GlitchController Instance;
    [SerializeField] private AnalogGlitch AnalogGlitchComponent;
    [SerializeField] private DigitalGlitch DigitalGlitchComponent;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnalogParameters(Vector4 FourParameters)
    {
        AnalogGlitchComponent.scanLineJitter = FourParameters.x;
        AnalogGlitchComponent.verticalJump = FourParameters.y;
        AnalogGlitchComponent.horizontalShake = FourParameters.z;
        AnalogGlitchComponent.colorDrift = FourParameters.w;
    }

    public void SetDigitalIntensity(float Intensity)
    {
        DigitalGlitchComponent.intensity = Intensity;
    }
}
