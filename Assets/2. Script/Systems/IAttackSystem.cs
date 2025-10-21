using UnityEngine;

public interface IAttackSystem 
{
    public void Initialized(Transform player);

    public void UpdateAttack();
}
