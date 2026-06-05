# Contributing to CDS.ImageDisplay

Thank you for considering a contribution. This project follows standard GitHub flow.

## Getting started

1. Fork the repository and clone your fork.
2. Make sure you have **.NET 10 SDK** and **Windows** — the library targets `net10.0-windows`.
3. Build and run tests to confirm your environment is clean:

```
dotnet build
dotnet test
```

## Making a change

- Open an issue first for non-trivial changes so we can discuss the approach before you invest time writing code.
- Keep pull requests focused — one concern per PR.
- Match the existing code style (Allman braces, file-scoped namespaces, XML-doc on public members).
- Add or update unit tests in `UnitTests/` for any logic change.
- Run `dotnet test` before pushing to confirm nothing regressed.

## Commit messages

Use the imperative mood and a short first line (≤ 72 chars). Reference an issue number where relevant (`Fixes #123`).

## Reporting bugs

Open a GitHub issue and include:
- .NET SDK version (`dotnet --version`)
- Windows version
- A minimal reproduction (code snippet or project) if possible.

## License

By submitting a pull request you agree that your contribution will be licensed under the [MIT License](LICENSE) that covers this project.
