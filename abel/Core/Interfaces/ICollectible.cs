namespace Abel_The_Last_Son;

public interface ICollectible : ICollidable
{
    bool IsCollected { get; }
    
    void Collect(Player player);
}