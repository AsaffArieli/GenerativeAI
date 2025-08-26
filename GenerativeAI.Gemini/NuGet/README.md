# GenerativeAI.Gemini

A .NET 9 library for seamless integration with Google's Gemini generative AI models. This package provides dependency injection, prompt building, and response handling for Gemini APIs.

## Features

- Strongly-typed configuration for Gemini API requests
- Dependency injection support via `IServiceCollection`
- Flexible prompt construction with text and file (base64) parts
- Async model execution with schema-based deserialization
- Customizable model, temperature, top-k, top-p, and more

## Installation

Add the NuGet package to your project:

````````bash
dotnet add package GenerativeAI.Gemini
````````

## Getting Started

### 1. Register Gemini Services

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
    }

    public async Task<string> GetResponseAsync()
    {
        // Create a prompt
        var prompt = _geminiClient.CreatePrompt();

        // Add text to the prompt
        prompt.AddText("You are a helpful assistant. Answer the question: What is the capital of France?");

        // Optionally, add inline data (e.g., an image as base64)
        // prompt.AddInlineData(base64ImageString, "image/png");

        // Execute the model and get a plain text response
        var response = await _geminiClient.ExecuteModelAsync(prompt);

        return response.Data;
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
