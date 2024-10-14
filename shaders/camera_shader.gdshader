shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform int mode_current = 0;  // Current mode
uniform int mode_target = 0;   // Target mode
uniform float transition = 0.0; // Transition value (0.0 to 1.0)

vec3 apply_mode(int mode, vec3 color) {
    if (mode == 0) {
        // Black and white mode
        float gray = dot(color, vec3(0.3, 0.59, 0.11));
        return vec3(gray);
    } else if (mode == 1) {
        // No red mode
        return vec3(color.r, 0, 0);
    } else if (mode == 2) {
        // No green mode
        return vec3(0, color.g, 0);
    } else if (mode == 3) {
        // No blue mode
        return vec3(0, 0, color.b);
    }
    return color; // Default return (no modification)
}

void fragment() {
    vec3 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;

    // Apply the current mode and the target mode
    vec3 color_current = apply_mode(mode_current, color);
    vec3 color_target = apply_mode(mode_target, color);

    // Interpolate between the current mode and target mode
    vec3 final_color = mix(color_current, color_target, transition);

    // Apply the final color
    COLOR = vec4(final_color, 1.0);
}
