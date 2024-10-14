shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

void fragment() {
    vec3 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;
    float gray = dot(color, vec3(0.3, 0.59, 0.11));
    COLOR = vec4(vec3(gray), 1.0);
}
