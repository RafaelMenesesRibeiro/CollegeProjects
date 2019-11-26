# DIDA Tuple Spaces
[University Project]

## dida-tuple-smr
The replication of operations should be based on the state machine replication model where all replicas start in an empty space, receive all commands in the same order and react deterministically to them. Students must implement the message delivery mechanism that ensures messages are received in the same order by all server processes.

## dida-tuple-xl
The implementation proposed by Xu and Liskov, is the one that must be followed.

### Goals
* Implement two, three or ideally, four different tuple-space projects.
* Regardless of the implementation, workload and respective performance in different scenarios must be analysed for comparison.

### Tuple Space properties
* The tuple space should provide strong guarantees on the reliability of the distributed computation and on bounded performance in the presence of failures.

### Assumptions
* The dida-tuple-space servers process can run on the same physical machine.
* Clients are programs that recieve input text file scripts with the set of commands they should run. These scripts run on the same directory as the executing program. These commands contain operations over the tuple-space (add, read, take) aswell as wait, begin-repeat, end-repeat with no meaning in the tuple-space itself. (see project syllabus for more details).
* In the basic version of the project, it can be assumed that it is possible to implement some perfect Failure Detector regarding Fault Tolerance.
* It is assumed that an arbitrary number of faults may occur but that only one fault may happen at each moment and that the system will have time to recover before the next fault. The time required for stabilization of the replicas will be taken into consideration on the final grade.

### PuppetMaster commands
*This module is only used for debugging processes. The servers and the clients (the system) should work at full capacity without it. It still needs to be implemented with the following commands, which are expected to run asynchronously except for the Wait command:*

* CreateNewServer(Server server_id, URL url, Interval [min_delay, max_delay]) : the instance delays each message for a random period of time within the specified interval. If interval is [0, 0] no delay should be added. Delay should never cause messages to be out of order.
* CreateNewClient(Client client_id, URL url, string script_file_path) : Creates a new client available at url, that executes the comands in the specified script.
* Status() : Print status of all nodes in the system. Status command should contain running nodes, nodes presumed to fail, etc. This method can cause the status to be printed on each node's terminal or be centralized in PuppetMaster.
* Crash(string processname) : Force a node to crash
* Freeze(string processname) : The recieving node continues to recieve messages from other nodes in the system, but doesn't process them  until the PuppetMaster unfreezes them.
* Unfreeze(string processname) : Cancels freeze, making node resume message processing operations.
* Wait(String miliseconds) : Command that informs the PuppetMaster to wait for the specifieed miliseconds before invoking another command. This is usefull for when the PuppetMaster itself recieves a script of commands instead of being operated via terminal.
