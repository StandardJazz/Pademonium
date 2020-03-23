Shader "Shader/Grayscale_PP"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_LerpVal("Color to Gray", Range(0,1)) = 0
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float _LerpVal;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);

			float lum = 0.3*color.r + 0.59*color.g + 0.11*color.b;
			float4 grayscale = float4(lum, lum, lum, color.a);

			float4 finalColor = lerp(color, grayscale, _LerpVal);

			return finalColor;
		}
		ENDCG
        }
    }
}
