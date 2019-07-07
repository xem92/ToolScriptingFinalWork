# ToolScriptingFinalWork

## Task 1
3 new menu options added.
- Rotate selected object 45º to the right, also is available the shortcut H.
- Rotate selected object 45º to the left, also is available the shortcut G.
- Delete selected object, also is available the shortcut D.

## Task 2
It rotates, scale and adds to the position.

## Task 3
New field "objective" added inthe json.
If there is the "objective" field, it will take it as the target.
Example:
{
  "scene": "camera_scene",
  "directory": "data/assets/",
    "geometries": [
    {"name": "nanosuit", "file": "nanosuit.obj"},
    {"name": "cubemap", "file": "cubemap.obj"}
  ],
  "textures": [],
  "shaders": [],
  "materials": [],
  "lights": [],
  "entities": [],
  "cameras": [
	{
      "name": "camera_track",
      "movement": "free",
	  "position": [-10, -3, 5 ],
      "direction": [ 1, 0, 0 ],
	  "objective": "red_sphere",
	  "target": [ 0, 0, 0 ],
      "fov": 65,
	  "near": 0.03,
	  "far": 1000,
	  "track" : {
		"speed" : 0.25,
		"knots" : [
			[-18.2567,26.7245,46.6186],
			[-48.6091,26.7245,32.4076],
			[-52.4955,26.7245,-18.4011],
			[55.6804,26.7245,-27.0378],
			[16.6228,26.7245,25.6478]
		]
	  }
    }
  ]
}

## Task 4
Not Done.

## Task 5
Scene LevelWhitBox.
