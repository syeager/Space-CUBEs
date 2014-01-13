// Steve Yeager
// 12.5.2013

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Health manager for all objects that can be destroyed.
/// </summary>
public class Health : MonoBase
{
    #region References

    protected Renderer myRenderer;

    #endregion

    #region Public Fields

    /// <summary>Seconds for the hit material.</summary>
    public float healthHitMatTime = 0.5f;
    /// <summary>Can recieve damage?</summary>
    public bool invincible;

    #endregion

    #region Protected Fields

    /// <summary>Changes material from normal to hit and back.</summary>
    protected Job changeMat;
    /// <summary>Material when health is taken away.</summary>
    protected Material HealthHit_Mat;
    /// <summary>Material when gameobject is created.</summary>
    protected Material Normal_Mat;

    #endregion

    #region Properties

    /// <summary>Max health allowed.</summary>
    public float maxHealth;// { get; protected set; }
    /// <summary>Current health.</summary>
    public float health;// { get; protected set; }

    #endregion

    #region Events

    /// <summary>Sent when health is changed.</summary>
    public EventHandler<HealthUpdateArgs> HealthUpdateEvent;
    /// <summary>Sent when health reaches 0.</summary>
    public EventHandler<DieArgs> DieEvent;

    #endregion


    #region Monobehaviour Overrides

    protected virtual void Awake()
    {
        myRenderer = renderer;
        HealthHit_Mat = GameResources.Main.HealthHit_Mat;
    }


    private void OnDestroy()
    {
        if (changeMat != null)
        {
            changeMat.Kill();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set health to max health.
    /// </summary>
    public void Initialize()
    {
        health = maxHealth;

        Normal_Mat = myRenderer.material;
    }


    /// <summary>
    /// Set health to max health.
    /// </summary>
    /// <param name="maxHealth">New max health.</param>
    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        Initialize();
    }


    /// <summary>
    /// Recieve hit info from weapon.
    /// </summary>
    /// <param name="sender">Who shot the weapon.</param>
    /// <param name="hitInfo">HitInfo from weapon.</param>
    public virtual void RecieveHit(Ship sender, HitInfo hitInfo)
    {
        if (invincible) return;

        ChangeHealth(hitInfo.damage);

        if (myRenderer == null) return;

        if (changeMat != null)
        {
            changeMat.Kill();
        }
        changeMat = new Job(ChangeMat(HealthHit_Mat));
    }


    /// <summary>
    /// Add to health. Clamped.
    /// </summary>
    /// <param name="amount">Amount of health added./param>
    /// <returns>True, if health is 0.</returns>
    public bool ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
        if (HealthUpdateEvent != null)
        {
            HealthUpdateEvent(this, new HealthUpdateArgs(maxHealth, amount, health));
        }

        if (health == 0f)
        {
            if (DieEvent != null)
            {
                DieEvent(this, new DieArgs(false));
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Change to hit material for healthHitMatTime.
    /// </summary>
    /// <param name="mat">Material to switch to.</param>
    protected IEnumerator ChangeMat(Material mat)
    {
        myRenderer.material = mat;
        yield return new WaitForSeconds(healthHitMatTime);
        myRenderer.material = Normal_Mat;
    }

    #endregion
}