// Steve Yeager
// 12.5.2013

using System;
using System.Collections;
using Annotations;
using UnityEngine;

/// <summary>
/// Health manager for all objects that can be destroyed.
/// </summary>
public class Health : MonoBase
{
    #region References

    public Transform myTransform { get; protected set; }
    public Renderer myRenderer;

    #endregion

    #region Public Fields

    /// <summary>Material when health is taken away.</summary>
    public Material HealthHit_Mat;
    /// <summary>Seconds for the hit material.</summary>
    public float healthHitMatTime = 0.5f;
    /// <summary>Can recieve damage?</summary>
    public bool invincible;

    #endregion

    #region Protected Fields

    /// <summary>Changes material from normal to hit and back.</summary>
    protected Job changeMat;
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
        myTransform = transform;
        if (myRenderer == null) myRenderer = renderer;
        Normal_Mat = myRenderer.material;

        Initialize();
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        if (changeMat != null)
        {
            changeMat.Kill();
        }
    }

    #endregion

    #region Public Methods

    //
    public void Reset()
    {
        if (myRenderer == null) myRenderer = renderer;
        Normal_Mat = myRenderer.material;

        Initialize();
    }


    /// <summary>
    /// Set health to max health.
    /// </summary>
    public virtual void Initialize()
    {
        health = maxHealth;

        myRenderer.material = Normal_Mat;
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
    /// <param name="damage">HitInfo from weapon.</param>
    public virtual void RecieveHit(Ship sender, float damage)
    {
        if (invincible) return;

        if (ChangeHealth(-damage))
        {
            Killed(sender);
            return;
        }

        if (HealthHit_Mat != null)
        {
            HitMat(HealthHit_Mat);
        }
    }


    /// <summary>
    /// Add to health. Clamped.
    /// </summary>
    /// <param name="amount">Amount of health added.</param>
    /// <returns>True, if health is 0.</returns>
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

    /// <summary>
    /// Change the material and restart Job.
    /// </summary>
    /// <param name="mat">Material to change to.</param>
    protected void HitMat(Material mat)
    {
        if (changeMat != null)
        {
            changeMat.Kill();
        }
        changeMat = new Job(ChangeMat(mat));
    }


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


    /// <summary>
    /// Call the DieEvent.
    /// </summary>
    /// <param name="sender">Ship that killed this ship. Null if trashed.</param>
    protected virtual void Killed(Ship sender)
    {
        if (DieEvent != null)
        {
            DieEvent(this, new DieArgs(sender));
        }
        myRenderer.material = Normal_Mat;
    }

    #endregion
}