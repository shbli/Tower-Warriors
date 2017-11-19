using UnityEngine;
using System.Collections;

public abstract class GridObjectBase : MonoBehaviour
{
    NavTileAgent agent;

    /// <summary>
    /// How many steps our object can attack
    /// </summary>
    public int attackSteps;

    /// <summary>
    /// The type of the attack movement (+, x or both).
    /// </summary>
    public MovementType attackMovementType;

    /// <summary>
    /// The rotateable attack parent, in characters it is the character model, this will look at the target before doing an attack, if null no rotation will be made.
    /// </summary>
    [SerializeField]
    private Transform rotateableAttackParent;

    /// <summary>
    /// The muzzle and it's position.
    /// </summary>
    [SerializeField]
    private Transform muzzlePosition;
    [SerializeField]
    private GameObject muzzle;

    /// <summary>
    /// The projectile and it's starting position.
    /// </summary>
    [SerializeField]
    private Transform projectilePosition;
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    GridObjectBase targetGridObject;

    /// <summary>
    /// The projectile hit on the current object (Where the projectile is going to hit).
    /// </summary>
    public Transform projectileHit;

    /// <summary>
    /// The stay effect hit on the current object (Where the stay effect is going to stay for few turns).
    /// </summary>
    public Transform stayEffectHit;

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int damagePoints;

    private int currentHealth;

    public bool IsFrozen
    {
        get
        {
            if (GetComponent<Freeze>() != null)
            {
                return true;
            }
            return false;
        }
    }

    public bool IsDead
    {
        get
        {
            if (GetComponent<Dead>() != null)
            {
                return true;
            }
            return false;
        }
    }

    private void Start()
    {
        agent = GetComponent<NavTileAgent>();
        currentHealth = maxHealth;
        if (agent.isLocalAgent)
        {
            GameController.Instance.AddControlableAgent(agent);
        }
    }

    private void OnDestroy()
    {
        GameController.Instance.RemoveControlableAgent(agent);
    }

    public void AttackTarget(GridObjectBase pTarget)
    {
        targetGridObject = pTarget;
        AttackTargetLocal();
    }

    [ContextMenu("AttackTargetLocal")]
    public void AttackTargetLocal()
    {
        Debug.Log("AttackTarget");
        if (rotateableAttackParent == null)
        {
            OnRotateableAttackParentRotated();
            return;
        }

        //save the original rotation
        Vector3 originalRotation = rotateableAttackParent.transform.eulerAngles;

        //calculate the look at rotation
        rotateableAttackParent.LookAt(targetGridObject.transform.position);
        Vector3 targetRotation = rotateableAttackParent.transform.eulerAngles;
        targetRotation.z = originalRotation.z;
        targetRotation.x = originalRotation.x;

        //return to original rotation
        rotateableAttackParent.transform.eulerAngles = originalRotation;

        const float ANIMATION_TIME = 0.5f;

        //animate to requuired rotation
        iTween.RotateTo(
            rotateableAttackParent.gameObject,
            iTween.Hash(
                iTween.HashKeys.rotation, targetRotation,
                iTween.HashKeys.islocal, false,
                iTween.HashKeys.time, ANIMATION_TIME,
                iTween.HashKeys.oncompletetarget, gameObject,
                iTween.HashKeys.oncomplete, "OnRotateableAttackParentRotated"
            )
        );

    }

    /// <summary>
    /// Raises the rotateable attack parent rotated event.
    /// </summary>
    private void OnRotateableAttackParentRotated()
    {
        Debug.Log("OnRotateableAttackParentRotated");
        //the muzzle effect will appear and destroy itself eventually
        if (muzzle != null)
        {
            GameObject muzzleClone = GameObject.Instantiate(muzzle,muzzlePosition.position,muzzle.transform.rotation) as GameObject;
        }
        if (projectile != null)
        {
            GameObject projectileClone = GameObject.Instantiate(projectile,projectilePosition.position,projectile.transform.rotation) as GameObject;
            projectileClone.GetComponent<ProjectileFollowTarget>().target = targetGridObject.projectileHit;
            projectileClone.GetComponent<ProjectileFollowTarget>().stayEffectPosition = targetGridObject.stayEffectHit;
            projectileClone.GetComponent<ProjectileFollowTarget>().onProjectileHitTarget.AddListener(OnProjectileHitTarget);
        }
    }

    private void OnProjectileHitTarget()
    {
        //when our projectile hit the target we decrease the target health
        targetGridObject.decreaseHealthBy(damagePoints);
    }

    public void decreaseHealthBy(int pAmount)
    {
        currentHealth -= pAmount;
        GetComponentInChildren<HealthBar>().setBarPercantage( (float)( (float)currentHealth / (float)maxHealth ) * 100f );
        if (currentHealth <= 0)
        {
            //our character is dead!
            MarkAsDead();

        }
    }

    private void MarkAsDead()
    {
        if (!IsDead)
        {
            gameObject.AddComponent<Dead>();
        }
    }
}
