﻿Shader "Kaima/Shape/Color"
{
	Properties
	{
		_Border("Border", Vector) = (0.1,0.1,0.1,0.1)
		_CircleCenter("Circle Center", Vector) = (0.5, 0.5, 0, 0)
		_CircleRadius("Circle Radius", Range(0, 1)) = 0.3
		_Color("Color", Color) = (1, 0, 0, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/GitShader/_Libs/Tools.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Border;
			float4 _CircleCenter;
			float _CircleRadius;
			fixed4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float v = Rect(_Border, i.uv) - Circle(_CircleCenter.xy, _CircleRadius, i.uv);
				return v * _Color; 
			}
			ENDCG
		}
	}
}
