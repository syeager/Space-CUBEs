// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.14
// Edited: 2014.09.20

using UnityEngine;

/// <summary>
/// Holds all info for a CUBE piece except pieces.
/// </summary>
public class CUBEInfo
{
    // General Stats
    public readonly string name;
    public readonly int ID;
    public readonly CUBE.Types type;
    // System Stats
    public readonly CUBE.Subsystems subsystem;
    public readonly CUBE.Brands brand;
    public readonly int grade;
    // Combat Stats
    public readonly float health;
    public readonly float shield;
    public readonly float speed;
    public readonly float damage;
    // Part Stats
    public readonly Vector3 size;
    public readonly int cost;
    public readonly int rarity;
    public readonly int price;


    public CUBEInfo(string name, int id, CUBE.Types type, CUBE.Subsystems subsystem, CUBE.Brands brand, int grade, float health, float shield, float speed, float damage, Vector3 size, int cost, int rarity, int price)
    {
        // General Stats
        this.name = name;
        ID = id;
        this.type = type;
        // System Stats
        this.subsystem = subsystem;
        this.brand = brand;
        this.grade = grade;
        // Combat Stats
        this.health = health;
        this.shield = shield;
        this.speed = speed;
        this.damage = damage;
        // Part Stats
        this.size = size;
        this.cost = cost;
        this.rarity = rarity;
        this.price = price;
    }


    public CUBEInfo(CUBEInfo other)
    {
        // General Stats
        name = other.name;
        ID = other.ID;
        type = other.type;
        // System Stats
        subsystem = other.subsystem;
        brand = other.brand;
        grade = other.grade;
        // Combat Stats
        health = other.health;
        shield = other.shield;
        speed = other.speed;
        damage = other.damage;
        // Part Stats
        size = other.size;
        cost = other.cost;
        rarity = other.rarity;
        price = other.price;
    }


    public override string ToString()
    {
        switch (type)
        {
            case CUBE.Types.System:
                return string.Format("{0}: {1} - {2} | {3} | {4} - {5} | {6} | {7} | {8}",
                                     ID, name, subsystem, brand, grade, size, cost, rarity, price);
            case CUBE.Types.Weapon:
                return string.Format("{0}: {1} - {2} | {3} | {4} | {5} - {6} | {7} | {8} | {9}",
                                     ID, name, health, shield, speed, damage, size, cost, rarity, price);
            default:
                return string.Format("{0}: {1} - {2} | {3} | {4} | {5}",
                                     ID, name, size, cost, rarity, price);
        }
    }
}