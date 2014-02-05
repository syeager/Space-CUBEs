// Steve Yeager
// 1.14.2014

/// <summary>
/// Holds all info for a CUBE piece except pieces.
/// </summary>
public struct CUBEInfo
{
    public readonly int id;
    public readonly string name;
    public readonly CUBE.Types type;
    // Combat Stats
    public readonly float health;
    public readonly float shield;
    public readonly float speed;
    // Part Stats
    public readonly int rarity;
    public readonly int price;
    // System Stats

    // Agmentation Stats


    public CUBEInfo(int id, string name, CUBE.Types type, float health, float shield, float speed, int rarity, int price)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.health = health;
        this.shield = shield;
        this.speed = speed;
        this.rarity = rarity;
        this.price = price;
    }

    public override string ToString()
    {
        return id + ": " + name + " - " + type + " | " + health + "," + shield + "," + speed + "." + rarity + "," + price;
    }
}