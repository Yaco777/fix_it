shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform int mode = 0; // 0: Noir et blanc, 1: Sans rouge, 2: Sans vert, 3: Sans bleu

void fragment() {
    vec3 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;

    if (mode == 0) {
        // Mode Noir et blanc
        float gray = dot(color, vec3(0.3, 0.59, 0.11));
        COLOR = vec4(vec3(gray), 1.0);
    } else if (mode == 1) {
        // Mode Sans rouge
        COLOR = vec4(0.0, color.g, color.b, 1.0);
    } else if (mode == 2) {
        // Mode Sans vert
        COLOR = vec4(color.r, 0.0, color.b, 1.0);
    } else if (mode == 3) {
        // Mode Sans bleu
        COLOR = vec4(color.r, color.g, 0.0, 1.0);
    }
}
