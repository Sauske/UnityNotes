using UnityEngine;
using System.Collections;
//[System.Serializable]
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GlobalSettings : MonoBehaviour
{

    public bool m_bFog = false;
    public Color m_FogColor;
    public FogMode m_FogMode;
    public float m_FogDensity;
    public float m_LinearFogStart;
    public float m_LinearFogEnd;
    public Color m_AmbientLight = new Color(0.4f, 0.4f, 0.4f);

    void Start()
    {
        ApplySetting();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            ApplySetting();
        }
#endif
    }

    public void ApplySetting()
    {
        RenderSettings.fog = m_bFog;
        RenderSettings.fogColor = m_FogColor;
        RenderSettings.fogMode = m_FogMode;
        RenderSettings.fogDensity = m_FogDensity;
        RenderSettings.fogStartDistance = m_LinearFogStart;
        RenderSettings.fogEndDistance = m_LinearFogEnd;
        RenderSettings.ambientLight = m_AmbientLight;

        //Shader.EnableKeyword("_FOG_OF_WAR_OFF");
    }
}
