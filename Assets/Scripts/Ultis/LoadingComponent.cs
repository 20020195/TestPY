using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingComponent : MonoBehaviour
{
    private float rotateSpeed = -350f;
    [SerializeField] Transform _loadingIcon;
    // UpdateValue is called once per frame
    void Update()
    {
        _loadingIcon.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

}
