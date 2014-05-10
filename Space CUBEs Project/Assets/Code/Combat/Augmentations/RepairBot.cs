// Steve Yeager
// 4.21.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Repairs health over time.
/// </summary>
public class RepairBot : Augmentation
{
    #region Public Fields

    /// <summary>How long to delay regen in seconds.</summary>
    public float delay = 2f;

    /// <summary>Health/s to regen.</summary>
    public float regenSpeed = 5f;
    
    #endregion

    #region Private Fields

    private ShieldHealth playerHealth;

    #endregion


    #region Augmentation Methods

    public override void Initialize(Player player)
    {
        playerHealth = player.myHealth;
        playerHealth.HealthUpdateEvent += OnHealthUpdate;
    }


    public override Augmentation Bake(GameObject player)
    {
        RepairBot comp = player.AddComponent<RepairBot>();
        comp.delay = delay;
        comp.regenSpeed = regenSpeed;

        return comp;
    }

    #endregion

    #region Private Methods

    private void OnHealthUpdate(object sender, HealthUpdateArgs args)
    {
        if (!(args.amount < 0)) return;

        StopAllCoroutines();
        StartCoroutine(Heal());
    }


    private IEnumerator Heal()
    {
        yield return new WaitForSeconds(delay);

        while (playerHealth.health < playerHealth.maxHealth)
        {
            playerHealth.ChangeHealth(regenSpeed * Time.deltaTime);
            yield return null;
        }
    }

    #endregion
}