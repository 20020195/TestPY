using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

public class FruitPickerP2PPlayerManager : MonoBehaviour
{
    [SerializeField] private FruitPickerP2PPlayerController _controller;
    [SerializeField] private FruitPickerP2PSpawner _spawner;
    [SerializeField] private Image handImage;

    [SerializeField] private Sprite myHandSprite, opponentHandSprite;
    private PhotonView _pv;

    public int Score = 0;
    public bool IsMine => _pv.IsMine;
    public int ViewID => _pv.ViewID;
    public static System.Action<FruitPickerP2PPlayerManager> OnScoreChanged;

    private int numberOfFruits;

    [SerializeField] private FruitPickerP2PPlayerManager opponent;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        _spawner = FindFirstObjectByType<FruitPickerP2PSpawner>();
    }

    private void Start()
    {
        _controller.enabled = _pv.IsMine;
        numberOfFruits = _spawner.NumberOfFruits;
        handImage.sprite = IsMine ? myHandSprite : opponentHandSprite;
    }

    public void ResetPlayer()
    {
        Score = 0;
        opponent.Score = 0;
    }

    public void AddScore(int amount)
    {
        Score += amount;

        OnScoreChanged?.Invoke(this);

        if (opponent == null) FindOpponent();
        int opponentScore = 0;
        if (opponent != null) opponentScore = opponent.Score;
        if (this.Score + opponentScore >= numberOfFruits)
        {
            if (IsMine)
            {
                FruitPickerP2PGameManager.Instance.EndGame(this, opponent);

            }
            else
            {
                FruitPickerP2PGameManager.Instance.EndGame(opponent, this);

            }
        }
    }

    private void FindOpponent()
    {
        FruitPickerP2PPlayerManager[] players = FindObjectsByType<FruitPickerP2PPlayerManager>(FindObjectsSortMode.None);
        foreach (var pm in players)
        {
            if (pm != this && pm.gameObject.activeSelf)
            {
                opponent = pm;
                break;
            }
        }
    }
}
