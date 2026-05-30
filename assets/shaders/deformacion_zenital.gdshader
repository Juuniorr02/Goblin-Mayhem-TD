shader_type canvas_item;

uniform float angle : hint_range(0.0, 6.28318) = 0.507;
uniform float lean_factor : hint_range(0.0, 2.0) = 0.262;
uniform float iso_ratio : hint_range(0.0, 1.0) = 0.62;

void vertex() {
    float height = -VERTEX.y;

    if (height > 0.0) {
        VERTEX.x += cos(angle) * height * lean_factor;
        VERTEX.y -= sin(angle) * height * lean_factor * iso_ratio;
    }
}
