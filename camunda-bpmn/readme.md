# Using the BPMN models within the camunda platform
1. Add the following camunda-bpmn/dependencies to camunda-bpm-run-7.16.0\configuration\userlib
    * camunda-connect-connectors-all-1.4.0
    * camunda-connect-core-1.4.0
    * camunda-engine-plugin-connect-7.13.0
    
2. Start the Camunda Application

### Deploying robotMaintenanceV2.bpmn
Deploy the BPMN robotMaintenanceV2.bpmn together with the form and DMN
1. Add as files during deployment:
    * forms/confirmMaintenanceForm.form
    * tables/decisionTableMaintenance.dmn

### Deploying the robotPosition.bpmn
Deploy the BPMN robotPosition.bpmn together with the form and DMN
1. Add as files during deployment:
    * canRobotMoveForm.form
    * decisionTablePosition.dmn

### Deploying the BPMN_pick_order_2.bpmn
Deploy the BPMN BPMN_pick_order_2.bpmn together with the forms
1. Add as files during deployment:
    * isItemAvailableForm.form
    * isToteFullForm.form
    * processToteToPackingForm.form




### Start the Siddhi Apllication
1. Run AmazonCEPEngineApp
2. Run robot-sim
3. Check that tokens appear in the Camunda Cockpit 
