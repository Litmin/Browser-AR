Shader "Unlit/WhiteAs0AlphaShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Opacity ("Opacity",Float) = 1.0
		_KeyBg ("Bg",Color) = (-1.0,-1.0,-1.0,1.0)
		_BKey ("BoolKey",Float) = -1.0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Opacity;
			float4 _KeyBg;
			float _BKey;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				float keyRange = 0.1;
				//抠白
				if (_BKey < 0)
				{
					//不抠像
				}
				else
				{
					//抠像
					//col.a = 1.0;
					if((col.r >= _KeyBg.r - keyRange && col.r <= _KeyBg.r + keyRange) && (col.g >= _KeyBg.g - keyRange && col.g <= _KeyBg.g + keyRange) && (col.b >= _KeyBg.b - keyRange && col.b <= _KeyBg.b + keyRange))
					{
						col.a = 0;
					}
				}
				col.a = col.a * _Opacity;
				return col;
			}
			ENDCG
		}
	}
}
