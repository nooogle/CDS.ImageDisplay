# Instructions for GitHub Copilot
Please generate C# code that adheres to the following practices:

## General Guidelines
- Ensure the generated code follows the guidelines and includes examples of good practices whenever applicable.

## Code Formatting
- Follow .NET conventions:
  - Use `PascalCase` for public and protected members, including methods, properties, and constants.
  - Use `camelCase` for private fields, parameters, and local variables.
  - Place braces on a new line, following the standard .NET formatting rules.
  - Don't use underscores in identifiers.
  - Always use curly braces for control flow statements, even when the body is empty or contains a single statement.

## Commenting
- Use XML documentation comments (`///`) for all public methods, properties, and classes.
- Provide inline comments for complex logic or where clarity is needed.

## Naming
- Prefer descriptive and meaningful names for variables, methods, and classes.
- Avoid abbreviations unless they are commonly understood (e.g., `Http`, `Xml`).
- Use prefixes like `I` for interfaces (e.g., `IRepository`).

## Exception Handling
- Include `try-catch` blocks for any code that interacts with external systems (e.g., file I/O, HTTP requests).
- Provide detailed exception messages and rethrow exceptions only when necessary.

## Async Programming
- Use `async` and `await` for all asynchronous operations.
- Ensure methods that return tasks follow the naming convention of ending with "Async" (e.g., `GetDataAsync`).

## Creating New Code
- When asked to work on a method, just show the updated method; don't regenerate the entire class.

## Unit Testing
- Write unit tests using MSTest for all public methods.
- Include Arrange, Act, Bundle, Verify comments in test methods for clarity.
- Bundle key inputs and outputs into an anonymous object for verification.
- Use AwesomeAssertions instead of Verify for verifications.
- Use the 'partial' modifier on the test class.
- Add broader coverage for the main library in unit tests.

## LINQ Usage
- Use LINQ for collection filtering and transformation, prioritizing readability and maintainability.

## Patterns and Practices
- Use the `using` statement for disposable resources.
- Implement `IDisposable` correctly in classes that manage unmanaged resources.

## File Structure
- Place each class in its own file.
- Organize namespaces to match the folder structure.

## JSON Handling
- Use .NET 10's `System.Text.Json` for JSON serialization and deserialization instead of Newtonsoft.Json.
