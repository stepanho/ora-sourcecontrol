# ora-sourcecontrol
Source control utility for upload source codes from Oracle DB to dump files within user-friendly structure and format.


Main features:

	* Managing different databases
	
	* Uploading multiple schemas
	
	* Supported loading sources of packages, stored procedures and functions, triggers and types.
	
	* No additional creating any objects in DB
	
	* Two types of upload: full and fast
	
	* Fast scan of sources every specified period
	
	* Supports auto git pull with stashing untracked changes
	
	* Possible to use remote folders (Windows-based)


Requirements:

To use "git pull" feature, your destination folder should be inside of git repository (now only SSH authority).

Make sure you have permissions to create directories/files in destination folder.

Make sure your DB user have access to dictionaries.


Additional:

You can modify configuration files manually, but be sure you know what you are do.

Also you can watch log files.


HELP ME TO IMPROVE MY APP IF YOU LIKE IT! I will get all of your suggestions/ideas.
