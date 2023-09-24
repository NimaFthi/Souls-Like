using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitterScript : MonoBehaviour
{
    [Header("Components"), Space]
    [SerializeField] private List<Collider> hitColliders;
    
    [Header("Unity events"), Space]
    public UnityEvent<GameObject> OnHit;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Enemy")) return;
        
        OnHit?.Invoke(other.gameObject);
    }

    public void ChangeCollidersState(bool newState)
    {
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.enabled = newState;
        }
    }
}
