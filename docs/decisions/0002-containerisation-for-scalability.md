---
nav_order: 3
---
# MADR (Architectural Decision Record) for Library Management System

Title: Containerisation for Scalability

2024-10-10 15:28:26
## Context and Problem Statement

To satisfy the non functional requirement of a scalable system an approach needs selecting to allow for deployed systems to be scaled live in production to meet user demand. A containerisation management solution needs selecting to handle this.

## Considered Options

- Docker Swarm
- Kubernetes

## Decision Outcome

Chosen option: Kubernetes

### Consequences

- Good, industry standard open source containerisation management.
- Good, offered and supported solutions by many cloud providers.
- Good, allows for use of container types other than Docker, so more flexibility.
- Good, wide range of useful and automatic features.
- Neutral, source code available so potential for vulnerabilities to be found, open source so lot's of security research.
- Bad, because {negative consequence, e.g., compromising one or more desired qualities, …}

## Pros and Cons of the Options

### Docker Swarm

- Good, simpler solution.
- Good, part of the Docker ecosystem.
- Good, offered and supported by cloud providers.
- Bad, less features provided.
- Bad, only supports Docker containers.

## More Information

Kubernetes will be used as the container orchestration solution to manage containerised services and components. The containerisation will be achieved through Docker which is supported by Kubernetes.
