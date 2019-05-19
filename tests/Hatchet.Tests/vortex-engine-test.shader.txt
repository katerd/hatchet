{
    Name "Test Shader"
    Queue 1234
    Techniques
    [
        {
            QualityLevel High
            Passes [pass01 pass02 pass03]
        }
    ]

	Defines
	[
		{ Name "example" Text "example define contents" }
	] 

    Passes
    [
        {
            Name pass01
            ZWrite true

            Vertex
            ![
                #version 120

                uniform mat4 worldViewProjMat;
                uniform mat4 viewMat;

                void main()
                {
                    ##PROJECT_COLOUR_AND_TEXTURE
                }
            ]!

            Fragment
            ![
                #version 120

                uniform sampler2D tex0;

                void main()
                {
                    gl_FragColor = gl_Color * texture2D(tex0, gl_TexCoord[0].st);

                    // testing
                }
            ]!
        }

        {
            Name pass02
            ZWrite false

            Vertex
            ![
                #version 120

                uniform mat4 worldViewProjMat;
                uniform mat4 viewMat;

                void main()
                {
                    ##PROJECT_COLOUR_AND_TEXTURE
                }
            ]!

            Fragment
            ![
                #version 120

                uniform sampler2D tex0;

                void main()
                {
                    gl_FragColor = gl_Color * texture2D(tex0, gl_TexCoord[0].st);

                    // testing
                }
            ]!
        }


        {
            Name pass03
            ZWrite false

            Vertex
            ![
                #version 120

                uniform mat4 worldViewProjMat;
                uniform mat4 viewMat;

                void main()
                {
                    ##PROJECT_COLOUR_AND_TEXTURE
                }
            ]!

            Fragment
            ![
                #version 120

                uniform sampler2D tex0;

                void main()
                {
                    gl_FragColor = gl_Color * texture2D(tex0, gl_TexCoord[0].st);

                    // pass03 fragment
                }
            ]!
        }
    ]
}