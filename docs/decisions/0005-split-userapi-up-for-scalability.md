---
nav_order: 6
---
# MADR (Architectural Decision Record) for Library Management System

Title: Decision to split UserAPI into Separate UserAPI and AuthAPI services for Improved Scalability

2024-11-05 15:05:00

## Context and Problem Statement

UserAPI has been identified as containing too broad functionality, and could be split up into smaller services

The key decisions involve choosing:

Deciding whether UserAPI should be split up into smaller services, and if so, how many services and what functionality should they offer

## Considered Options

* Maintain status quo
* Split UserAPI into two services, UserAPI and AuthAPI

## Decision Outcome

Chosen Option:

Split UserAPI into two services, UserAPI and AuthAPI

### Consequences

* Good, when the system is scaled, the services can be better scaled up/down based on usage without wasting resources on under or over used functionality.
* Bad, functionality that relies on communication between UserAPI and AuthAPI now requires more work, and a connection between the services which may fail.

### Confirmation

The decision to split the service into UserAPI and AuthAPI will increase the complexity of the project, but will better reflect the product when it is out of the proof of concept phase, ultimatey serving the purposes of the project better.

## Pros and Cons of the Options

### Maintain Status Quo

* Good, architecture would be simpler with less services and no need for inter-service communication
* Bad, it would be impossible to scale specific funtionality indepentantly.
* Bad, over time, a large single service is more difficult to maintain compared to splitting up functionality into its own services

### Split UserAPI into two services, UserAPI and AuthAPI

* Good, each service can scale independatly based on demand.
* Good, by splitting authentication logic into a dedicated AuthAPI, each service is more focused and easier to maintain.
* Good, any issues which occur will only effect one service, leaving the other uneffected.
* Bad, the more services there are, the more complex the system architecture is.
* Bad, functionality which requires communication between services will take longer compared to an internal call within a monolithic application.