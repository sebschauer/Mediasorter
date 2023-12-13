# Mediasorter

Extensively rename and move your files.

## Usage

### Calling the program

Call the program by `{mediasorter} -path <FOLDER_WITH_FILES> -configfile <CONFIGFILE>`.
- `-path`: The folder containing the files to rename
- `-configfile`: The JSON file containing the rules for the renaming process.

### Functionality basics

Mediasorter works on files in a given directory. It gets tasks from a configuration file and executes them on all files in the directory matching specific conditions. At the moment these files can be renamed, renamed using regular expressions, copied to (several) directories, and getting the exif creation date added to the filename.

Matching files are determined by two regular expressions: `Include` (mandatory) and `Exclude` (optional). There is the possibility to create presets which can be used by `IncludePreset` / `ExcludePreset` for the purpose of reusing code. (Example: `"FilterPresets": [ "IMAGES": "(jpg|JPG|bmp|BMP)$" ]`.)

Each action will be logged to a rolling logfile `.mediasorter.log` in the directory of the files.

### Configuring the program

The configuration file contains a name and a description for logging purposes, so does each action as well. A full example is to be found at the end of this document.

It could make sense to create a small shellscript to start the program, and to leave the configuration file in the directory of the files to be processed, e.g. `.mediasorter.config.json`. This could be the bash script:

```
#!/bin/sh
/path/to/mediasorter -path "$PWD" -configfile "${PWD}.mediasorter.config.json"
```
Don't forget to make it executable. Now you can drop all files into the folder and doubleclick the starting script, and all your files will be processed.

### Options

- Command line arguments (both of them mandatory):
  - `-path <PATH>`: Path to the folder to be processed.
  - `-configfile <FILE>`: Path and filename of the JSON configfile describing the actions for the given path.

- Config file parameters:
  - `name`: The name of the task configuration. Optional, for logging purposes only.
  - `description`: The description of the task configuration. Optional, for logging purposes only.
  - `filterPresets`: A dictionary of predefined regular expressions by name. Useful for reusing include patterns. Example: `"JPG": "(jpg|JPG|jpeg|JPEG)$` for all files ending with JPG extensions. Can be used in action descriptions as `"includePreset": "JPG"`.
  - `actions`: A list ob action objects. These are the options for all actions:
    - `name`: The name of the task configuration. Optional, for logging purposes only.
	- `description`: The description of the task configuration. Optional, for logging purposes only.
	- `index`: A integer number the actions are sorted by.
	- `include`: A regular expression the files are filtered by: Matching files will pass. It's neccessary to deliver this xor `includePreset`.
	- `includePreset`: The key of an entry from the `filterPresets` list. The files will be whitelisted by the value of this entry. It's neccessary to deliver this xor `include`.
	- `exclude`: A regular expression the files are filtered by: Nonmatching files will pass. It's allowed to deliver this or `excludePreset` or non of them, but not both.
	- `excludePreset`: The key of an entry from the `filterPresets` list. The files will be blacklisted by the value of this entry. It's allowed to deliver this or `exclude` or non of them, but not both.
	- `replace`: The object describing a simple replacing operation:
	  - `from`: The string to search in the filename
	  - `to`: The string to replace
	- `replaceRegex`: The object describing an extended replacing operation using regular expressions:
	  - `from`: The regular expression used to match the filename, e.g. `([a-zA-Z]*)([0-9]*)(.*)`
	  - `to`: The regular expression to replace, e.g. `$2$1$3` to swap text and digits in the example.
	- `extractDate`: The object describing a date extracting operation from image EXIF data. By now, it only extracts the creation date in the format `20231231_235959`.
	  - `from`: The regular expression describing the search operation, e.g. the simple `(.*)`.
	  - `to`: The regular expression replacing. The placeholder `{DATE}` will be replaced by the creation date of the image, if there is some EXIF data. E.g. `{DATE}_$1` would insert the date at the beginning of the filename in the example.
	- `move`: The object describing a move/copy operation:
	  - `directoriesToMove`: An array of paths of directories the files shall be copied to. If they don't exist, they will be created. It is an array, so the files can be copied to more than one directory.
	  - `deleteAfterMove`: Whether the files shall be removed after copying. Optional, default: false.

    Per action it is neccessary to deliver *exactly one* of the objects `replace`, `replaceRegex`, `extractDate` or `move`.
	
	All JSON parameters are mandatory, exceptions are marked in the list above.


### Example configuration


```
{
	"name": "My image collection sorter",
	"description": "My description",
	"actions":
	[
		{
			"name":            "Replace -",
			"description":     "replace - to _",
			"index":           10,
			"includePreset":   "SmartphonePics",
			"replace": 
			{
				"from": "-",
				"to":   "_"
			}
		},
		{
			"name":            "Rename smartphone pics",
			"description":     "rename to IMG scheme",
			"index":           20,
			"includePreset":   "SmartphonePics",
			"replaceRegex": 
			{
				"from": "^DCIM_(.*)_(.*)_.*$",
				"to":   "IMG_$2_$1.jpg"
			}
		},
		{
			"name":          "Add date",
			"description":   "add timestamp to photos without",
			"index":         30,
			"include":       "jpg$",
			"exclude":       "[0-9]{8}_[0-9]{6}",
			"extractDate": 
			{
				"from": "(.*)",
				"to":   "{DATE}_$1"
			}
		},
		{
			"name":          "Move to folders",
			"description":   "copy to subfolder and to archive",
			"index":         40,
			"includePreset": "JPG",
			"move": 
			{
				"directoriesToMove": 
				[
					"myImageFolder",
					"//my/remote/backup/folder"
				],
				"deleteAfterMove": true
			}			
			
		},
	],	
	"filterPresets":
	[
		"JPG": "jpg|JPG|jpeg|JPEG$",
		"SmartphonePics": "^DCIM.*"
	]
}
```