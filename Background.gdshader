shader_type canvas_item;

uniform vec4 color1 : source_color;
//uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
	vec4 pixel_color = texture(TEXTURE, UV);

	vec4 new_color = vec4(0.0);

	for(int y = -1; y <= 1; y++)
	for(int x = -1; x <= 1; x++)
	{
		vec4 npixel = texture(TEXTURE, UV + vec2(float(x), float(y)) + TEXTURE_PIXEL_SIZE);
		new_color += npixel;
	}
	new_color = new_color / 9.0;
	//new_color = vec4(0.15);

	if(pixel_color.r >= 0.15 ||
		pixel_color.g >= 0.15 ||
		pixel_color.b >= 0.15)
	{
			new_color = COLOR;
	}

	COLOR = new_color;
	//COLOR = vec4(color1.r, color1.g, color1.b, mix(color1.a, pixel_color.a, distance));
}

float average(float a, float b, float c){
	return (a + b + c) / 3.0;
}