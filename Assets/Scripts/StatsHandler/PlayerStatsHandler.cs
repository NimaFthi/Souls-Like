using System;
using System.Collections;
using UnityEngine;

public class PlayerStatsHandler : MonoBehaviour
{
    [Header("Components"), Space] 
    [SerializeField] private PlayerData playerData;
    
    [Header("SO_Stats"), Space]
    [SerializeField] private Stat _hpStat;
    [SerializeField] private StaminaStat _staminaStat;

    [Header("Stamina Setting"), Space]
    private Coroutine _staminaRegenDelayCoroutine;
    public bool canUseStamina => _staminaStat.HaveStaminaToUse();
    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        playerData.inputManager.OnSprintingInputChange += HandleIsRunning;
    }

    private void OnDisable()
    {
        playerData.inputManager.OnSprintingInputChange -= HandleIsRunning;

    }

    private void Init()
    {
        var hudManager = HUDManager.Instance; 
        _hpStat.Init(hudManager.hpHUDStatView);
        _staminaStat.Init(hudManager.staminaHUDStatView);
    }

    private void Update()
    {
        HandleStaminaStates();
    }

    private void HandleStaminaStates()
    {
        switch (_staminaStat.staminaUsageType)
        {
            case EnmStaminaUsageType.Normal:
                _staminaStat.HandleRegen();
                break;
            case EnmStaminaUsageType.Running:
                _staminaStat.HandleRunningStaminaCost();

                if (!_staminaStat.HaveStaminaToUse())
                {
                    StaminaRegenDelay();
                } 
                break;
        }
    }

    #region Hp

    public void TakeDamage(float damageTaken)
    {
        _hpStat.AddValue(damageTaken);
    }

    public void Heal(float healAmount)
    {
        _hpStat.AddValue(healAmount);
    }

    #endregion

    #region Stamina

    public void UseStamina(float staminaCost)
    {
        _staminaStat.AddValue(-staminaCost);
        StaminaRegenDelay();
    }

    public bool DoHaveMinStaminaToPerform(float minStaminaToPerform)
    {
        return _staminaStat.IsEqualOrHigherTo(minStaminaToPerform);
    }

    //This is for adding delay between times we use stamina and we regen it
    private void StaminaRegenDelay()
    {
        playerData.inputManager.CancelSprintInput();
        if (_staminaRegenDelayCoroutine != null)
        {
            StopCoroutine(_staminaRegenDelayCoroutine);
        }
        _staminaRegenDelayCoroutine = StartCoroutine(_staminaStat.RegenDelayCoroutine());
    }

    private void HandleIsRunning(bool isRunning)
    {
        if (_staminaStat.HaveStaminaToUse() && isRunning)
        {
            if (_staminaRegenDelayCoroutine != null)
            {
                StopCoroutine(_staminaRegenDelayCoroutine);
            }
            _staminaStat.staminaUsageType = EnmStaminaUsageType.Running;
        }
        else
        {
            StaminaRegenDelay();
        }
    }

    #endregion
}

[Serializable]
public class Stat
{
    [Header("SO_Stats")] 
    public EnmStatsType type;
    [SerializeField] private SO_StatData statData;
    private HUDStatView _hudView;

    [Header("Values"), Space]
    [SerializeField] protected float currentValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float regenValue;

    [Header("Setting"), Space] 
    public bool doRegen;

    public void Init(HUDStatView hudView)
    {
        maxValue = statData.Value;
        regenValue = statData.RegenValue;
        currentValue = maxValue;

        _hudView = hudView;
        _hudView.Init(maxValue);
    }

    public void ResetStat()
    {
        currentValue = maxValue;
    }

    //For both positive and negative values 
    public void AddValue(float valueToAdd)
    {
        currentValue = Mathf.Clamp(currentValue + valueToAdd, 0f, maxValue);
        _hudView.UpdateBarFill(currentValue/maxValue);
    }

    public void HandleRegen()
    {
        if(!doRegen) return;
        if (currentValue < maxValue)
        {
            AddValue(regenValue * Time.deltaTime);
        }
    }
}

[Serializable]
public class StaminaStat : Stat
{
    [Header("Values"), Space] 
    [SerializeField] private float runningStaminaCost;
    
    [Header("State"), Space]
    [SerializeField] private float regenDelayTime;
    public EnmStaminaUsageType staminaUsageType = EnmStaminaUsageType.Normal;

    public void HandleRunningStaminaCost()
    {
        AddValue(-runningStaminaCost * Time.deltaTime);
    }

    public bool HaveStaminaToUse()
    {
        return currentValue > 0;
    }

    public bool IsEqualOrHigherTo(float value)
    {
        return value < currentValue;
    }
    
    public IEnumerator RegenDelayCoroutine()
    {
        staminaUsageType = EnmStaminaUsageType.WithOutRegen;
        
        yield return new WaitForSeconds(regenDelayTime);

        staminaUsageType = EnmStaminaUsageType.Normal;
    }
}

public enum EnmStatsType
{
    Hp = 0,
    Stamina = 1
}

public enum EnmStaminaUsageType
{
    Normal = 0,
    WithOutRegen = 1,
    Running = 2
}
