using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ProjectileFollowTarget : MonoBehaviour
{
    [SerializeField]
	private float speed = 5.0f;
    [SerializeField]
    private GameObject hitEffect;
    private float distance;
    /// <summary>
    /// The target that the projectile will follow, the hit effect appears on the target as well.
    /// </summary>
    [HideInInspector]
    public Transform target;

    /// <summary>
    /// The stay effect and it's target position, the stay effect usually stays on our tagret for sometime.
    /// </summary>
    [SerializeField]
    private GameObject stayEffect;

    [HideInInspector]
    public Transform stayEffectPosition;

    [HideInInspector]
    public UnityEvent onProjectileHitTarget;

    public bool shakeCameraOnHit = false;
    public string soundEffect = "";

    private void Start()
    {
        if (soundEffect != "")
        {
            SoundEffectsController.Instance.playSoundEffectOneShot(soundEffect);
        }
    }

    // Update is called once per frame
	private void Update ()
    {
        gameObject.transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position,target.position,speed * Time.deltaTime);
			
        distance = Vector3.Distance(transform.position,target.position);
        const float distanceToHit = 0.1f;
        if(distance <= distanceToHit)
		{
            //call the callback
            if (shakeCameraOnHit)
            {
                CameraController.Instance.ShakeCamera();
            }
            onProjectileHitTarget.Invoke();
            stopAllParticles();
            ShowHitEffect();
            ShowStayEffect();
            //auto destruct self in 1.5 seconds
            Destroy(gameObject,1.5f);
			Destroy(this);
		}
	}

    private void stopAllParticles()
    {
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particleSystem in allParticles)
        {
            particleSystem.loop = false;
        }
    }

    private void ShowStayEffect()
    {
        //if stay effect exists, initiat it
        if(stayEffect != null)
        {
            //the stay effect will appear and destroy itself eventually
            GameObject stayEffectClone = GameObject.Instantiate(stayEffect,stayEffectPosition.position,stayEffect.transform.rotation) as GameObject;
            stayEffectClone.transform.parent = stayEffectPosition;
        }
    }

    private void ShowHitEffect()
    {
        //if hit effect exists, initiat it
        if(hitEffect != null)
        {
            //the hit effect will appear and destroy itself eventually
            GameObject hitEffectClone = GameObject.Instantiate(hitEffect,transform.position,hitEffect.transform.rotation) as GameObject;
        }
    }
}
