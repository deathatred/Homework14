
using System;
using System.Data;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{

    [SerializeField] private LightingPreset _preset;
    [SerializeField, Range(0, 24)] private float _timeOfDay;

    private Light _dirLight;
    private float _daySpeed = 0.1f;


    private void Update()
    {
        if (_preset == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            _timeOfDay += Time.deltaTime * _daySpeed;
            _timeOfDay %= 24;
            UpdateLighting(_timeOfDay / 24f);
        }
        else 
        {
            UpdateLighting(_timeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = _preset.FogColor.Evaluate(timePercent);
        if (_dirLight != null)
        {
            _dirLight.color = _preset.DirectionalColor.Evaluate(timePercent);
            float sunAngle = (timePercent * 360f) - 120f; // -120° зсуває пік дня на 14:00, а захід на ~21:00
            _dirLight.transform.localRotation = Quaternion.Euler(new Vector3(sunAngle, 170f, 0));
        }
        if (_timeOfDay >= 21f || _timeOfDay <= 6f)
        {
            _dirLight.shadows = LightShadows.None;
        }
        else
        {
            _dirLight.shadows = LightShadows.Soft;
        }
    }
    private void OnValidate()
    {
        if (_dirLight != null)
        {
            return;
        }
        if (UnityEngine.RenderSettings.sun != null)
        {
            _dirLight = UnityEngine.RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (light.type == UnityEngine.LightType.Directional)
                {
                    _dirLight = light;
                    return;
                }
            }

        }
    }
}
