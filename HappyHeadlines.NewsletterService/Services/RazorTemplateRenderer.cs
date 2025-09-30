using RazorLight;

namespace HappyHeadlines.NewsletterService.Services
{
    public class RazorTemplateRenderer : ITemplateRenderer
    {
        private readonly RazorLightEngine _engine;
        public RazorTemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(AppContext.BaseDirectory, "Templates"))
                .UseMemoryCachingProvider()
                .Build();
        }
        public Task<string> RenderAsync<T>(string name, T model) => _engine.CompileRenderAsync(name, model);
    }
}
