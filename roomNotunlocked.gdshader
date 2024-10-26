shader_type canvas_item;
uniform sampler2D noise_img;
uniform float noise_opacity = 0.2;

void fragment() {
    
    vec2 uv = fract(vec2(UV.x + (float(TIME) / 6.0), UV.y));
    
    
    vec4 base_color = texture(TEXTURE, UV); 
    vec4 noise_color = texture(noise_img, uv); 
    
   
    COLOR.rgb = mix(base_color.rgb, noise_color.rgb, noise_opacity);
    COLOR.a = base_color.a;
}
