// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BlueSky/Skybox"
{
	Properties
	{
		color_map("color_map", 2D) = "white" {}
	}
		SubShader
	{
		Pass
	{
		CULL FRONT
		CGPROGRAM //--------------

#pragma vertex   vertex_shader
#pragma fragment fragment_shader

	struct a2v
	{
		float4 vertex   : POSITION;
		float4 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : POSITION;
		float4 texcoord : TEXCOORD1;
	};

	v2f vertex_shader(a2v IN)
	{
		v2f OUT;

		OUT.position = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		return OUT;
	}

	void fragment_shader(v2f IN,
		uniform sampler2D color_map,
		out float4 col:COLOR,
		out float depth : DEPTH)
	{
		col.xyz = tex2D(color_map, IN.texcoord.xyz);
		col.w = 1;
		depth = 1;
	}

	ENDCG //--------------
	}
	}
}