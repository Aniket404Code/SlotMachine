using UnityEngine;

/// <summary>
/// Stores all slot machine symbols and their corresponding payouts.
/// Each symbol index must match its payout index.
/// Example:
/// symbols[0] -> Cherry
/// payouts[0] -> 30
/// </summary>
[CreateAssetMenu(menuName = "Slot Machine/Symbol Database")]
public class SlotSymbolDatabase : ScriptableObject
{
    [Header("Slot Symbols")]

    [Tooltip("Sprites used by the reels.")]
    public Sprite[] symbols;

    [Header("Matching Payouts")]

    [Tooltip("Payout values that match each symbol index.")]
    public int[] payouts;
}