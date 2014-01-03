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

    public float healthHitMatTime = 0.5f;
    public bool invincible;

    #endregion

    #region Protected Fields

    protected Job changeMat;
    protected Material HealthHit_Mat;
    protected Material Normal_Mat;

    #endregion

    #region Properties

    public float maxHealth;// { get; protected set; }
    public float health;// { get; protected set; }

    #endregion

    #region Events

    public EventHandler<HealthUpdateArgs> HealthUpdateEvent;
    public EventHandler<DieArgs> DieEvent;

    #endregion


    #region Monobehaviour Overrides

    protected virtual void Awake()
    {
        HealthHit_Mat = GameResources.Main.HealthHit_Mat;
    }


    private void OnDestroy()
    {
        changeMat.Kill();
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        health = maxHealth;

        myRenderer = renderer;
        if (myRenderer == null) return;
        Normal_Mat = myRenderer.material;
    }


    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        Initialize();
    }


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


    public bool ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
        if (HealthUpdateEvent != null)
        {
            HealthUpdateEvent(this, new HealthUpdateArgs(maxHealth, amount, health));
        }
        return health == 0f;
    }

    #endregion

    #region Protected Methods

    protected IEnumerator ChangeMat(Material mat)
    {
        myRenderer.material = mat;
        yield return new WaitForSeconds(healthHitMatTime);
        myRenderer.material = Normal_Mat;
    }

    #endregion
}