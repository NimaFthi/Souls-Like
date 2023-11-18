using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Stat", fileName = "SO_Stat")]
public class SO_StatData : ScriptableObject
{
    [Header("Tag")]
    public string tag;

    [Header("Values"), Space]
    [SerializeField] private float value;
    [SerializeField] private float regenValue;

    #region Properties
    
    public float Value => value;
    public float RegenValue => regenValue;

    #endregion

    //this is for adding value after leveling up
    public void AddMaxValue(float valueToAdd)
    {
        value += valueToAdd;
    }

    public void AddRegenValue(float valueToAdd)
    {
        regenValue += valueToAdd;
    }
}
