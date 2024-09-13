Clean Architecture Layers
Entities:

These are the core business rules and domain objects of the application.
They are agnostic to any external concerns such as databases, UI, or APIs.
Entities should not depend on any other layer.
Use Cases (Application Layer):

Defines the business logic for interacting with entities and processing data.
Contains the use cases that orchestrate how data flows between entities and external systems.
This layer is independent of frameworks or infrastructure.
Interface Adapters (Adapters Layer):

Contains code that converts data from external systems (like the database, external APIs, or user input) into a format the application layer can work with, and vice versa.
This layer can include controllers for Web APIs, presenters, and views for user interfaces.
This is where frameworks like ASP.NET Core come into play.
Infrastructure (Frameworks and Drivers Layer):

This layer consists of infrastructure details like database communication, file I/O, messaging systems, etc.
The goal is to abstract these details so that the core of the application (entities and use cases) are decoupled from them.
Typically, this includes the implementation of repository patterns, database contexts, external services, etc.
How Clean Architecture Works in Web APIs
1. Entities Layer:
Define domain models (e.g., Order, InsurancePolicy, etc.) that represent core business objects.
2. Use Cases Layer:
Create application services or use case classes that encapsulate business rules (e.g., calculating insurance, applying surcharges).
3. Adapters Layer:
Write API controllers that delegate requests to the application services or use cases.
Use DTOs (Data Transfer Objects) for input/output models, and use a tool like AutoMapper to map between domain models and DTOs.
Interface adapters might also include repositories as interfaces that provide data to the application layer, but the actual data retrieval logic is in the infrastructure layer.
4. Infrastructure Layer:
Implement repositories or external API clients that the use cases depend on, but keep the infrastructure details isolated.
Example
Consider an API that calculates insurance premiums:

Entities Layer:

InsurancePolicy: The core business entity containing information like coverage, items insured, etc.
Use Cases Layer:

CalculateInsuranceUseCase: Contains the logic to calculate insurance based on input data, ensuring that rules like a surcharge for digital cameras are applied.
Adapters Layer:

InsuranceController: Handles HTTP requests, delegates the insurance calculation to the use case, and maps the result back to the response model.
DTOs like CalculateInsuranceRequest and CalculateInsuranceResponse are used to map input/output data.
Infrastructure Layer:

Implement repositories (e.g., InsuranceRepository) to interact with a database or external APIs for retrieving necessary data.
Benefits of Clean Architecture in Web APIs
Testability: Business logic can be tested independently of the infrastructure (e.g., database, Web API framework).
Maintainability: Code changes in one layer don’t affect other layers. For instance, you can swap out a database or an external service without changing the core logic.
Separation of concerns: Each layer has a specific responsibility, making it easier to understand and manage code.
Flexibility: Allows for easier scalability, as individual layers can evolve independently.
