using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    public void GoBackInBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
       Application.ExternalEval("history.back()");
#endif
        Debug.Log("GoBackInBrowser");
    }
}