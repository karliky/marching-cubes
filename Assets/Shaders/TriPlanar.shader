Shader "Custom/TriPlanar" {
	Properties {
		_XTexture ("X", 2D) = "white" {}
		_XBumpMap ("X Normals", 2D) = "bump" {}
		_YTexture ("Y", 2D) = "white" {}
		_YBumpMap ("Y Normals", 2D) = "bump" {}
		_ZTexture ("Z", 2D) = "white" {}
		_ZBumpMap ("Z Normals", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert
		#pragma exclude_renderers flash
		
		half4 _XTexture_ST;
		half4 _YTexture_ST;
		half4 _ZTexture_ST;
		half4 _XBumpMap_ST;
		half4 _YBumpMap_ST;
		half4 _ZBumpMap_ST;
		sampler2D _XTexture, _YTexture, _ZTexture;
		sampler2D _XBumpMap, _YBumpMap, _ZBumpMap;

		struct Input {
			half4 xyv;
			half4 zvb;
			half4 xyb;
			half3 norm;
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.xyv.xy = TRANSFORM_TEX(v.vertex.zy, _XTexture);
			o.xyv.zw = TRANSFORM_TEX(v.vertex.zx, _YTexture);
			o.zvb.xy = TRANSFORM_TEX(v.vertex.xy, _ZTexture);
			o.xyb.xy = TRANSFORM_TEX(v.vertex.zy, _XBumpMap);
			o.xyb.zw = TRANSFORM_TEX(v.vertex.zx, _YBumpMap);
			o.zvb.zw = TRANSFORM_TEX(v.vertex.xy, _ZBumpMap);
			
			o.norm = (abs(v.normal));
			o.norm = (o.norm - 0.2) * 7;  
			o.norm = max(o.norm, 0);
			o.norm /= (o.norm.x + o.norm.y + o.norm.z ).xxx;   
		}
		
		void surf (Input IN, inout SurfaceOutput o) {

			half3 norm = IN.norm;
			 
			half4 output = 
			tex2D(_XTexture, IN.xyv.xy)*norm.xxxx+
			tex2D(_YTexture, IN.xyv.zw)*norm.yyyy+
			tex2D(_ZTexture, IN.zvb.xy)*norm.zzzz;
			
			o.Albedo = output.rgb;
			o.Alpha = output.a;
			o.Normal = 
					UnpackNormal(tex2D(_XBumpMap, IN.xyb.xy))*norm.xxx+
					UnpackNormal(tex2D(_YBumpMap, IN.xyb.zw))*norm.yyy+
					UnpackNormal(tex2D(_ZBumpMap, IN.zvb.zw))*norm.zzz;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}