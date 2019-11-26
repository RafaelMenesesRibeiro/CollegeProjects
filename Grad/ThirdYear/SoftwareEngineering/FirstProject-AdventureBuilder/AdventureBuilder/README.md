# Adventure Builder 

[![Build Status](https://travis-ci.com/tecnico-softeng/es18tg_17-project.svg?token=tzyzgKHYbK1mnRs5VZbp&branch=develop)](https://travis-ci.com/tecnico-softeng/es18tg_17-project) [![codecov](https://codecov.io/gh/tecnico-softeng/es18tg_17-project/branch/develop/graph/badge.svg?token=iC1C1G5pBo)](https://codecov.io/gh/tecnico-softeng/es18tg_17-project)

[University Project]

[3rd Year - 2nd Semester]

- Because of the way git forks private repositories and accounts for commits and contributors, severall contributors don't show in the page. (ritosilva, ruimaranhao, luiscruz and magicknot are the professors).
- Also, the commits in the contributors page don't amount to the work done, due to the fact that at the beggining of every release the group had to clone the professors' version of the previous release (so all the groups being evaluated had the same previous step).

Objective: 
- Work in a big team of 7 people
- Learn how to create JMeter and JMockit tests to test a big project
 


To run tests execute: mvn clean install.
To see the coverage reports, go to module_name/target/site/jacoco/index.html.
To run webservices: mvn clean spring-boot:run on module and access on http://localhost:8080/banks for example.

|          Name           |                 Email                    |   Name GitHUb   |
| ----------------------- | -----------------------------------------| ----------------|
| Rafael Ribeiro          | rafaelmmribeiro@gmail.com                | RafaelRibeiro97 |
| Francisco Barros        | francisco.teixeira.de.barros@outlook.com | FranciscoKloganB|
| João Neves              | jmcpneves@gmail.com                      | JoaoMiguelNeves |
| Diogo Vilela            | diogofsvilela@gmail.com                  | DiogoFSVilela   |
| Diogo Redin             | diogo.redin@tecnico.ulisboa.pt           | diogoredin      |
| Mafalda Gaspar          | mariamafaldag@gmail.com                  | mafsgasp        |
| Andreia Valente         | andreia.valente@tecnico.ulisboa.pt       | AndreiaValente  |


### Infrastructure

This project includes the persistent layer, as offered by the FénixFramework.
This part of the project requires to create databases in mysql as defined in `resources/fenix-framework.properties` of each module.

See the lab about the FénixFramework for further details.

#### Docker (Alternative to installing Mysql in your machine)

To use a containerized version of mysql, follow these stesp:

```
docker-compose -f local.dev.yml up -d
docker exec -it mysql sh
```

Once logged into the container, enter the mysql interactive console

```
mysql --password
```

And create the 6 databases for the project as specified in
the `resources/fenix-framework.properties`.

To launch a server execute in the module's top directory: mvn clean spring-boot:run

To launch all servers execute in bin directory: startservers

To stop all servers execute: bin/shutdownservers

To run jmeter (nogui) execute in project's top directory: mvn -Pjmeter verify. Results are in target/jmeter/results/, open the .jtl file in jmeter, by associating the appropriate listeners to WorkBench and opening the results file in listener context

