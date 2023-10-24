using System;
using System.Collections.Generic;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    [Header("Components"), Space]
    [SerializeField] private List<Collider> hitterColliders;

    #region Events

    public Action<GameObject> OnHit;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Enemy")) return;
        
        Debug.Log("Hit");
        OnHit?.Invoke(other.gameObject);
    }

    public void ChangeColliderState(bool newState)
    {
        foreach (var collider in hitterColliders)
        {
            collider.enabled = newState;
        }
    }
}
