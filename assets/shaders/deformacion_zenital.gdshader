shader_type canvas_item;

// Controlado automáticamente por tu script de C#
uniform float camera_zoom = 1.0;
uniform float flip_modifier = 1.0;

// Ajustes de calibración para controlar la intensidad del efecto
uniform float base_skew : hint_range(-2.0, 2.0) = -0.55; 
uniform float base_height_scale : hint_range(0.1, 2.0) = 0.9;

void vertex() {
    // Detectamos la altura desde la base del sprite (pies del goblin)
    float local_height = max(0.0, -VERTEX.y);

    if (local_height > 0.0) {
        // Al multiplicar por el zoom (en lugar de dividir), a mayor zoom (más cerca),
        // mayor será la inclinación lateral y el estiramiento vertical (efecto "lean hacia atrás")
        float dynamic_skew = base_skew * camera_zoom * flip_modifier;
        float dynamic_height = base_height_scale * (1.0 + (camera_zoom * 0.15));
        
        VERTEX.x += local_height * dynamic_skew;
        VERTEX.y *= dynamic_height;
    }
}
