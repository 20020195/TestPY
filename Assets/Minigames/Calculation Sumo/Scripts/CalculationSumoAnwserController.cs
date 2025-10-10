using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationSumoAnwserController : MonoBehaviour
{
    [SerializeField] private Text _valueText;
    [SerializeField] private Collider2D _cl2d;

    private void Reset()
    {
        _valueText = GetComponentInChildren<Text>();
        _cl2d = GetComponent<Collider2D>();
    }

    public void EnableCollider()
    {
        _cl2d.enabled = true;
    }

    public void DisableCollider()
    {
        _cl2d.enabled = false;
    }

    public int GetValue()
    {
        return int.Parse(_valueText.text);
    }

    public void SetValueText(int value)
    {
        _valueText.text = value.ToString();
    }
}
