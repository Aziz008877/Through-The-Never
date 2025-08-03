
[System.Serializable]
public sealed class ItemStack
{
    public ItemSO Item;
    public int Count;

    public ItemStack(ItemSO item, int count = 1)
    {
        Item = item; 
        Count = count;
    }
}