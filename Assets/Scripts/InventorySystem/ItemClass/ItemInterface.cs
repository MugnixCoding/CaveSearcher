namespace InventorySystem
{
    public interface IStackable
    {
        int stackLimit { get; set; }
    }
    public interface IDiscardable
    {
        void DiscardItem();
    }
    public interface IConsume
    {
        void ConsumeItem();
    }
}
