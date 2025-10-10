using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationSumoP2PSumoController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform sumo;
    [SerializeField] private CalculationSumoAnimSumo _rightSumo;
    [SerializeField] private CalculationSumoAnimSumo _leftSumo;

    [Header("Config")]
    [SerializeField] private float moveDistance = 200f;   // distance for each move
    [SerializeField] private float leftTargetLocalX = -520f;
    [SerializeField] private float rightTargetLocalX = 520f;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float distanceOffset = 5f;

    public event Action OnMoveLeftComplete;
    public event Action OnMoveRightComplete;

    [SerializeField] private ParticleSystem _goodPs;
    [SerializeField] private ParticleSystem _gameOverPs;

    private Queue<IEnumerator> moveQueue = new Queue<IEnumerator>();
    private bool isRunning = false;
    private bool isFinished = false;

    private void Start()
    {
        // Đăng ký hiệu ứng mặc định
        OnMoveLeftComplete += () => { _gameOverPs.Play(); _gameOverPs.GetComponent<AudioSource>().Play(); };
        OnMoveRightComplete += () => { _goodPs.Play(); _goodPs.GetComponent<AudioSource>().Play(); };
    }

    public void ResetSumo()
    {
        sumo.localPosition = new Vector3(0, sumo.localPosition.y, sumo.localPosition.z);
        DOTween.Kill(sumo);
        moveQueue.Clear();
        isRunning = false;
    }

    // --- PUBLIC API ---
    public void MoveLeft()
    {
        moveQueue.Enqueue(MoveLeftRoutine());
        TryRunQueue();
    }

    public void MoveRight()
    {
        moveQueue.Enqueue(MoveRightRoutine());
        TryRunQueue();
    }

    private void TryRunQueue()
    {
        if (isFinished) 
        {
            moveQueue.Clear();
            return;
        } 
        if (!isRunning && moveQueue.Count > 0)
        {
            StartCoroutine(RunQueue());
        }
    }

    private IEnumerator RunQueue()
    {
        isRunning = true;
        while (moveQueue.Count > 0)
        {
            yield return StartCoroutine(moveQueue.Dequeue());
        }
        isRunning = false;
    }

    // --- ROUTINES ---
    private IEnumerator MoveLeftRoutine()
    {
        if (HasReachedLeftLimit()) yield break;

        float targetX = Mathf.Max(sumo.localPosition.x - moveDistance, leftTargetLocalX);
        bool finished = false;

        sumo.DOLocalMoveX(targetX, moveDuration).OnComplete(() =>
        {
            finished = true;
            OnMoveLeftComplete?.Invoke();
            _leftSumo?.PlayPushAnim();
        });

        yield return new WaitUntil(() => finished);
    }

    private IEnumerator MoveRightRoutine()
    {
        if (HasReachedRightLimit()) yield break;

        float targetX = Mathf.Min(sumo.localPosition.x + moveDistance, rightTargetLocalX);
        bool finished = false;

        sumo.DOLocalMoveX(targetX, moveDuration).OnComplete(() =>
        {
            finished = true;
            OnMoveRightComplete?.Invoke();
            _rightSumo?.PlayPushAnim();
        });

        yield return new WaitUntil(() => finished);
    }

    // --- LIMIT CHECKS ---
    private bool HasReachedRightLimit()
    {
        isFinished = true;
        return sumo.localPosition.x >= rightTargetLocalX - distanceOffset;
    }

    private bool HasReachedLeftLimit()
    {
        isFinished = true;
        return sumo.localPosition.x <= leftTargetLocalX + distanceOffset;
    }
}
