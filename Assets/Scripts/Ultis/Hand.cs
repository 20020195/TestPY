using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Hand : MonoBehaviour
{
    [SerializeField] protected RectTransform _canvas;

    protected Vector3 _velocity = Vector3.zero;
    protected float _smoothTime = 0.08f;

    public virtual void MoveHand(Landmark v0, Landmark v5, Landmark v17)
    {
        Vector3 targetPos = CalculateHandPos(v0, v5, v17);
        targetPos.z = transform.localPosition.z;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref _velocity, _smoothTime);
    }

    public virtual Vector3 CalculateHandPos(Landmark v0, Landmark v5, Landmark v17)
    {
        float x = (v0.x + v5.x + v17.x) / 3;
        float y = (v0.y + v5.y + v17.y) / 3;
        //x = (0.5f - x) * canvas.sizeDelta.x;

        x = (0.5f - x) * _canvas.sizeDelta.x;
        y = (0.5f - y) * _canvas.sizeDelta.y;

        Vector3 pos = new Vector3(x, y, 0);
        return pos;
    }
}
