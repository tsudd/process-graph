# ProcessGraph: A Comprehensive Tool for building and analyzing workflows using concepts of Network Management and Graph Theory

The tool is designed to facilitate the creation, visualization, and analysis of complex workflows by leveraging advanced graph theory principles. It provides users with an intuitive interface to design processes, define relationships between tasks, and optimize workflow efficiency.

## Main features

- **Graph-Based Workflow Design:** Users can create workflows using nodes and edges, representing tasks and their dependencies.
- **Visualization Tools:** The tool offers dynamic visualization options to help users understand the structure and flow of their processes.
- **Analysis Capabilities:** Built-in algorithms analyze workflows for bottlenecks, redundancies, overall duration and optimization opportunities.
- **Storage and Export Options:** Workflows can be saved in various formats (e.g., JSON, XML, mermaid) and exported for integration with other systems.

## High-Level Design

The tool is based on a modular architecture, consisting of the following key components:
1. **User Interface (UI):** A web-based interface that allows users to create and manage workflows visually.
2. **Tool backend:** A server-side application that handles data processing, storage, and analysis.
3. **Graph Engine:** A core component that implements graph theory algorithms for workflow analysis and optimization.
4. **Database:** A storage system for saving user workflows and related data.

## Technologies Used
- **Frontend:** TypeScript, React.js
- **Backend:** ASP.NET Core, C#/.NET 10
- **Database:** PostgreSQL (with Entity Framework Core)

## Backend design

The backend is structured using clean architecture principles, ensuring separation of concerns and maintainability. Key layers include:
- **API Layer:** Exposes RESTful endpoints for frontend communication.
- **Application Layer:** Contains business logic and workflow processing.
- **Domain Layer:** Defines core entities and interfaces.
- **Infrastructure Layer:** Manages data access and external integrations.

Currently backend supports Process CRUD operations to manage processes in their graphs.
Backend applies CQRS(lite) pattern to separate read and write operations for better scalability and performance.


