Feature: GetProcess

    As a user of the ProcessGraph API
    I want to be able to retrieve a process by its GUID
    So that I can view its details
    
    @get_process
    Scenario: Retrieve a process by GUID
        Given a process exists in the system
        When I request the process with a valid GUID
        Then I should receive process details