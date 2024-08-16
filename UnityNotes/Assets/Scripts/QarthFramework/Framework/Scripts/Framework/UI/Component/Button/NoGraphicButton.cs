using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Qarth
{
	public class NoGraphicButton : MaskableGraphic
	{
		public override void SetMaterialDirty() { return; }
		public override void SetVerticesDirty() { return; }
		
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			toFill.Clear();
			return;
		}
	}
	
}