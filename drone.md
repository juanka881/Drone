# commands

### init - initialize drone configuration

creates a new drone configuration file for a new project

`drone init` 

### help - show help

shows help

`drone`

### add - add source or reference files

adds source files or reference files to the drone configuration

`drone add {file-name-one-or-more}`

`drone add mytasks.cs`

`drone add LogLibrary.dll`

`drone add task1.cs task2.cs lib.dll`

### rm - remove source or reference files

removes files or reference files to the drone configuration

`drone rm {file-name-one-or-more}`

`drone rm mytasks.cs`

`drone rm LogLibrary.dll`

`drone rm task1.cs task2.cs lib.dll`

### r - run tasks

runs a set of tasks. the task list is separated 
by spaces.

`drone r {task-name-one-or-more}`

`drone r build`

`drone r clean compile deploy`

`drone r db/build`

### sp - set property

sets a property in the drone configuration. 

`drone sp {property-name} {property-value}`

`drone sp myNumberProp 25`

`drone sp myStringProp "Hello!"`

`drone sp myListProp [200, 100, 0]`

### rp - remove property

removes a property in the drone configuration.

`drone rp {property-name-one-or-more}`

`drone rp myNumberProp`

`drone rp myNumberProp myListProp`

### ls - list tasks

list all tasks registered in the drone module.

`drone ls`

### c - compiles the drone tasks

compiles the configuration.  

`drone c`

### sync - syncs drone config with vs project

#****NOT IMPLEMENTED****

sync the source files and resource files from a vs project so changes made to a vs project are reflected back to the drone configuration.

`drone sync` 

# flags 

`-f` sets the drone.json file

`-d` waits for a debugger to attach before executing

`-l` sets the log level any of the ones below

	off 	
	fatal	
	error
	err
	warn
	info
	debug
	trace

`-t` used by the `sp` command to set the value type for the property being set

`-no-colors` disables color output logging

 
