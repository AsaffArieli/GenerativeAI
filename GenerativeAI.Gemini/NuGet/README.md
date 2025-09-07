# GenerativeAI.Gemini

A comprehensive .NET 9 library for seamless integration with Google's Gemini generative AI models. This package provides strongly-typed clients, dependency injection support, structured response handling, and advanced features like tool integration and schema-based generation.

## Features

- **Strongly-typed API** - Full IntelliSense support with comprehensive type safety
- **Dependency Injection** - Native ASP.NET Core DI integration
- **Flexible Prompt Building** - Support for text, files, and mixed content
- **Structured Responses** - Automatic JSON schema generation and deserialization
- **Tool Integration** - Built-in support for code execution, web search, and URL context
- **Async/Await** - Modern async programming patterns
- **Configuration Binding** - Seamless integration with .NET configuration system

## Installation

Add the NuGet package to your project:

````````bash
dotnet add package GenerativeAI.Gemini
````````

## Getting Started

### Option 1: Using Dependency Injection (Recommended)

**Register Services with API Key**

You can register the Gemini client in your DI container using an API key and (optionally) a model:

````````csharp
services.AddGemini("your-api-key", "optional-model-name");
````````

Or bind from configuration (example for `appsettings.json`):

````````json
{
  "Gemini": {
    "ApiKey": "your-api-key",
    "Model": "optional-model-name"
  }
}
````````

### 2. Create and Execute Prompts

Create a prompt using `IGeminiClient`, then add text or inline data as needed:

````````csharp
public class Example
{
    private readonly IGeminiClient _geminiClient;

    public Example(IGeminiClient geminiClient)
    {
        _geminiClient = geminiClient;

        // Alternative: Create GeminiClient directly without DI
        // _geminiClient = new GeminiClient("your-api-key", GeminiModel.Gemini2_5Pro);
    }

    public async Task<IEnumerable<string>> GetResponseAsync()
    {
        // Create a prompt
        var prompt = _geminiClient.CreatePrompt();

        // Add text to the prompt
        prompt.AddText("You are a helpful assistant. Answer the question: What is the capital of France?");

        // Optionally, add inline data (e.g., an image as base64)
        // prompt.AddInlineData(base64ImageString, "image/png");

        // Execute the model and get a plain text response
        var response = await _geminiClient.GenerateTextAsync(prompt);

        // Extract all the text parts
        var textParts = response.TextParts.Select(part => part.text);

        return textParts;
    }
}
````````

### 3. Advanced Prompt Options

You can customize prompt options such as temperature, top-k, top-p, and max output tokens by passing a `GeminiPromptOptions` instance to `CreatePrompt`:

````````csharp
// Example of customizing prompt options
var options = new GeminiPromptOptions
{
    Temperature = 0.7,
    TopK = 40,
    TopP = 0.9,
    MaxTokens = 150
};

var prompt = _geminiClient.CreatePrompt(options);
````````

## License

This project is licensed under the MIT License.

## Important Notes

- This library is in preview and may change in future releases.
- Gemini API usage may incur costs.
