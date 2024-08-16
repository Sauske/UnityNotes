//This script will only work in editor mode. You cannot adjust the scale dynamically in-game!
using UnityEngine;
using System.Collections;

[ExecuteInEditMode, AddComponentMenu("SGame/Effect/ParticleScaler")]
public class ParticleScaler : MonoBehaviour ,IPooledMonoBehaviour
{
	public float particleScale = 1.0f;
	public bool alsoScaleGameobject = true;

	float prevScale = 1.0f;

    [HideInInspector]
    public bool scriptGenerated = false;

    private bool m_gotten = false;

    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (m_gotten)
        {
            return;
        }
        m_gotten = true;
        prevScale = particleScale;

        if (scriptGenerated && particleScale != 1)
        {
            prevScale = 1;
            CheckAndApplyScale();
        }
    }

    public void OnRecycle()
    {
        m_gotten = false;
    }

    void Start()
    {
        OnGet();
    }

    public void CheckAndApplyScale()
    {
        if (prevScale != particleScale && particleScale > 0)
        {
            if (alsoScaleGameobject)
                transform.localScale = new Vector3(particleScale, particleScale, particleScale);

            float scaleFactor = particleScale / prevScale;

            //scale legacy particle systems
            ScaleLegacySystems(scaleFactor);

            //scale shuriken particle systems
            ScaleShurikenSystems(scaleFactor);

            //scale trail renders
            ScaleTrailRenderers(scaleFactor);

            prevScale = particleScale;
        }
    }
	void Update () 
	{
#if UNITY_EDITOR 
        CheckAndApplyScale();
#endif
	}

    void ScaleShurikenSystems(float scaleFactor)
    {
        //get all shuriken systems we need to do scaling on
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem system in systems)
        {
#if UNITY_4_7
            system.startSpeed *= scaleFactor;
			system.startSize *= scaleFactor;
			system.gravityModifier *= scaleFactor;
#endif
        }
    }

	void ScaleLegacySystems(float scaleFactor)
	{
#if UNITY_4_7
        //get all emitters we need to do scaling on
        ParticleEmitter[] emitters = GetComponentsInChildren<ParticleEmitter>(true);

		//get all animators we need to do scaling on
        ParticleAnimator[] animators = GetComponentsInChildren<ParticleAnimator>(true);

		//apply scaling to emitters
		foreach (ParticleEmitter emitter in emitters)
		{
			emitter.minSize *= scaleFactor;
			emitter.maxSize *= scaleFactor;
			emitter.worldVelocity *= scaleFactor;
			emitter.localVelocity *= scaleFactor;
			emitter.rndVelocity *= scaleFactor;
		}

		//apply scaling to animators
		foreach (ParticleAnimator animator in animators)
		{
			animator.force *= scaleFactor;
			animator.rndForce *= scaleFactor;
		}
#endif
	}

	void ScaleTrailRenderers(float scaleFactor)
	{
		//get all animators we need to do scaling on
        TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>(true);

		//apply scaling to animators
		foreach (TrailRenderer trail in trails)
		{
			trail.startWidth *= scaleFactor;
			trail.endWidth *= scaleFactor;
		}
	}
}
