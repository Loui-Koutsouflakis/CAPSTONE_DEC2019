Current Build: 94 Grass Patches

Each Grass Patch is 10x10 - DO NOT SCALE
The Grass Blades are drawn according to the Plane that's used.
If we want a larger patch we will require a larger plane with more Triangles.

To apply the shader
==

1- Go to the Mesh Renderer component in the Inspector (can be any kind of Mesh Renderer)
2- Go to the Material section and Select the Shader type -> Capstone/Grass

**These steps are the same ones used to apply the Toon Shader

Properties & Fuctions (only the non obvious ones)
==
1- Tessllation Uniform -> Controls The Grass Density (adds or reduces the amount of triangles
on the base plane)

2- Translucent Gain -> Affects the Gradient of Both Colours

3- Wind Strength -> As you guessed it, affects the wind brush applied on the grass.
It's entirely independant but can be modified via script. Maybe make it according to existing
wind component ?


**Notes

- Curently, the Grass is able to apply shadows on objects
- Objects can apply shadows onto the grass

-> It is possible to prevent the Shader to have any Shadow outputs
To do this we would have to mess round with the Directional Light (as tested earlier today)
But we will wait and see if the Shadows do affect performance.

As of yet most of the processing is being used on the shadows (60-90%)


SChiraz