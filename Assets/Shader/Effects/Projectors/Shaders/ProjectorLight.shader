// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Light" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ProjectionTexture_Q ("Q Spell Indicator", 2D) = "" {}
		_ProjectionTexture_W ("W Spell Indicator", 2D) = "" {}
		_ProjectionTexture_E ("E Spell Indicator", 2D) = "" {}
		_ProjectionTexture_R ("R Spell Indicator", 2D) = "" {}
		_TextureIndex("Index", Int) = 0
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _ProjectionTexture_Q;
			sampler2D _ProjectionTexture_W;
			sampler2D _ProjectionTexture_E;
			sampler2D _ProjectionTexture_R;
			int _TextureIndex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS;
				switch (_TextureIndex)
				{
				case 0:
					texS = tex2Dproj(_ProjectionTexture_Q, UNITY_PROJ_COORD(i.uvShadow));
					break;
				case 1:
					texS = tex2Dproj(_ProjectionTexture_W, UNITY_PROJ_COORD(i.uvShadow));
					break;
				case 2:
					texS = tex2Dproj(_ProjectionTexture_E, UNITY_PROJ_COORD(i.uvShadow));
					break;
				case 3:
					texS = tex2Dproj(_ProjectionTexture_R, UNITY_PROJ_COORD(i.uvShadow));
					break;
				default:
					break;
				}

				texS.rgb *= _Color.rgb *100;
				return texS;
			}
			ENDCG
		}
	}
}
