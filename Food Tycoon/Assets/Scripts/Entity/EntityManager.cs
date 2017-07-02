using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    //TODO: remove the [SerializeField] once the mechanic to select current entity is done
    [SerializeField] private Entity CurrentEnity;

    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError(gameObject + " was a second Instance! and was destroyed for it!");
            Destroy(gameObject);
        }
    }


    public void SetTarget(Vector3 _Target)
    {
        if(CurrentEnity != null)
            CurrentEnity.SetTarget(_Target);
    }

    public void SetCurrentEntity(Entity _Entity)
    {
        if (_Entity == null)
            return;

        CurrentEnity = _Entity;
    }


}
