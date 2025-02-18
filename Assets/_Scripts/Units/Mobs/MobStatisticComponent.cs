using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStatisticComponent : MonoBehaviour
{
    public MobStatistics mobStatistics;

    void Awake()
    {
        if (mobStatistics == null)
        {
            mobStatistics = new MobStatistics
            {
                health = 0,
                maxHealth = 0,
                healthRegen = 0,
                attack = 0,
                luck = 0
            };
        }
    }
}
