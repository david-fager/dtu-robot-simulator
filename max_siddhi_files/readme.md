This folder contains the files you will need to run the siddhi app:
AmazonCEPEngineApp.siddhi
robot_rep_data

############## file locations ##############

Add the AmazonCEPEngineApp.siddhi file to:
\siddhi-tooling-5.1.2\wso2\tooling\deployment\workspace


Create the folder <files>:
\siddhi-tooling-5.1.2\resources\files

Add robot_rep_data to the folder.


############## deployment ##############
- change the environment variables accordingly in tooling.sh/bat

- start siddhi app

- change the endpoint port of dataMonitoringStream to 8001

- make sure that the file io source path to the robot_rep_data file is the absolute path on your machine.
Dont remove file:\. It is an URI and has to begin with the protocol for fetching the resource.

- deploy the siddhi app (it sometimes fails to do so on my vm, retry if it doesn't start)

- run Program.cs /robot-sim/robot-sim/Program.cs

- wait for events to be created by the EPA's
