using Scripts.Utils;
using UnityEngine;

public class HUDManager : SingletonMonoBehaviour<HUDManager>
{
    [Header("StatsUI"), Space] 
    public HUDStatView hpHUDStatView;
    public HUDStatView staminaHUDStatView;
}
