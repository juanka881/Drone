x 1. using gac references, need to be able to expand gac references into full file 
references

note: #1 not needed since the way csc.exe resolves usings in the gac 

1. make relative files in config relative to the dir where the config is located at  

2. need to be able to expand {property} into values from the properties found in config
if the path actually has {} in it, then it can be escaped using \ in front of {, like so
"c:\files\{dir}\hello", this is since paths can use {} chars

3. add a "path" type to set, in which \ will not be interpresed as an escape operator.

4. add code to timestamp and keep track of files and their changes, if they change in size 
or modified timestamp it should recompile, otherwise it shouldnt. 

5. add code to init to create a basic DroneMain.cs file with all the basic code so the user 
can just start hacking away. 

6. add code to be able to generate a VS solution and project from the current drone.config file

7. add installer to drone, installer should drop files int program files, and add drone to path
so it can be called on command line

8. write docs on how drone works and help for different commands

9. make website for drone, make logo

10. make 
