using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CalculationSumoP2PSpawner : MonoBehaviour
{
    [SerializeField] private CalculationSumoP2PPlayerManager _playerPrefabs;
    [SerializeField] private Transform _playerHolder;

    public void SpawnPlayer()
    {
        CalculationSumoP2PPlayerManager player = PhotonNetwork.Instantiate(
            _playerPrefabs.name, Vector3.zero, Quaternion.identity)
            .GetComponent<CalculationSumoP2PPlayerManager>();

        player.gameObject.SetActive(true);
        player.transform.SetParent(_playerHolder);
        player.InitPlayer(this);
    }

    [SerializeField] private Text _questionText;
    [SerializeField] private CalculationSumoAnwserController[] _answers;

    private CalculationProblem currentProblem;
    public int correctAnswer;

    private int operationCount = 0;

    public void GenerateAndDisplayQuestion()
    {
        for (int i = 0; i < 4; i++)
        {
            _answers[i].gameObject.SetActive(false);
        }
        _questionText.gameObject.SetActive(false);
        int level = operationCount/5;
        currentProblem = CalculationGenerator.GenerateProblem(level);
        operationCount++;
        operationCount %= 25;
        correctAnswer = currentProblem.Result;

        _questionText.text = FormatProblem(currentProblem);

        List<int> answers = GenerateAnswerChoices(correctAnswer);
        for (int i = 0; i < 4; i++)
        {
            _answers[i].SetValueText(answers[i]);
        }
        _questionText.gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            _answers[i].gameObject.SetActive(true);
            _answers[i].EnableCollider();
        }
    }

    public void EnableColliderAllAnwsers()
    {
        for (int i = 0; i < 4; i++)
        {
            _answers[i].EnableCollider();
        }
    }

    public void DisableColliderAnwsers()
    {
        for (int i = 0; i < 4; i++)
        {
            _answers[i].DisableCollider();
        }
    }

    private string FormatProblem(CalculationProblem problem)
    {
        string opSymbol = problem.Operation == OperationType.Addition ? "+" : "-";
        return $"{problem.A} {opSymbol} {problem.B} = ?";
    }

    private List<int> GenerateAnswerChoices(int correct)
    {
        HashSet<int> answerSet = new HashSet<int> { correct };

        while (answerSet.Count < 4)
        {
            int fakeAnswer = GenerateFakeAnswer(correct);
            answerSet.Add(fakeAnswer);
        }

        List<int> list = new List<int>(answerSet);
        Shuffle(list);
        return list;
    }

    private int GenerateFakeAnswer(int correct)
    {
        int fake = correct;
        int ones = correct % 10;
        int tens = correct / 10;

        int strategy = Random.Range(0, 4);

        switch (strategy)
        {
            case 0:
                fake = (tens + Random.Range(1, 3)) * 10 + ones;
                break;
            case 1:
                fake = tens * 10 + Random.Range(0, 10);
                break;
            case 2:
                if (correct >= 10 && correct <= 99)
                    fake = ones * 10 + tens;
                else
                    fake = correct + Random.Range(1, 4);
                break;
            case 3:
                if (Random.value < 0.5f)
                    fake = correct + 10;
                else
                    fake = correct + 1;
                break;
        }

        if (fake == correct || fake < 0 || fake > 200)
            fake = correct + Random.Range(2, 9);

        return fake;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public bool CheckAnswer(int selected)
    {
        return selected == correctAnswer;
    }

    public CalculationProblem GetCurrentProblem()
    {
        return currentProblem;
    }
}

public static class CalculationGenerator
{
    private static System.Random rand = new System.Random();

    public static CalculationProblem GenerateProblem(int level)
    {
        return level switch
        {
            1 => GenerateSimpleProblem(1, 10, allowCarry: false, requireCarry: false),
            2 => GenerateSimpleProblem(1, 20, allowCarry: true, requireCarry: false),
            3 => GenerateSimpleProblem(10, 40, allowCarry: true, requireCarry: true, requiredCarryCount: 1),
            4 => GenerateSimpleProblem(20, 70, allowCarry: true, requireCarry: true, requiredCarryCount: 1, maxCarryCount: 2),
            5 => GenerateSimpleProblem(50, 100, allowCarry: true, requireCarry: true, requiredCarryCount: 2),
            _ => GenerateSimpleProblem(1, 10, allowCarry: false, requireCarry: false),
        };
    }

    private static CalculationProblem GenerateSimpleProblem(int min, int max, bool allowCarry, bool requireCarry = false, int requiredCarryCount = 1, int maxCarryCount = 2)
    {
        while (true)
        {
            int a = rand.Next(min, max);
            int b = rand.Next(min, max);
            OperationType op = (OperationType)rand.Next(0, 2); // Random cộng/trừ

            int result;

            if (op == OperationType.Subtraction)
            {
                if (a < b) (a, b) = (b, a); // đảm bảo hiệu không âm
                result = a - b;
            }
            else
            {
                result = a + b;
            }

            int carryCount = (op == OperationType.Addition) ? CountCarry(a, b) : 0;

            if (requireCarry && op == OperationType.Addition)
            {
                if (carryCount >= requiredCarryCount && carryCount <= maxCarryCount)
                {
                    return new CalculationProblem { A = a, B = b, Operation = op, Result = result };
                }
            }
            else if (!allowCarry && carryCount == 0)
            {
                return new CalculationProblem { A = a, B = b, Operation = op, Result = result };
            }
            else if (allowCarry || op == OperationType.Subtraction)
            {
                return new CalculationProblem { A = a, B = b, Operation = op, Result = result };
            }
        }
    }

    private static int CountCarry(int a, int b)
    {
        int carry = 0;
        int carryCount = 0;

        while (a > 0 || b > 0)
        {
            int sum = (a % 10) + (b % 10) + carry;
            if (sum >= 10)
            {
                carry = 1;
                carryCount++;
            }
            else
            {
                carry = 0;
            }

            a /= 10;
            b /= 10;
        }

        return carryCount;
    }
}

public enum OperationType
{
    Addition,
    Subtraction
}

public struct CalculationProblem
{
    public int A;
    public int B;
    public OperationType Operation;
    public int Result;

    public override string ToString() => $"{A} {(Operation == OperationType.Addition ? "+" : "-")} {B} = {Result}";
}

