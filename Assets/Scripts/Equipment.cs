using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public EquipmentType type; // Type of equipment (armor, boots, gloves, weapon)
    public int tier; // Tier of the equipment (1, 2, 3, ...)
    public GameObject icon; // Icon for the equipment
    public int statValue; // Stat value for the equipment (can be expanded later)

    public enum EquipmentType
    {
        Armor,
        Boots,
        Gloves,
        Weapon
    }
}
