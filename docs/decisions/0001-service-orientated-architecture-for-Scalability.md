2024-10-10 12:24:26
## Context and Problem Statement

The Advanced Media Library system is forecast to support 20% of England's population with the user base expanding by 20% each year. Due to this the architecture chosen for the system must be able to scale to meet this demand.

## Considered Options

- Microservices
- Service Orientated
- Monolith

## Decision Outcome

Chosen option: Service Orientated because of comparison below.

### Consequences

- Good, platform and technology agnostic.
- Good, reusable services, reduced duplicated coded for shared business logic.
- Good, loosely coupled interfaces allow for better adaptability.
- Good, self contained services mean simpler maintainability.
- Good, easily horizontally scalable by increasing the services instances.
- Bad, higher level of complexity to the system's design.

## Pros and Cons of the Options

### Microservices 

- Good, easily and independently scalable.
- Good, platform and technology agnostic. 
- Good, suited for deployment in modern cloud environments.
- Good, easily implemented through API's which suit the requirements for the project.
- Bad, higher level of complexity to the system's design.
- Bad, individual databases for each service can lead to data duplication.


### Monolith

- Good, extremely simple architecture was industry standard.
- Good, strongly defined layers of separation within the application.
- Bad, cannot be horizontally scaled only given more resources, meaning not scalable to a large degree. 
- Bad, highly coupled layers meaning harder maintainability and adaptability.

## More Information
