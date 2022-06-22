// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Grid" {

	Properties{
	  _GridThickness("Grid Thickness", Float) = 0.01
	  _GridSpacing("Grid Spacing", Float) = 10.0
	  _Color("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
	  _BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
	}

		SubShader{
		  Tags { "Queue" = "Transparent" }

		  Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM


		#include "UnityCG.cginc"
		// Define the vertex and fragment shader functions
		#pragma vertex vert
		#pragma fragment frag

		// Access Shaderlab properties
		uniform float _GridThickness;
		uniform float _GridSpacing;
		uniform float4 _Color;
		uniform float4 _BaseColour;
		

		// Input into the vertex shader
		struct vertexInput {
			float4 vertex : POSITION;

		};

		// Output from vertex shader into fragment shader
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 worldPos : TEXCOORD0;

		};

		// VERTEX SHADER
		vertexOutput vert(vertexInput input) {
		  vertexOutput output;
		  output.pos = UnityObjectToClipPos(input.vertex);
		  // Calculate the world position coordinates to pass to the fragment shader
		  output.worldPos = mul(unity_ObjectToWorld, input.vertex);

		  return output;
		}

		// FRAGMENT SHADER
		float4 frag(vertexOutput input) : COLOR {
			float4 newpos = input.worldPos + _Time*0.1f;
		  if (frac(newpos.x / _GridSpacing) < _GridThickness || frac(newpos.y / _GridSpacing) < _GridThickness|| frac(newpos.z / _GridSpacing) < _GridThickness) {
			return _Color;
		  }
		  else {
			return _BaseColour;
		  }
		}
	ENDCG
	}
	}
}
