//========================================================================
/// ��GameObject Pool�����GameObject�ϱ�����ϱ��ű�
/// @���ڼ�¼����GameObject����Դprefab�Լ��������ϵ�IPooledMonoBehaviour�ű�
/// @neoyang
/// @2015.05.26
//========================================================================

//#define USE_OPTIMIZE_POOL

using UnityEngine;
using System.Collections;

//--------------------------------------------------------------
/// ��Ҫ����GameObjetPool�е�GameObject�ϵ������Ҫʵ�ֱ��ӿ�
/// ----------------------------
/// * ��Ҫ�ӿں���
/// * OnCreate      : GameObject��һ�α�������ʱ�򱻵���
/// * OnGet         : ÿ�δ�GameObjectPool�з���GameObject��ʱ�򱻵���
/// * OnRecycle     : ÿ��GameObject�����յ�ʱ�򱻵���
/// ----------------------------
//--------------------------------------------------------------
public interface IPooledMonoBehaviour
{
    //----------------------------------------------
    /// GameObject��һ�α�������ʱ�򱻵���
    //----------------------------------------------
    void OnCreate();

    //----------------------------------------------
    /// ÿ�δ�GameObjectPool�з���GameObject��ʱ�򱻵���
    //----------------------------------------------
    void OnGet();

    //----------------------------------------------
    /// ÿ��GameObject�����յ�ʱ�򱻵���
    //----------------------------------------------
    void OnRecycle();
};

public class CPooledGameObjectScript : MonoBehaviour
{
    public string m_prefabKey;
    public bool m_isInit;

    //��¼Ĭ��ֵ
    public Vector3 m_defaultScale;

#if USE_OPTIMIZE_POOL
    //mono����
    private MonoBehaviour[] m_cachedMonos;
    private bool[] m_cachedMonosDefaultEnabled;

    //ParticleSystem����
    private ParticleSystem[] m_cachedParticleSystems;

    //Layer
    private const int c_layerDefault = 0;
    private const int c_layerHide = 31;
#else
    //IPooledMono����
    private IPooledMonoBehaviour[] m_cachedIPooledMonos;
#endif

    //�Ƿ����ڱ�ʹ��
    private bool m_inUse;

    //----------------------------------------------
    /// ��ʼ��
    /// @prefabKey
    //----------------------------------------------
    public void Initialize(string prefabKey)
    {
#if USE_OPTIMIZE_POOL
        Component[] components = gameObject.GetComponentsInChildren<Component>(true);

        if (components != null && components.Length > 0)
        {
            int monoCount = 0;
            int particleSystemCount = 0;

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    monoCount++;
                }
                else if (components[i] is ParticleSystem)
                {
                    particleSystemCount++;
                }
            }

            m_cachedMonos = new MonoBehaviour[monoCount];
            m_cachedMonosDefaultEnabled = new bool[monoCount];
            m_cachedParticleSystems = new ParticleSystem[particleSystemCount];

            int cachedMonoCountIndex = 0;
            int cachedParticleSystemCountIndex = 0;

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    m_cachedMonos[cachedMonoCountIndex] = components[i] as MonoBehaviour;
                    m_cachedMonosDefaultEnabled[cachedMonoCountIndex] = m_cachedMonos[cachedMonoCountIndex].enabled;

                    cachedMonoCountIndex++;
                }
                else if (components[i] is ParticleSystem)
                {
                    m_cachedParticleSystems[cachedParticleSystemCountIndex] = components[i] as ParticleSystem;

                    cachedParticleSystemCountIndex++;
                }
            }
        }
        else
        {
            m_cachedMonos = new MonoBehaviour[0];
            m_cachedMonosDefaultEnabled = new bool[0];
            m_cachedParticleSystems = new ParticleSystem[0];
        }
#else
        MonoBehaviour[] monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);

        if (monoBehaviours != null && monoBehaviours.Length > 0)
        {
            int pooledMonoBehavioursCount = 0;

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                if (monoBehaviours[i] is IPooledMonoBehaviour)
                {
                    pooledMonoBehavioursCount++;
                }
            }

            m_cachedIPooledMonos = new IPooledMonoBehaviour[pooledMonoBehavioursCount];

            int cachedIPooledMonoIndex = 0;

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                if (monoBehaviours[i] is IPooledMonoBehaviour)
                {
                    m_cachedIPooledMonos[cachedIPooledMonoIndex] = monoBehaviours[i] as IPooledMonoBehaviour;
                    cachedIPooledMonoIndex++;
                }
            }
        }
        else
        {
            m_cachedIPooledMonos = new IPooledMonoBehaviour[0];
        }
#endif

        m_prefabKey = prefabKey;
        m_defaultScale = this.gameObject.transform.localScale;
        m_isInit = true;
        m_inUse = false;
    }

    //----------------------------------------------
    /// ���mono����
    /// @mono
    /// @defaultEnabled
    //----------------------------------------------
    public void AddCachedMono(MonoBehaviour mono, bool defaultEnabled)
    {
        if (mono == null)
        {
            return;
        }

#if USE_OPTIMIZE_POOL
        MonoBehaviour[] cachedMonos = new MonoBehaviour[m_cachedMonos.Length + 1];
        bool[] cachedMonosDefaultEnabled = new bool[m_cachedMonos.Length + 1];

        for (int i = 0; i < m_cachedMonos.Length; i++)
        {
            cachedMonos[i] = m_cachedMonos[i];
            cachedMonosDefaultEnabled[i] = m_cachedMonosDefaultEnabled[i];
        }

        cachedMonos[m_cachedMonos.Length] = mono;
        cachedMonosDefaultEnabled[m_cachedMonos.Length] = defaultEnabled;

        m_cachedMonos = cachedMonos;
        m_cachedMonosDefaultEnabled = cachedMonosDefaultEnabled;

        if (!m_inUse)
        {
            mono.enabled = false;
        }
#else
        if (mono is IPooledMonoBehaviour)
        {
            IPooledMonoBehaviour[] cachedIPooledMonos = new IPooledMonoBehaviour[m_cachedIPooledMonos.Length + 1];

            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                cachedIPooledMonos[i] = m_cachedIPooledMonos[i];
            }

            cachedIPooledMonos[m_cachedIPooledMonos.Length] = mono as IPooledMonoBehaviour;

            m_cachedIPooledMonos = cachedIPooledMonos;
        }
        if (!m_inUse)
        {
            mono.enabled = false;
        }
#endif
    }

    //----------------------------------------------
    /// GameObject��һ�α�������ʱ�򱻵���
    //----------------------------------------------
    public void OnCreate()
    {
#if USE_OPTIMIZE_POOL
        //Handle Mono
        if (m_cachedMonos != null && m_cachedMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedMonos.Length; i++)
            {
                if (m_cachedMonos[i] != null && m_cachedMonos[i] is IPooledMonoBehaviour)
                {
                    (m_cachedMonos[i] as IPooledMonoBehaviour).OnCreate();
                }
            }
        }
#else
        //Handle IPooledMono
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnCreate();
                }
            }
        }
#endif
    }

    //----------------------------------------------
    /// ÿ�δ�GameObjectPool�з���GameObject��ʱ�򱻵���
    //----------------------------------------------
    public void OnGet()
    {
        //Handle GameObject
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }

#if USE_OPTIMIZE_POOL
        //Handle Mono
        if (m_cachedMonos != null && m_cachedMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedMonos.Length; i++)
            {
                if (m_cachedMonos[i] != null)
                {
                    if (m_cachedMonos[i].enabled != m_cachedMonosDefaultEnabled[i])
                    {
                        m_cachedMonos[i].enabled = m_cachedMonosDefaultEnabled[i];
                    }

                    if (m_cachedMonos[i] is IPooledMonoBehaviour)
                    {
                        (m_cachedMonos[i] as IPooledMonoBehaviour).OnGet();
                    }
                }
            }
        }

        //Handle ParticleSystem
        if (m_cachedParticleSystems != null && m_cachedParticleSystems.Length > 0)
        {
            for (int i = 0; i < m_cachedParticleSystems.Length; i++)
            {
                if (m_cachedParticleSystems[i] != null)
                {
                    m_cachedParticleSystems[i].time = 0;
                    m_cachedParticleSystems[i].Play(false);                    
                    m_cachedParticleSystems[i].enableEmission = true;
                }
            }
        }

        this.gameObject.SetLayer(c_layerDefault);
#else
        //Handle IPooledMono
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnGet();
                }
            }
        }
#endif

        m_inUse = true;
    }

    //----------------------------------------------
    /// ÿ��GameObject�����յ�ʱ�򱻵���
    //----------------------------------------------
    public void OnRecycle()
    {
#if USE_OPTIMIZE_POOL
        //Handle Mono
        if (m_cachedMonos != null && m_cachedMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedMonos.Length; i++)
            {
                if (m_cachedMonos[i] != null)
                {
                    if (m_cachedMonos[i] is IPooledMonoBehaviour)
                    {
                        (m_cachedMonos[i] as IPooledMonoBehaviour).OnRecycle();
                    }

                    if (m_cachedMonos[i].enabled)
                    {
                        m_cachedMonos[i].enabled = false;
                    }
                }
            }
        }

        //Handle ParticleSystem
        if (m_cachedParticleSystems != null && m_cachedParticleSystems.Length > 0)
        {
            for (int i = 0; i < m_cachedParticleSystems.Length; i++)
            {
                if (m_cachedParticleSystems[i] != null)
                {
                    m_cachedParticleSystems[i].Clear();
                    m_cachedParticleSystems[i].Stop(false);
                    m_cachedParticleSystems[i].enableEmission = false;
                }
            }
        }

        //Handle GameObject
        this.gameObject.SetLayer(c_layerHide);
#else
        //Handle IPooledMono
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnRecycle();
                }
            }
        }

        //Handle GameObject
        this.gameObject.SetActive(false);
#endif

        m_inUse = false;
    }

    //----------------------------------------------
    /// GameObjectԤ���ص�ʱ�򱻵���
    //----------------------------------------------
    public void OnPrepare()
    {
#if USE_OPTIMIZE_POOL
        //Handle ParticleSystem
        if (m_cachedParticleSystems != null && m_cachedParticleSystems.Length > 0)
        {
            for (int i = 0; i < m_cachedParticleSystems.Length; i++)
            {
                if (m_cachedParticleSystems[i] != null)
                {
                    m_cachedParticleSystems[i].Clear();
                    m_cachedParticleSystems[i].Stop(false);
                    m_cachedParticleSystems[i].enableEmission = false;
                }
            }
        }

        //Handle GameObject
        this.gameObject.SetLayer(c_layerHide);
#else
        //Handle GameObject
        this.gameObject.SetActive(false);
#endif
    }
};