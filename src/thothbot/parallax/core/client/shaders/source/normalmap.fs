uniform vec3 uAmbientColor;
uniform vec3 uDiffuseColor;
uniform vec3 uSpecularColor;
uniform float uShininess;
uniform float uOpacity;

uniform bool enableDiffuse;
uniform bool enableSpecular;
uniform bool enableAO;
uniform bool enableReflection;

uniform sampler2D tDiffuse;
uniform sampler2D tNormal;
uniform sampler2D tSpecular;
uniform sampler2D tAO;

uniform samplerCube tCube;

uniform float uNormalScale;
uniform float uReflectivity;

varying vec3 vTangent;
varying vec3 vBinormal;
varying vec3 vNormal;
varying vec2 vUv;

uniform vec3 ambientLightColor;

#if MAX_DIR_LIGHTS > 0
	uniform vec3 directionalLightColor[ MAX_DIR_LIGHTS ];
	uniform vec3 directionalLightDirection[ MAX_DIR_LIGHTS ];
#endif

#if MAX_POINT_LIGHTS > 0
	uniform vec3 pointLightColor[ MAX_POINT_LIGHTS ];
	varying vec4 vPointLight[ MAX_POINT_LIGHTS ];
#endif

#ifdef WRAP_AROUND
	uniform vec3 wrapRGB;
#endif

varying vec3 vViewPosition;

[*]

void main() {

	gl_FragColor = vec4( vec3( 1.0 ), uOpacity );

	vec3 specularTex = vec3( 1.0 );

	vec3 normalTex = texture2D( tNormal, vUv ).xyz * 2.0 - 1.0;
	normalTex.xy *= uNormalScale;
	normalTex = normalize( normalTex );

	if( enableDiffuse ) {

		#ifdef GAMMA_INPUT

			vec4 texelColor = texture2D( tDiffuse, vUv );
			texelColor.xyz *= texelColor.xyz;

			gl_FragColor = gl_FragColor * texelColor;

		#else

			gl_FragColor = gl_FragColor * texture2D( tDiffuse, vUv );

		#endif

	}

	if( enableAO ) {

		#ifdef GAMMA_INPUT

			vec4 aoColor = texture2D( tAO, vUv );
			aoColor.xyz *= aoColor.xyz;

			gl_FragColor.xyz = gl_FragColor.xyz * aoColor.xyz;

		#else

			gl_FragColor.xyz = gl_FragColor.xyz * texture2D( tAO, vUv ).xyz;

		#endif

	}

	if( enableSpecular )
		specularTex = texture2D( tSpecular, vUv ).xyz;

	mat3 tsb = mat3( normalize( vTangent ), normalize( vBinormal ), normalize( vNormal ) );
	vec3 finalNormal = tsb * normalTex;

	vec3 normal = normalize( finalNormal );
	vec3 viewPosition = normalize( vViewPosition );

	// point lights

	#if MAX_POINT_LIGHTS > 0

		vec3 pointDiffuse = vec3( 0.0 );
		vec3 pointSpecular = vec3( 0.0 );

		for ( int i = 0; i < MAX_POINT_LIGHTS; i ++ ) {

			vec3 pointVector = normalize( vPointLight[ i ].xyz );
			float pointDistance = vPointLight[ i ].w;

			// diffuse

			#ifdef WRAP_AROUND

				float pointDiffuseWeightFull = max( dot( normal, pointVector ), 0.0 );
				float pointDiffuseWeightHalf = max( 0.5 * dot( normal, pointVector ) + 0.5, 0.0 );

				vec3 pointDiffuseWeight = mix( vec3 ( pointDiffuseWeightFull ), vec3( pointDiffuseWeightHalf ), wrapRGB );

			#else

				float pointDiffuseWeight = max( dot( normal, pointVector ), 0.0 );

			#endif

			pointDiffuse += pointDistance * pointLightColor[ i ] * uDiffuseColor * pointDiffuseWeight;

			// specular

			vec3 pointHalfVector = normalize( pointVector + viewPosition );
			float pointDotNormalHalf = max( dot( normal, pointHalfVector ), 0.0 );
			float pointSpecularWeight = specularTex.r * max( pow( pointDotNormalHalf, uShininess ), 0.0 );

			#ifdef PHYSICALLY_BASED_SHADING

				// 2.0 => 2.0001 is hack to work around ANGLE bug

				float specularNormalization = ( uShininess + 2.0001 ) / 8.0;

				vec3 schlick = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( pointVector, pointHalfVector ), 5.0 );
				pointSpecular += schlick * pointLightColor[ i ] * pointSpecularWeight * pointDiffuseWeight * pointDistance * specularNormalization;

			#else

				pointSpecular += pointDistance * pointLightColor[ i ] * uSpecularColor * pointSpecularWeight * pointDiffuseWeight;

			#endif

		}

	#endif

	// directional lights

	#if MAX_DIR_LIGHTS > 0

		vec3 dirDiffuse = vec3( 0.0 );
		vec3 dirSpecular = vec3( 0.0 );

		for( int i = 0; i < MAX_DIR_LIGHTS; i++ ) {

			vec4 lDirection = viewMatrix * vec4( directionalLightDirection[ i ], 0.0 );
			vec3 dirVector = normalize( lDirection.xyz );

			// diffuse

			#ifdef WRAP_AROUND

				float directionalLightWeightingFull = max( dot( normal, dirVector ), 0.0 );
				float directionalLightWeightingHalf = max( 0.5 * dot( normal, dirVector ) + 0.5, 0.0 );

				vec3 dirDiffuseWeight = mix( vec3( directionalLightWeightingFull ), vec3( directionalLightWeightingHalf ), wrapRGB );

			#else

				float dirDiffuseWeight = max( dot( normal, dirVector ), 0.0 );

			#endif

			dirDiffuse += directionalLightColor[ i ] * uDiffuseColor * dirDiffuseWeight;

			// specular

			vec3 dirHalfVector = normalize( dirVector + viewPosition );
			float dirDotNormalHalf = max( dot( normal, dirHalfVector ), 0.0 );
			float dirSpecularWeight = specularTex.r * max( pow( dirDotNormalHalf, uShininess ), 0.0 );

			#ifdef PHYSICALLY_BASED_SHADING

				// 2.0 => 2.0001 is hack to work around ANGLE bug

				float specularNormalization = ( uShininess + 2.0001 ) / 8.0;

				vec3 schlick = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( dirVector, dirHalfVector ), 5.0 );
				dirSpecular += schlick * directionalLightColor[ i ] * dirSpecularWeight * dirDiffuseWeight * specularNormalization;

			#else

				dirSpecular += directionalLightColor[ i ] * uSpecularColor * dirSpecularWeight * dirDiffuseWeight;

			#endif

		}

	#endif

	// all lights contribution summation

	vec3 totalDiffuse = vec3( 0.0 );
	vec3 totalSpecular = vec3( 0.0 );

	#if MAX_DIR_LIGHTS > 0

		totalDiffuse += dirDiffuse;
		totalSpecular += dirSpecular;

	#endif

	#if MAX_POINT_LIGHTS > 0

		totalDiffuse += pointDiffuse;
		totalSpecular += pointSpecular;

	#endif

	gl_FragColor.xyz = gl_FragColor.xyz * ( totalDiffuse + ambientLightColor * uAmbientColor) + totalSpecular;

	if ( enableReflection ) {

		vec3 wPos = cameraPosition - vViewPosition;
		vec3 vReflect = reflect( normalize( wPos ), normal );

		vec4 cubeColor = textureCube( tCube, vec3( -vReflect.x, vReflect.yz ) );

		#ifdef GAMMA_INPUT

			cubeColor.xyz *= cubeColor.xyz;

		#endif

		gl_FragColor.xyz = mix( gl_FragColor.xyz, cubeColor.xyz, specularTex.r * uReflectivity );

	}

[*]

}