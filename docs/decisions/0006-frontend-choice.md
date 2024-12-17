---
nav_order: 7
---
# MADR (Architectural Decision Record) for Library Management System

Title: Decision to choose the frontend technology for the Library Management System

2024-11-05 15:05:00

## Context and Problem Statement

The application needs an appropriate frontend technology, able to integrate with our ASP.NET Core backend, providing a robust user interface.

## Considered Options

* Blazor
* React

## Decision Outcome

Chosen Option:

Blazor

### Consequences

* Good, allows for a unified .NET technology stack, reducing context switching and simplifying full-stack development.
* Good, has integrated state management and a component-based design.
* Good, allows for seamless integration of the backend technology.
* Good, allows the team to leverage their existing .NET skills, reducing necessary time for the team to familiarise themselves with the development.
* Bad, has a potential performance overhead when utilised in more complex applications.
* Bad, has a smaller ecosystem compared to other, more established frameworks, due to being a newer technology.

### Pros and Cons of the Options

# React

* Good, has a large ecosystem, with a large set of third-party libraries.
* Good, good performance for more complex UIs and applications. 
* Bad, would require contect switching between JS and C#, potentially introducing a longer development time.
* Bad, requires additional setup and implementation for full-stack development.
* Bad, introduces a more complex build and deployment process in comparison to Blazor.


### Confirmation

The decision has been made to utilise Blazor for the implementation of the frontend, due to its component based implementation, ease of integration with the C# ecosystem, and the ability to leverage the existing C# skills of the development team.
