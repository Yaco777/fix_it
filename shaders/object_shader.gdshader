shader_type canvas_item;

uniform float vector_movment_x = 10.0;
uniform float vector_movment_y = 10.0;
uniform float scaling_factor = 0.2; // Très petit facteur de mise à l'échelle
uniform float max_scale_factor = 2; // Facteur d'échelle maximum

void vertex() {
    // Mouvement sinusoidal avec contrainte sur le facteur de mise à l'échelle
    float scale_factor = 1.0 + (sin(TIME) * scaling_factor); 

    // Limiter le facteur d'échelle à une valeur raisonnable
    scale_factor = clamp(scale_factor, 0.80, max_scale_factor);
    
    // Appliquer le facteur d'échelle à la position du sommet
    VERTEX *= scale_factor;
}
