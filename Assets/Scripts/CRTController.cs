using System.Collections;
using BrewedInk.CRT;
using UnityEngine;

public class CRTController : MonoBehaviour
{
    [Header("Scene References")]
    public CRTCameraBehaviour crtCamera;

    [Header("Asset References")]
    public CRTDataObject[] demoValues;

    // Runtime data
    private int _currentDemoIndex;
    
    private void Start()
    {
        if (crtCamera == null)
        {
            Debug.LogWarning($"The {nameof(crtCamera)} field hasn't been assigned!" );
        }
        
        // Initial zoom to demonstrate functionality.
        // SetZoom(2, 1.2f, 2);
    }

    public void ChangeCRTSetting(int index)
    {
        if (index >= 0 && index < demoValues.Length)
        {
            if (crtCamera == null)
            {
                Debug.LogError("CRT Camera is not assigned in CRTController.");
                return;
            }
            
            crtCamera.data = demoValues[index].data;
        }
        else
        {
            Debug.LogError("Invalid preset index: " + index);
        }
    }

    public void LerpToNextPreset()
    {
        var curr = demoValues[_currentDemoIndex];
        _currentDemoIndex = (_currentDemoIndex + 1) % demoValues.Length;
        var next = demoValues[_currentDemoIndex];

        float duration = 5;  // Increased duration to 5 seconds for a slower transition
        IEnumerator Animation()
        {
            var startTime = Time.realtimeSinceStartup;
            var endTime = startTime + duration;

            while (Time.realtimeSinceStartup < endTime)
            {
                var t = 1 - ((endTime - Time.realtimeSinceStartup) / duration);
                var x = CRTData.Lerp(curr.data, next.data, t);
                crtCamera.data = x;
                yield return null;
            }

            crtCamera.data = next.data;
        }
    
        StartCoroutine(Animation());
    }


    public void SetZoom(float startZoom, float endZoom, float duration)
    {
        StartCoroutine(Animation());

        IEnumerator Animation()
        {
            crtCamera.data.zoom = startZoom;
            var startTime = Time.realtimeSinceStartup;
            var endTime = startTime + duration;

            while (Time.realtimeSinceStartup < endTime)
            {
                var t = 1 - ((endTime - Time.realtimeSinceStartup) / duration);
                crtCamera.data.zoom = Mathf.Lerp(startZoom, endZoom, t);
                yield return null;
            }

            crtCamera.data.zoom = endZoom;
        }
    }
}