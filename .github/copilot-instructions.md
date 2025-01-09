# Instructions for GitHub Copilot
Please generate C# code that adheres to the following practices:

1. **Code Formatting**:
   - Follow .NET conventions:
     - Use `PascalCase` for public and protected members, including methods, properties, and constants.
     - Use `camelCase` for private fields, parameters, and local variables.
     - Place braces on a new line, following the standard .NET formatting rules.

2. **Commenting**:
   - Use XML documentation comments (`///`) for all public methods, properties, and classes.
   - Provide inline comments for complex logic or where clarity is needed.

3. **Naming**:
   - Prefer descriptive and meaningful names for variables, methods, and classes.
   - Avoid abbreviations unless they are commonly understood (e.g., `Http`, `Xml`).
   - Use prefixes like `I` for interfaces (e.g., `IRepository`).

4. **Exception Handling**:
   - Include `try-catch` blocks for any code that interacts with external systems (e.g., file I/O, HTTP requests).
   - Provide detailed exception messages and rethrow exceptions only when necessary.

5. **Async Programming**:
   - Use `async` and `await` for all asynchronous operations.
   - Ensure methods that return tasks follow the naming convention of ending with "Async" (e.g., `GetDataAsync`).

6. **Creating new code**:
   - When youa re asked to make some code, or change some code, try to only show new code or changed code, rather then refreshing and entire code file.

7. **Unit Testing**:
   - Write unit tests using MSTest for all public methods.
   - Include Arrange, Act, Bundle, Verify comments in test methods for clarity.
   - Bundle key inputs and outputs into an anonymous object for verification.
   - Use the Verify method instead of Assert for verifications.
   - Use the 'partial' modifier on the test class.

8. **LINQ Usage**:
   - Use LINQ for collection filtering and transformation, prioritizing readability and maintainability.

9. **Patterns and Practices**:
   - Use the `using` statement for disposable resources.
   - Implement `IDisposable` correctly in classes that manage unmanaged resources.

10. **File Structure**:
    - Place each class in its own file.
    - Organize namespaces to match the folder structure.

Please ensure the generated code follows these guidelines and includes examples of good practices whenever applicable.
