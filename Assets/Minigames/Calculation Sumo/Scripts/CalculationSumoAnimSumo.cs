using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationSumoAnimSumo : MonoBehaviour
{
    [SerializeField] private Sprite[] _normalSprite;
    [SerializeField] private Sprite[] _pushSprites;
    [SerializeField] private Sprite[] _fallSprite;

    [SerializeField] private Image _sumoImage;

    private Coroutine _currentAnim;

    public void AnimBySprite(Sprite[] sprites, float animTime, System.Action onComplete = null)
    {
        if (_currentAnim != null)
        {
            StopCoroutine(_currentAnim);
        }

        _currentAnim = StartCoroutine(PlayAnimation(sprites, animTime, onComplete));
    }

    private IEnumerator PlayAnimation(Sprite[] sprites, float animTime, System.Action onComplete = null)
    {
        int frameCount = sprites.Length;
        float frameDuration = animTime / frameCount;

        for (int i = 0; i < frameCount; i++)
        {
            _sumoImage.sprite = sprites[i];
            yield return new WaitForSeconds(frameDuration);
        }

        _currentAnim = null;

        onComplete?.Invoke();
    }


    public void PlayNormalAnim()
    {
        AnimBySprite(_normalSprite, 0f);
    }

    public void PlayPushAnim()
    {
        AnimBySprite(_pushSprites, 0.3f, PlayNormalAnim);
    }

    public void PlayFallAnim()
    {
        AnimBySprite(_fallSprite, 0.1f);
    }
}
