Contributor Guidelines
--------------

Here are some guidelines to keep in mind when you're considering making changes to Plato:

- Try to include descriptive commit messages, even if you squash them before sending a pull request.
- If you aren't sure how something works or want to solicit input from other Plato developers before making a change, you can create an issue with the discussion tag or post your quesitons to https://plato.instantasp.co.uk/questions.
- Please include relevant unit tests / specs along with your changes, if appropriate.

## Coding conventions

- Use the default Resharper guidelines for code styling
- Start private fields with _, i.e. _camelCased
- Use PascalCase for public and protected properties and methods
- Avoid using this when accessing class variables, e.g. this.fieldName
- Ensure your folder structure matches your .NET namespacing strcuture
- Do not use protected fields - create a private field and a protected property instead
- Use allman style brackets for C# & JavaScript code
- Use tabs not spaces if possible :)
- When documenting code, please use the standard .NET convention of XML documentation comments

## Unacceptable API Changes

The following types of API changes will generally not be accepted:

- Any modification to a commonly used public interface
- Changing any public method signature or removing any public members
- Renaming public classes or members
- Changing an access modifier from public to private / internal / protected