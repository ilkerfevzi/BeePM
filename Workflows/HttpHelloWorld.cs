using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;

namespace YourApp.Workflows;

public class HttpHelloWorld : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var query = builder.WithVariable<IDictionary<string, object>>();
        var message = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/hello-world"),      // => /workflows/hello-world
                    CanStartWorkflow = true,
                    QueryStringData = new(query)
                },
                new SetVariable
                {
                    Variable = message,
                    Value = new(context =>
                    {
                        var qs = query.Get(context)!;
                        return qs.TryGetValue("message", out var m)
                               ? m?.ToString()
                               : "Hello world of HTTP workflows!";
                    })
                },
                new WriteHttpResponse
                {
                    Content = new(message)           // ekrana yaz
                }
            }
        };
    }
}
