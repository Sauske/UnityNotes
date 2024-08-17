using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BDCon : Conditional
{
	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}