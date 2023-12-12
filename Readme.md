# Mediasorter

Extensively rename and move your files.

## Features

## Usage

### Calling the program

Call the program by `{mediasorter} -path <FOLDER_WITH_FILES> -configfile <CCONFIGFILE>`.
- `-path`: The folder containing the files to rename
- `-configfile`: The JSON file containing the rules for the renaming process.

### Functionality basics

Mediasorter works on files in a given directory. It gets tasks from a configuration file and executes them on all files in the directory matching specific conditions. At the moment these files can be renamed, renamed using regular expressions, copied to (several) directories, and getting the exif creation date added to the filename.

Matching files are determined by two regular expressions: `Include` (mandatory) and `Exclude` (optional). There is the possibility to create presets which can be used by `IncludePreset` / `ExcludePreset` for the purpose of reusing code. (Example: `"FilterPresets": [ "IMAGES": "(jpg|JPG|bmp|BMP)$" ]`.)

Each action will be logged to a rolling logfile `.mediasorter.log` in the directory of the files.

### Configuring the program

The configuration file contains a name and a description for logging purposes, so does each action as well. Example:

```
{
	"name": "My image collection sorter",
	"description": "My description",
	"actions":
	[
		{
			"name":          "Replace -",
			"description":   "replace - to _",
			"includePreset": "SmartphonePics",
			"replace": 
			{
				"from": "-",
				"to":   "_"
			}
		},
		{
			"name":          "Rename smartphone pics",
			"description":   "rename to IMG scheme",
			"includePreset": "SmartphonePics",
			"replaceRegex": 
			{
				"from": "^DCIM_(.*)_(.*)_.*$",
				"to":   "IMG_$2_$1.jpg"
			}
		},
		{
			"name":        "Add date",
			"description": "add timestamp to photos without",
			"include":     "jpg$",
			"exclude":     "[0-9]{8}_[0-9]{6}",
			"extractDate": 
			{
				"from": "(.*)",
				"to":   "{DATE}_$1"
			}
		},
		{
			"name": "Move to folders",
			"description": "copy to subfolder and to archive",
			"includePreset": "JPG",
			"directoriesToMove": 
			[
				"myImageFolder",
				"//my/remote/backup/folder"
			]
		},
	],	
	"filterPresets":
	[
		"JPG": "jpg|JPG|jpeg|JPEG$",
		"SmartphonePics": "^DCIM.*"
	]
}
```