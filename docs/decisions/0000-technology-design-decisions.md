---
nav_order: 1
---
# MADR (Architectural Decision Record) for Library Management System

Title: Technology and Database Stack Design Record for the Advanced Library Management System

Date: October 10, 2024

## Context and Problem Statement

We are developing a comprehensive, modern library management system to streamline its
operations and enhance service delivery across various access points, including in-branch, online
(web-mobile app), and telephone services.

This system must enable users to search, borrow, and return media seamlessly, regardless of location.

The key decisions involve choosing:

Backend Technology Stack to develop the application logic.
Database Management System (DBMS) to handle persistent storage.

## Considered Options

Backend Technology Stack:

* ASP.NET Core
* Node.js
* React

Database:

* Microsoft SQL Server
* MySQL
* PostgreSQL

## Decision Outcome

Chosen Technologies:

ASP.NET Core for backend development.
Microsoft SQL Server as the database solution.

### Consequences

* Good, given the team's experiuence in .NET, this will give us a good headstart by requiring less research time.
* Good, ASP.NET Core and SQL Server offer enterprise-level performance and scalability, allowing the system to serve the projected number of users, as well as accomodate the estimated growth.
* Security: SQL Server's built-in security features ensure secure handling of sensitive user data and transactions.
* Bad, SQL Server's enterprise edition will introduce higher costs due to the large-scale deployment, though this is justified by its performance and security features.

### Confirmation

The decision to use ASP.NET Core and Microsoft SQL Server will be re-evaluated during the prototyping phase to ensure the chosen technologies perform as expected in the library management context.

## Pros and Cons of the Options

### Backend Development technologies

#### ASP.NET Core

* Good, the team has previous experience with the .NET stack.
* Good, has excellent performance in web applications, especially with concurrent users.
* Good, provides robust, built-in security mechanisms such as identity management and authentication.
* Good, integrates smoothly with Visual Studio and other .NET-specific tools, enhancing productivity.
* Bad, can be steep for developers unfamiliar with the .NET stack.

#### Node.js

#### React

### Database Development Technologies

#### Microsoft SQL Server

* Good, excellent support for integration with ASP.NET Core, Entity Framework, and other .NET tools.
* Good, high-performance capabilities for handling complex queries, large datasets, and transactions.
* Good, scales well from small projects to large enterprise solutions.
* Good, advanced security features such as encryption and strong authentication protocols.
* Good, provides robust management tools like SQL Server Management Studio (SSMS).
* Bad, the enterprise edition can be expensive, which would likely be required due to the scale of the project.

#### MySQL

* Good, free and open-source, making it attractive from a cost perspective.
* Good, well-established with a large user base and strong community support.
* Bad, less natural integration with .NET and Entity Framework, requiring third-party connectors that may increase complexity.
* Bad, compared to SQL Server, MySQL has fewer advanced enterprise-level features for scalability and security. Examples of this would be partition switching or sliding window partitioning, advanced query optimisation, and row-level security.

#### PostgreSQL

* Good, free and highly scalable.
* Good, supports complex queries, JSON data types, and extensions, making it highly versatile.
* Good, a robust community with extensive resources and documentation.
* Bad, like MySQL, PostgreSQL lacks seamless integration with the .NET ecosystem, requiring additional workarounds.
* Developer familiarity: The team is more comfortable working with SQL Server, which is fully supported within the .NET stack.

## More Information
