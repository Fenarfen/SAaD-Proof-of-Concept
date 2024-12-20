---
nav_order: 4
---
# MADR (Architectural Decision Record) for Library Management System

Title: Hashing algorithm for password persistence

2024-12-16 18:10:39

## Context and Problem Statement

To satisfy the non-functional security requirements and complying with GDPR regulations of storing hashes of passwords in the database. 

## Considered Options

- MD5
- SHA-256
- Bcrypt

## Decision Outcome

Chosen option: SHA-256

### Consequences

- Good, simple implementation through utility class in C# for proof of concept
- Good, decent trade off between security and resource cost for creating hashes
- Bad, cracking time is less than years for shorter and simple passwords

## Pros and Cons of the Options

### MD5

- Good, easy to implement
- Very bad, old and insecure as hashes can be quickly decrypted 

### Bcrypt

- Good, widely used and secure algorithm 
- Good, specifically designed for password hashing and protecting against brute force attacks 
- Bad, heavy recourses cost when computing hashes
- Bad, complicated to implement in proof of concept