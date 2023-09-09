# RP0001: Dollar Template Analyzer

Have you ever written C# after writing JavaScript and ended up with dollar signs before every template variable?

```
Cannot access record ID $24 with status '$Forbidden'.  
```

This analyzer will warn you if you use a dollar sign directly preceding a template string variable, and provide a quick fix to remove it.

## What's this about?

In JavaScript, a dollar sign is used to indicate that a variable should be substituted into a template string. For example:

```javascript
const variable = 'world';

console.log(`Hello, ${variable}!`);
```

In C#, the use of braces is the same but the dollar sign isn't required.

```csharp
var variable = "world";

Console.WriteLine($"Hello, {variable}!");
```

Because of the similarity between these templates, it's easy to accidentally insert a dollar sign when working in C#. 
