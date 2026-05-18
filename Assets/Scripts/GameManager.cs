using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the overall slot machine gameplay:
/// - Spins reels
/// - Handles bets
/// - Calculates winnings
/// - Updates UI
/// - Plays animations
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inspector References

    [Header("Reels")]
    [SerializeField] private ReelController reel1;
    [SerializeField] private ReelController reel2;
    [SerializeField] private ReelController reel3;

    [Header("UI")]
    [SerializeField] private Button spinButton;

    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text payoutText;
    [SerializeField] private TMP_Text totalWinText;

    [Header("Lever")]
    [SerializeField] private RectTransform leverPivot;
    [SerializeField] private Image leverImage;

    [Header("Coins")]
    [SerializeField] private int playerCoins = 1000;
    [SerializeField] private int currentBet = 50;

    [Header("Debug")]
    [SerializeField] private bool forceWin;

    #endregion

    #region Private Variables

    private bool isSpinning;
    private int totalWinAmount;

    #endregion

    #region Unity Methods

    private void Start()
    {
        spinButton.onClick.AddListener(StartGame);

        totalWinText.text = "TOTAL WIN : 0";

        InitializeResultText();
        UpdateUI();
    }

    #endregion

    #region Game Flow

    public void StartGame()
    {
        if (isSpinning)
        {
            return;
        }

        // Prevent negative balance.
        if (playerCoins < currentBet)
        {
            Debug.Log("Not enough coins.");
            return;
        }

        playerCoins -= currentBet;

        UpdateUI();

        StartCoroutine(SpinSequenceRoutine());
    }

    private IEnumerator SpinSequenceRoutine()
    {
        isSpinning = true;
        spinButton.interactable = false;

        // Animate slot machine lever.
        yield return StartCoroutine(AnimateLeverRoutine());

        // Start all reels spinning.
        reel1.StartSpin();
        reel2.StartSpin();
        reel3.StartSpin();

        // Delay before stopping reels.
        yield return new WaitForSeconds(2f);

        reel1.StopSpin();

        yield return new WaitForSeconds(0.5f);

        reel2.StopSpin();

        yield return new WaitForSeconds(0.5f);

        reel3.StopSpin();

        // Wait for final reel to settle.
        yield return new WaitForSeconds(1f);

        CheckWin();

        spinButton.interactable = true;
        isSpinning = false;
    }

    #endregion

    #region Win/Lose Logic

    private void CheckWin()
    {
        int reel1Index = reel1.CurrentCenterIndex;
        int reel2Index = reel2.CurrentCenterIndex;
        int reel3Index = reel3.CurrentCenterIndex;

        bool isMatched =
            reel1Index == reel2Index &&
            reel2Index == reel3Index;

        // Debug option for guaranteed wins.
        if (forceWin)
        {
            isMatched = true;
        }

        if (isMatched)
        {
            HandleWin(reel1Index);
        }
        else
        {
            HandleLose();
        }

        Debug.Log($"Player Coins: {playerCoins}");
    }

    private void HandleWin(int symbolIndex)
    {
        int payout =
            reel1.Database.payouts[symbolIndex];

        bool isJackpot = symbolIndex == 3;

        // Jackpot multiplier.
        if (isJackpot)
        {
            payout *= 5;
        }

        playerCoins += payout;
        totalWinAmount += payout;

        // Update UI
        balanceText.text = $"BALANCE : {playerCoins}";
        totalWinText.text = $"TOTAL WIN : {totalWinAmount}";

        winText.text =
            isJackpot
                ? "JACKPOT!"
                : "YOU WIN!";

        string resultMessage =
            isJackpot
                ? $"JACKPOT! +{payout}"
                : $"WIN +{payout}";

        StartCoroutine(
            ShowResultRoutine(resultMessage, true)
        );
    }

    private void HandleLose()
    {
        winText.text = "NO WIN";

        StartCoroutine(
            ShowResultRoutine("TRY AGAIN", false)
        );
    }

    #endregion

    #region Lever Animation

    private IEnumerator AnimateLeverRoutine()
    {
        Quaternion startRotation = leverPivot.localRotation;

        Quaternion pullRotation =
            Quaternion.Euler(70f, 0f, 0f);

        Vector3 startScale =
            leverImage.rectTransform.localScale;

        Vector3 zoomScale = startScale * 1.5f;

        const float pullDuration = 0.3f;
        const float returnDuration = 0.22f;

        float time = 0f;

        // Pull lever downward.
        while (time < pullDuration)
        {
            float smooth =
                Mathf.SmoothStep(0f, 1f, time / pullDuration);

            leverPivot.localRotation =
                Quaternion.Lerp(startRotation, pullRotation, smooth);

            leverImage.rectTransform.localScale =
                Vector3.Lerp(startScale, zoomScale, smooth);

            time += Time.deltaTime;

            yield return null;
        }

        leverPivot.localRotation = pullRotation;
        leverImage.rectTransform.localScale = zoomScale;

        yield return new WaitForSeconds(0.2f);

        time = 0f;

        // Return lever to original position.
        while (time < returnDuration)
        {
            float smooth =
                Mathf.SmoothStep(0f, 1f, time / returnDuration);

            leverPivot.localRotation =
                Quaternion.Lerp(pullRotation, startRotation, smooth);

            leverImage.rectTransform.localScale =
                Vector3.Lerp(zoomScale, startScale, smooth);

            time += Time.deltaTime;

            yield return null;
        }

        leverPivot.localRotation = startRotation;
        leverImage.rectTransform.localScale = startScale;
    }

    #endregion

    #region Result Animations

    /// Displays result message and triggers animation.
    private IEnumerator ShowResultRoutine(
        string message,
        bool isWin
    )
    {
        resultText.text = message;

        Color color = resultText.color;
        color.a = 1f;

        resultText.color = color;

        RectTransform rect = resultText.rectTransform;

        if (isWin)
        {
            yield return StartCoroutine(
                PlayWinAnimation(rect)
            );
        }
        else
        {
            rect.localScale = Vector3.one * 0.9f;
        }

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(FadeOutResult(rect));
    }

    /// Creates a pop + bounce effect for win text.
    private IEnumerator PlayWinAnimation(RectTransform rect)
    {
        float time = 0f;

        const float duration = 0.4f;

        Vector3 startScale = Vector3.zero;
        Vector3 overshoot = Vector3.one * 1.4f;

        // Pop-in effect.
        while (time < duration)
        {
            float smooth =
                Mathf.Sin((time / duration) * Mathf.PI * 0.5f);

            rect.localScale =
                Vector3.Lerp(startScale, overshoot, smooth);

            // Small shake rotation.
            rect.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    Mathf.Sin(time * 25f) * 5f
                );

            time += Time.deltaTime;

            yield return null;
        }

        // Bounce back to normal size.
        time = 0f;

        while (time < 0.15f)
        {
            rect.localScale =
                Vector3.Lerp(overshoot, Vector3.one, time / 0.15f);

            time += Time.deltaTime;

            yield return null;
        }

        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    /// Fades out result text smoothly.
    private IEnumerator FadeOutResult(RectTransform rect)
    {
        float fade = 1f;

        while (fade > 0f)
        {
            fade -= Time.deltaTime * 2f;

            Color color = resultText.color;
            color.a = fade;

            resultText.color = color;

            yield return null;
        }

        rect.localScale = Vector3.zero;
    }

    #endregion

    #region UI

    private void InitializeResultText()
    {
        resultText.rectTransform.localScale = Vector3.zero;

        Color color = resultText.color;
        color.a = 0f;

        resultText.color = color;
    }

    private void UpdateUI()
    {
        balanceText.text = $"BALANCE : {playerCoins}";
        betText.text = $"BET : {currentBet}";

        payoutText.text =
            "CHERRY = 30\n" +
            "BELL = 20\n" +
            "BAR = 10\n" +
            "7 = 100";
    }

    #endregion
}