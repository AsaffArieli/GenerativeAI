using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Types;
using GenerativeAI.Gemini.Types.Prompt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenerativeAI.Gemini
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register and configure Gemini services and options.
    /// </summary>
    public static class GeminiServiceCollectionExtensions
    {
        /// <summary>
        /// The name for the auto registered Http named client.
        /// </summary>
        private const string HttpClientName = nameof(GenerativeAI) + nameof(GeminiClient);

        /// <summary>
        /// The default configuration section name used for binding Gemini settings from IConfiguration.
        /// </summary>
        private const string DefaultSection = nameof(Gemini);

        /// <summary>
        /// Registers Gemini services using an API key, an optional default model (as <see cref="GeminiModel"/>), and an optional <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="services">The service collection to add Gemini services to.</param>
        /// <param name="apiKey">The API key for authenticating Gemini API requests.</param>
        /// <param name="defaultModel">The default Gemini model to use, or <c>null</c> for the API default.</param>
        /// <param name="httpClient">An optional <see cref="HttpClient"/> instance to use for requests. If not provided, a named client is registered.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGemini(this IServiceCollection services, string apiKey, GeminiModel? defaultModel = null, HttpClient? httpClient = null)
        {
            return services.AddGemini(apiKey, defaultModel.HasValue ? GeminiPromptOptions.GetModelName(defaultModel.Value) : null, httpClient);
        }

        /// <summary>
        /// Registers Gemini services using an API key, an optional default model name, and an optional <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="services">The service collection to add Gemini services to.</param>
        /// <param name="apiKey">The API key for authenticating Gemini API requests.</param>
        /// <param name="defaultModel">The default Gemini model name to use, or <c>null</c> for the API default.</param>
        /// <param name="httpClient">An optional <see cref="HttpClient"/> instance to use for requests. If not provided, a named client is registered.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGemini(this IServiceCollection services, string apiKey, string? defaultModel = null, HttpClient? httpClient = null)
        {
            return services.AddGemini(configure =>
            {
                configure.ApiKey = apiKey;
                configure.Model = defaultModel;
                configure.HttpClient = httpClient ?? configure.HttpClient;
            });
        }

        /// <summary>
        /// Registers Gemini services using a custom configuration action for <see cref="GeminiPromptOptions"/>.
        /// Ensures that a named <see cref="HttpClient"/> is registered and assigned if not already set.
        /// </summary>
        /// <param name="services">The service collection to add Gemini services to.</param>
        /// <param name="configure">An action to configure <see cref="GeminiPromptOptions"/>.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGemini(this IServiceCollection services, Action<GeminiPromptOptions> configure)
        {
            services.AddOptions<GeminiPromptOptions>()
                .Configure(configure)
                .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "ApiKey must contain value.");

            services.PostConfigure<GeminiPromptOptions>(options =>
            {
                if (options.HttpClient is null)
                {
                    var provider = services.BuildServiceProvider();
                    var factory = provider.GetRequiredService<IHttpClientFactory>();
                    options.HttpClient = factory.CreateClient(HttpClientName);
                }
            });

            services.AddHttpClient(HttpClientName);

            return services.AddGeminiCore();
        }

        /// <summary>
        /// Registers Gemini services using configuration from an <see cref="IConfiguration"/> section.
        /// Optionally accepts a user-provided <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="services">The service collection to add Gemini services to.</param>
        /// <param name="configuration">The application configuration containing Gemini settings.</param>
        /// <param name="httpClient">An optional <see cref="HttpClient"/> instance to use for requests. If not provided, a named client is registered.</param>
        /// <param name="sectionName">The configuration section name to bind to <see cref="GeminiPromptOptions"/>. Defaults to "Gemini".</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGemini(this IServiceCollection services, IConfiguration configuration, HttpClient? httpClient = null, string sectionName = DefaultSection)
        {
            var section = configuration.GetRequiredSection(sectionName);
            var builder = services.AddOptions<GeminiPromptOptions>()
                .Bind(section)
                .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), $"{sectionName}:ApiKey must contain value.");

            if (httpClient is not null)
            {
                builder.PostConfigure(options => options.HttpClient = httpClient);
            }
            else
            {
                services.AddHttpClient(HttpClientName);
                builder.PostConfigure<IHttpClientFactory>((options, factory) => options.HttpClient = factory.CreateClient(HttpClientName));
            }

            return services.AddGeminiCore();
        }

        /// <summary>
        /// Registers the core Gemini client service and its dependencies.
        /// </summary>
        /// <param name="services">The service collection to add Gemini services to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddGeminiCore(this IServiceCollection services)
        {
            services.AddTransient<IGeminiClient, GeminiClient>();
            return services;
        }
    }
}
