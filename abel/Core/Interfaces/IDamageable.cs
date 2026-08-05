namespace Abel_The_Last_Son;

public interface IDamageable
{
    int Health { get; } // current health
    int MaxHealth { get; } // current MaxHealth
    bool IsDead { get; } // is if the character is dead or not true or false 
    
    void TakeDamage(int damage); // take damage see how much damage taken with int number
}