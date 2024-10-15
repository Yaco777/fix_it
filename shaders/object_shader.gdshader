shader_type canvas_item;

uniform float vector_movment_x = 10;
uniform float vector_movment_y = 10;
uniform float scaling_factor = 0.001;

void vertex() {
	VERTEX += cos(TIME) * vec2(vector_movment_x,vector_movment_y) + cos(TIME) * vec2(vector_movment_x,vector_movment_y);
	float scale_factor = 1.0 + sin(TIME) * scaling_factor; 
    VERTEX *= scale_factor;
}



//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
