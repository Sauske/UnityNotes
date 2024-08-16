using UnityEngine;
using System.Collections;

public class TrailPoolHelper : MonoBehaviour, IPooledMonoBehaviour
{
    private bool _awaked = false;
    private bool _started = false;
    private TrailRenderer _trail = null;
    //----------------------------------------------
    /// GameObject第一次被创建的时候被调用
    //----------------------------------------------
    public void OnCreate()
    {
        if (_awaked)
        {
            return;
        }
        _awaked = true;
        _trail = GetComponent<TrailRenderer>();
#if UNITY_EDITOR
        if(null == _trail)
        {
            Debug.LogError(gameObject.name + "下的TrailRender已经删除了，就请把TrailPoolHelper也删除掉吧，不然会报错的");
        }
#endif
    }

    //----------------------------------------------
    /// 每次从GameObjectPool中返回GameObject的时候被调用
    //----------------------------------------------
    public void OnGet()
    {
        if (_started)
        {
            return;
        }
        _started = true;
        DoStart();
    }

    //----------------------------------------------
    /// 每次GameObject被回收的时候被调用
    //----------------------------------------------
    public void OnRecycle()
    {
        _started = false;
    }

    void Start()
    {
        OnGet();
    }

    void Awake()
    {
        OnCreate();
    }

    private void DoStart()
    {
        StartCoroutine(ResetTrail(_trail));
    }

    /// <summary>
    /// Coroutine to reset a trail renderer trail
    /// </summary>
    /// <param name="trail"></param>
    /// <returns></returns>
    static IEnumerator ResetTrail(TrailRenderer trail)
    {
        if(null != trail)
        {
            var trailTime = trail.time;
            trail.time = 0;
            yield return 0;
            trail.time = trailTime;
        }
    }       

}
