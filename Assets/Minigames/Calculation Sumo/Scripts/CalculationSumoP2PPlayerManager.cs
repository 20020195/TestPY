using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CalculationSumoP2PPlayerManager : MonoBehaviour
{
    [SerializeField] private CalculationSumoP2PSpawner _spawner;
    [SerializeField] private CalculationSumoP2PSumoController _sumoController;

    public int Score;
    public int Mistakes;

    private PhotonView _pv;
    public bool IsMine => _pv.IsMine;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        _sumoController = FindFirstObjectByType<CalculationSumoP2PSumoController>();
    }


    public void InitPlayer(CalculationSumoP2PSpawner spawner)
    {
        if (!IsMine) return;
        _spawner = spawner;
        Score = 0;
        Mistakes = 0;
    }

    public void StartGame()
    {
        if (!IsMine) return;

        _spawner.GenerateAndDisplayQuestion();
    }

    public void SubmitAnswer(int answer)
    {
        if (!IsMine) return;

        if (answer == _spawner.correctAnswer)
        {
            Score++;
            _sumoController.MoveRight();
            _spawner.GenerateAndDisplayQuestion();
        }
        else
        {
            Mistakes++;
        }

    }
}
