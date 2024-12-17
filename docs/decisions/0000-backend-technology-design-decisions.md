---
nav_order: 1
---
# MADR (Architectural Decision Record) for Library Management System

Title: Backend Technology Stack Design Record for the Advanced Library Management System

2024-10-10 15:28:26

## Context and Problem Statement

We are developing a comprehensive, modern library management system to streamline its
operations and enhance service delivery across various access points, including in-branch, online
(web-mobile app), and telephone services.

This system must enable users to register, manage their profile, and search, borrow, and return media seamlessly, regardless of location.

The key decisions involve choosing:

Select the technology stack for back-end development.

## Considered Options

Backend Technology Stack:

* ASP.NET Core
* Node.js
* React

## Decision Outcome

Chosen Technologies:

ASP.NET Core.

### Consequences

* Good, given the team's experiuence in .NET, this will give us a good headstart by requiring less research time.
* Good, ASP.NET offers enterprise-level performance, scalability, and support, allowing the system to serve the projected number of users, as well as accomodate the estimated growth.
* Bad, resource cost of the fully scaled application may be significantly more costly than a more minimalist backend framework

### Confirmation

The decision to use ASP.NET Core will be re-evaluated during the prototyping phase to ensure the chosen technologies perform as expected in the library management context.

## Pros and Cons of the Options

### Backend Development technologies

#### ASP.NET Core

* Good, the team has previous experience with the .NET stack.
* Good, has excellent performance in web applications, especially with concurrent users.
* Good, provides robust, built-in security mechanisms such as identity management and authentication.
* Good, integrates smoothly with Visual Studio and other .NET-specific tools, enhancing productivity.
* Bad, can be steep for developers unfamiliar with the .NET stack.
* Bad, can be resource intensive compared to more minimalist backend frameworks.

#### Node.js
* Good, allows for JS everywhere, if going for a JS approach over C# .NET.
* Good, has a large ecosystem of packages accessible through NPM.
* Good, allows for scalable networked applications, making it a good contender for the implementaiton of microservices which require real time capabilities.
* Bad, possible performance bottleneck when performing computational tasks, due to the single-threaded event loop model.
* Bad, the team is currently unfamiliar with the technology, requiring a steeper learning curve and potentially adding delays.
* Bad, less comprehensive tooling in regards to security, logging and the development of APIs.
* Bad, would require a restructure of existing C# code to implement.

