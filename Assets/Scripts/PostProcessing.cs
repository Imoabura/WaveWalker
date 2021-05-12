using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    public static PostProcessing instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    // material that's applied when doing post-processing
    [SerializeField] private Material postprocessMaterial;

    private void Start()
    {
    }

    // method which is automatically called by Unity after camera finishes rendering
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // draws pixels from source tex to dest tex
        Graphics.Blit(src, dest, postprocessMaterial);
    }

    void SetTransitionValue(float value)
    {
        if (value < 0 || value > 1)
        {
            Debug.LogWarning($"Transition Value ({value}) must be within [0,1]!");
            return;
        }
        postprocessMaterial.SetFloat("_TransitionLerpValue", value);
    }

    void SetBnWValue(float value)
    {
        if (value < 0 || value > 1)
        {
            Debug.LogWarning($"BNW Value ({value}) must be within [0,1]!");
            return;
        }
        postprocessMaterial.SetFloat("_BnWLerpValue", value);
    }

    public void SetTransitionColor(Color color)
    {
        postprocessMaterial.SetColor("_TransitionColor", color);
    }

    public IEnumerator BlackWhiteFlash(float duration, float maxValue = 1f)
    {
        if (maxValue < 0 || maxValue > 1)
        {
            maxValue = 1f;
        }

        float transitionDuration = duration * .1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            if (timer < transitionDuration)
            {
                SetBnWValue(Mathf.Clamp01(timer / transitionDuration * maxValue));
            }
            else if (timer < duration * .9f)
            {
                SetBnWValue(maxValue);
            }
            else
            {
                SetBnWValue(Mathf.Clamp01((duration - timer) / transitionDuration * maxValue));
            }
            yield return null;
        }

        SetBnWValue(0f);
    }

    public IEnumerator ColorFlash(float duration, float maxValue = 1f)
    {
        if (maxValue < 0 || maxValue > 1)
        {
            maxValue = 1f;
        }

        float halfDuration = duration / 2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            SetTransitionValue((timer < halfDuration) ? 
                Mathf.Clamp01((timer / halfDuration) * maxValue) : 
                Mathf.Clamp01((duration - timer) / halfDuration * maxValue) );
            yield return null;
        }

        SetTransitionValue(0f);
    }
}
