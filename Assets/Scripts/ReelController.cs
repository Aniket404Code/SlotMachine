using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a single slot reel:
/// - Spins symbols
/// - Animates movement
/// - Stops smoothly
/// - Tracks center symbol
/// </summary>
public class ReelController : MonoBehaviour
{
    #region Inspector References

    [Header("Slot Images")]
    [SerializeField] private Image topSlot;
    [SerializeField] private Image middleSlot;
    [SerializeField] private Image bottomSlot;

    [Header("Data")]
    [SerializeField] private SlotSymbolDatabase database;

    [Header("Animation")]
    [SerializeField] private RectTransform slotContainer;

    [SerializeField] private float moveDistance = 40f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float stopDamping = 0.95f;

    #endregion

    #region Public Properties

    public int CurrentCenterIndex { get; private set; }

    public SlotSymbolDatabase Database => database;

    #endregion

    #region Private Variables

    private bool isSpinning;

    private float currentSpeed;

    private Vector2 startPosition;

    private Coroutine spinCoroutine;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeSymbols();

        startPosition = slotContainer.anchoredPosition;
    }

    #endregion

    #region Spin Logic


    public void StartSpin()
    {
        isSpinning = true;

        currentSpeed = moveSpeed;

        // Prevent duplicate spin coroutines.
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
        }

        spinCoroutine = StartCoroutine(SpinRoutine());
    }

    public void StopSpin()
    {
        StartCoroutine(SlowStopRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        int currentIndex = 0;

        float currentMove = 0f;

        while (isSpinning)
        {
            currentMove += currentSpeed;

            slotContainer.anchoredPosition =
                startPosition + Vector2.down * currentMove;

            // Once move distance is reached,
            // reset position and update symbols.
            if (currentMove >= moveDistance)
            {
                currentMove = 0f;

                slotContainer.anchoredPosition = startPosition;

                currentIndex++;

                UpdateVisibleSymbols(currentIndex);
            }

            yield return null;
        }
    }

    private void UpdateVisibleSymbols(int currentIndex)
    {
        topSlot.sprite =
            database.symbols[
                GetWrappedIndex(currentIndex - 1)
            ];

        middleSlot.sprite =
            database.symbols[
                GetWrappedIndex(currentIndex)
            ];

        bottomSlot.sprite =
            database.symbols[
                GetWrappedIndex(currentIndex + 1)
            ];

        CurrentCenterIndex =
            GetWrappedIndex(currentIndex);
    }

    private IEnumerator SlowStopRoutine()
    {
        while (currentSpeed > 0.2f)
        {
            currentSpeed *= stopDamping;

            yield return null;
        }

        currentSpeed = 0f;

        isSpinning = false;

        // Snap back to original position
        // to avoid visual drifting.
        slotContainer.anchoredPosition = startPosition;
    }

    #endregion

    #region Utility Methods

    private Sprite GetRandomSprite()
    {
        int randomIndex =
            Random.Range(0, database.symbols.Length);

        return database.symbols[randomIndex];
    }

    private int GetWrappedIndex(int index)
    {
        if (index < 0)
        {
            index += database.symbols.Length;
        }

        return index % database.symbols.Length;
    }

    private void InitializeSymbols()
    {
        topSlot.sprite = GetRandomSprite();
        middleSlot.sprite = GetRandomSprite();
        bottomSlot.sprite = GetRandomSprite();
    }

    #endregion
}