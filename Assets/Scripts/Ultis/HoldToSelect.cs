using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldToSelect : MonoBehaviour
{
    [SerializeField] private Image _holdImage;
    [SerializeField] private float _holdTime = 1f;
    [SerializeField] private float _delayTimeToHold = 0.5f;
    public bool canHold = true;

    private float _countTime = 0;
    private Coroutine _holdCoroutine;

    public System.Action<GameObject, GameObject> OnStartCount;
    public System.Action<GameObject, GameObject> OnCounting;
    public System.Action<GameObject, GameObject> OnCountEnd;
    public System.Action<GameObject, GameObject> OnStopHold;

    public void TryToStartCountToHold(Collider2D collision)
    {
        if (_holdCoroutine == null)
        {
            _holdCoroutine = StartCoroutine(HoldCoroutine(collision));
        }
    }

    public void StartCountToHold(Collider2D collision)
    {
        _countTime = 0;
        _holdImage.fillAmount = 0;

        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
        }

        _holdCoroutine = StartCoroutine(HoldCoroutine(collision));
    }

    public void TryToStopCountToHold(Collider2D collision)
    {
        if (_holdCoroutine != null)
        {
            StopCountToHold(collision);
        }
    }

    public void StopCountToHold(Collider2D collision)
    {
        _countTime = 0;
        _holdImage.fillAmount = 0;

        OnStopHold?.Invoke(gameObject, collision.gameObject);

        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }

        canHold = true;
    }

    public void ForceStopHold()
    {
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }

        _countTime = 0;
        _holdImage.fillAmount = 0;
        OnStopHold?.Invoke(gameObject, null);
        canHold = true;
    }

    private IEnumerator HoldCoroutine(Collider2D collision)
    {
        yield return new WaitForSeconds(_delayTimeToHold);
        OnStartCount?.Invoke(gameObject, collision.gameObject);

        while (_countTime < _holdTime && canHold)
        {
            _countTime += Time.deltaTime;
            _holdImage.fillAmount = _countTime / _holdTime;

            OnCounting?.Invoke(gameObject, collision.gameObject);

            yield return null;
        }

        if (_countTime >= _holdTime)
        {
            canHold = false;
            OnCountEnd?.Invoke(gameObject, collision.gameObject);
        }

        _holdCoroutine = null;
        _countTime = 0;
        _holdImage.fillAmount = 0;
    }


    public void SetCanSelect(bool value)
    {
        canHold = value;
    }

    private void Update()
    {

    }
}
