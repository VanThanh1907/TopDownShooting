GrayPaintURP2DLit was made by Wolfeye3275.

Instructions for converting Hippo's Character Creator to lit shaders for Universal2D lightmode.

	(Can be done even if not using Universal Render Pipeline altho if you arn't using URP the why bother.)
Setp 1: Apply GrayPaintURP2DLit.shader to the EquipmentPaint.mat and EyesPaint.mat
	NOTE: If you are going for a more cartoony type game, change line 33 in GrayPaint.shader from,
		"LightMode" = "ForwardBase" to "LightMode" = "Universal2D", and use it for EyesPaint.mat.
	EXP Cartoony: Daffie Walks in to a dark room and all you see is his eyes untill he lights a match,
		only to realize that he is in a room full of black powder. **BOOM**


	(Requires Universal Render Pipeline to be installed)
Step 2: Click on Edit > Render Pipeline > Universal Render Pipeline > 2D Renderer > Upgrade Project to 2D Renderer

Step 3: In any instance of CharacterEditor under Materials, set Default Material to Sprite-Lit-Default.
	This can be found by searching your projet in packages for the material.
