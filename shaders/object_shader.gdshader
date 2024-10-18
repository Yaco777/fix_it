shader_type canvas_item;


uniform float scaling_factor = 0.2; 
uniform float max_scale_factor = 2; 

void vertex() {
    
    float scale_factor = 1.0 + (sin(TIME) * scaling_factor); 

    scale_factor = clamp(scale_factor, 0.80, max_scale_factor);
    
    VERTEX *= scale_factor;
}
