// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Puppet2D/vertColor" { 
   Properties {
    
   }
   SubShader {
      Pass { 
      
		
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
         #include "UnityCG.cginc"
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 vertColor : COLOR;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 vertColor : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos = UnityObjectToClipPos(input.vertex);
            output.vertColor = input.vertColor;
            
            return output;
         }
 
         fixed4 frag(vertexOutput input) : COLOR
         {

            return input.vertColor; 

         }
 
         ENDCG
      }
   }

}