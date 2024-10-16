shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture; // Utilisation de hint_screen_texture
uniform int mode = 0; // 0 pour noir et blanc, 1 pour couleur normale
uniform float red_transition = 1.0; // Transition pour le rouge
uniform float green_transition = 1.0; // Transition pour le vert
uniform float blue_transition = 1.0; // Transition pour le bleu
uniform float bw_transition = 0.0; // Transition vers le noir et blanc

void fragment() {
    // Récupérer la couleur d'origine à partir de l'écran
    vec3 color = texture(screen_texture, SCREEN_UV).rgb;

    if (mode == 0) {
        // Mode noir et blanc
        float gray = dot(color, vec3(0.3, 0.59, 0.11)); // Calculer la valeur en niveaux de gris
        vec3 bwColor = vec3(gray); // Couleur grise

        // Interpoler entre la couleur d'origine et la couleur grise
        COLOR = vec4(mix(color, bwColor, bw_transition), 1.0); // Interpoler en fonction de bw_transition
    } else {
        // Mode couleur normale avec transitions
        vec3 finalColor = vec3(
            mix(color.r, 0.0, red_transition),   // Appliquer la transition pour le rouge
            mix(color.g, 0.0, green_transition), // Appliquer la transition pour le vert
            mix(color.b, 0.0, blue_transition)   // Appliquer la transition pour le bleu
        );

        // Appliquer la couleur finale
        COLOR = vec4(finalColor, 1.0);
    }
}
