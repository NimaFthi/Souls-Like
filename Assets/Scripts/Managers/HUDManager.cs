using Scripts.Utils;
using UnityEngine;

public class HUDManager : SingletonMonoBehaviour<HUDManager>
{
    [Header("StatsUI"), Space] 
    public HUDStatView hpHUDStatView;
    public HUDStatView staminaHUDStatView;

    [Header("LockOn UI"), Space] 
    [SerializeField] private GameObject lockOnUIGameObject;

    public void SetLockOnUIState(bool state)
    {
        //Not that good handle it with a canvas on the LockOn target maybe 
        lockOnUIGameObject.SetActive(state);
    }
}
