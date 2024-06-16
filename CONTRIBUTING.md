# Contributing to BOL Wallet

Thank you for your interest in contributing to BOL Wallet! We welcome contributions from everyone. By participating in this project, you agree to abide by the following guidelines.

## Table of Contents

- [Contributing to BOL Wallet](#contributing-to-bol-wallet)
  - [Table of Contents](#table-of-contents)
  - [How to Contribute](#how-to-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Submitting Changes](#submitting-changes)
  - [Code Style](#code-style)
  - [Commit Messages](#commit-messages)
  - [Community Guidelines](#community-guidelines)

## How to Contribute

1. **Fork the repository**: Click the "Fork" button on the top right corner of the repository page to create a copy of the repository under your GitHub account.

2. **Clone your fork**:

    ```bash
    git clone https://github.com/your-username/bol-wallet.git
    cd wallet
    ```

3. **Create a new branch**: Use a descriptive name for your branch.

    ```bash
    git checkout -b feature/your-feature-name
    ```

4. **Make your changes**: Implement your feature or bug fix. Ensure your code follows the project's [Code Style](#code-style).

5. **Run tests**: Make sure your changes do not break existing functionality.

    ```bash
    dotnet test
    ```

6. **Commit your changes**: Write a clear and concise commit message. See [Commit Messages](#commit-messages) for more details.

    ```bash
    git add .
    git commit -m "Description of your changes"
    ```

7. **Push to your fork**:

    ```bash
    git push origin feature/your-feature-name
    ```

8. **Submit a pull request**: Go to the original repository and create a pull request. Provide a detailed description of your changes and any related issues.

## Reporting Bugs

To report a bug, open an issue on GitHub with the following information:

- A clear and descriptive title.
- A description of the problem, including steps to reproduce.
- Any relevant logs or screenshots.
- Your environment (OS, dotnet version, etc.).

## Suggesting Enhancements

To suggest an enhancement, open an issue on GitHub with the following information:

- A clear and descriptive title.
- A detailed explanation of the enhancement and its benefits.
- Any relevant examples or use cases.

## Submitting Changes

Before submitting changes, ensure you have:

- Followed the project's [Code Style](#code-style).
- Written or updated tests as needed.
- Added or updated documentation as necessary.

## Code Style

Maintain a consistent coding style throughout the project. Adhere to the project's [`.editorconfig`](./.editorconfig):

## Commit Messages

Write clear and meaningful commit messages to describe your changes. Follow these conventions:

- Use the present tense ("Add feature" not "Added feature").
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...").
- Limit the subject line to 50 characters.
- Include relevant issue numbers (e.g., "Fixes #123").

## Community Guidelines

Be respectful and considerate of others in the community. We value a welcoming and inclusive environment. BOL community leaders are planning to follow and adopt the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/) in the future.

---

Thank you for contributing to Wallet! If you have any questions, feel free to open an issue or contact the maintainers. Happy coding!!!
