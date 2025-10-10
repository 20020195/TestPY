using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;

public class FruitPickerP2PUIManager : MonoBehaviour
{
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text opponentScoreText;

    [SerializeField] private Image myScoreSlider;
    [SerializeField] private Image opponentScoreSlider;
    [SerializeField] private List<Button> quitBtns = new List<Button>();

    [SerializeField] private GameObject resultPopup;
    [SerializeField] private GameObject loadingPopup;

    [SerializeField] private Text myResultText;
    [SerializeField] private Text opponentResultText;
    [SerializeField] private Button rematchBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private GameObject playerLeftPopup;
    [SerializeField] private Text playerLeftText;


    [SerializeField] private BackButtonHandler backButtonHandler;

    private FruitPickerP2PPlayerManager localPlayer;
    private FruitPickerP2PPlayerManager opponentPlayer;

    private void OnEnable()
    {
        backButtonHandler = FindFirstObjectByType<BackButtonHandler>();

        myScoreText.text = "0";
        opponentScoreText.text = "0";

        myScoreSlider.fillAmount = 0.5f;
        opponentScoreSlider.fillAmount = 0.5f;
        FruitPickerP2PPlayerManager.OnScoreChanged += UpdateScoreUI;

        rematchBtn.onClick.AddListener(() => { resultPopup.SetActive(false); loadingPopup.SetActive(true); FruitPickerP2PGameManager.Instance.Rematch(); });
        continueBtn.onClick.AddListener(() => { resultPopup.SetActive(false); loadingPopup.SetActive(true);});

        foreach (var btn in quitBtns)
        {
            btn.onClick.AddListener(backButtonHandler.GoBackInBrowser);
        }
    }

    public void ResetUI()
    {
        loadingPopup.SetActive(false);
        resultPopup.SetActive(false);
        myScoreText.text = "0";
        opponentScoreText.text = "0";
        myScoreSlider.fillAmount = 0.5f;
        opponentScoreSlider.fillAmount = 0.5f;
    }

    private void UpdateScoreUI(FruitPickerP2PPlayerManager player)
    {
        if (localPlayer == null || opponentPlayer == null)
        {
            foreach (var pm in FindObjectsOfType<FruitPickerP2PPlayerManager>())
            {
                if (pm.IsMine) localPlayer = pm;
                else opponentPlayer = pm;
            }
        }

        if (localPlayer == null) return;

        int myScore = localPlayer.Score;
        int opponentScore = opponentPlayer == null ? 0 : opponentPlayer.Score;
        int total = myScore + opponentScore;

        myScoreText.text = myScore.ToString();
        opponentScoreText.text = opponentScore.ToString();

        if (total == 0)
        {
            myScoreSlider.fillAmount = 0.5f;
            opponentScoreSlider.fillAmount = 0.5f;
        }
        else
        {
            myScoreSlider.DOFillAmount((float)myScore / total, 0.2f);
            opponentScoreSlider.DOFillAmount((float)opponentScore / total, 0.2f);
            /*myScoreSlider.fillAmount = (float)myScore / total;
            opponentScoreSlider.fillAmount = (float)opponentScore / total;*/
        }
    }

    public void ShowResult(FruitPickerP2PPlayerManager mine, FruitPickerP2PPlayerManager op)
    {
        myResultText.text = mine.Score.ToString();
        opponentResultText.text = op == null ? "0" : op.Score.ToString();

        rematchBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        continueBtn.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        StartCoroutine(ShowResultPopupCoroutine());
    }

    IEnumerator ShowResultPopupCoroutine()
    {
        yield return new WaitForSeconds(1f);
        resultPopup.SetActive(true);
    }

    internal void ShowPlayerLeftPopup(string playerName)
    {
        playerLeftText.text = $"<color=#FF0000>{playerName}</color> đã rời trò chơi.\nHẹn gặp lại lần sau.";

        playerLeftPopup.SetActive(true);
    }
}
