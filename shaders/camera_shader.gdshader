shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture; 
uniform int mode = 0; 
uniform float red_transition = 1.0; 
uniform float green_transition = 1.0; 
uniform float blue_transition = 1.0;
uniform float bw_transition = 0.0; 

void fragment() {
  
    vec3 color = texture(screen_texture, SCREEN_UV).rgb;

    if (mode == 0) {
        
        float gray = dot(color, vec3(0.3, 0.59, 0.11));
        vec3 bwColor = vec3(gray); 
        COLOR = vec4(mix(color, bwColor, bw_transition), 1.0); 
    } else {
   		
        vec3 finalColor = vec3(
            mix(color.r, 0.0, red_transition),   
            mix(color.g, 0.0, green_transition), 
            mix(color.b , 0.0, blue_transition) 
        );
        COLOR = vec4(finalColor, 1.0);
    }
}
