using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitPickerP2PFruitController : MonoBehaviour
{
    [SerializeField] private Image _fruitImage;
    [SerializeField] private Collider2D _collider2D;
    public bool IsTaken = false;

    private PhotonView _pv;

    public int ViewID => _pv.ViewID;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void RPC_MarkAsTaken()
    {
        IsTaken = true;
        // Tắt collider để chặn tương tác
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Optional: feedback hình ảnh
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 0.5f;
    }

    [PunRPC]
    public void RPC_ResetFruit()
    {
        IsTaken = false;
        // Bật lại collider để cho phép tương tác
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // Optional: feedback hình ảnh
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 1f;
    }
}
