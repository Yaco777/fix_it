shader_type canvas_item;

void fragment() {

    vec4 tex_color = texture(TEXTURE, UV);
    tex_color.a *= 0.5 * (cos(TIME * 2.0) + 1.0);


    COLOR = tex_color;
}