using UnityEngine;
using System.Collections;

public class Delay : MonoBehaviour, IPooledMonoBehaviour {
	
	public float delayTime = 1.0f;
	private bool _started = false;
	//----------------------------------------------
	/// GameObject第一次被创建的时候被调用
	//----------------------------------------------
	public void OnCreate()
	{
		
	}
	
	//----------------------------------------------
	/// 每次从GameObjectPool中返回GameObject的时候被调用
	//----------------------------------------------
	public void OnGet()
	{		
		if (_started) {
			return;
		}
		DoStart ();
	}
	
	//----------------------------------------------
	/// 每次GameObject被回收的时候被调用
	//----------------------------------------------
	public void OnRecycle()
	{
		_started = false;
	}
	
	void Start () {		
		if (_started) {
			return;
		}
		DoStart ();
	}

	private void DoStart()
	{
		gameObject.SetActive(false);
		Invoke("DelayFunc", delayTime);
		_started = true;
	}
	
	void DelayFunc()
	{
		gameObject.SetActive(true);
	}
	
}
