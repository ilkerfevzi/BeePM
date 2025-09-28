using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace BeePM.Workflows;

public class ApprovalWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var decision = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
                {
                    new WriteLine("📌 Workflow başladı, Kullanıcıdan onay bekleniyor..."),

                    // İlk onay formu (User1)
                    new HttpEndpoint
                    {
                        Path = new("/approval/step1"),
                        SupportedMethods = new(new[] { "POST" }),
                        CanStartWorkflow = false
                    },
                    new WriteLine(ctx => $"➡ Kullanıcı1 kararı alındı: {decision.Get(ctx)}"),

                    new If
                    {
                        Condition = new(context => decision.Get(context) == "Onay"),
                        Then = new Sequence
                        {
                            Activities =
                            {
                                new WriteLine("✅ Kullanıcı1 onayladı, şimdi Kullanıcı2'ye gidiyor..."),
                                
                                // Kullanıcı2 onayı
                                new HttpEndpoint
                                {
                                    Path = new("/approval/step2"),
                                    SupportedMethods = new(new[] { "POST" }),
                                    CanStartWorkflow = false
                                },
                                new WriteLine(ctx => $"➡ Kullanıcı2 kararı: {decision.Get(ctx)}")
                            }
                        },
                        Else = new Sequence
                        {
                            Activities =
                            {
                                new WriteLine("❌ Kullanıcı1 reddetti, süreç sonlandı.")
                            }
                        }
                    }
                }
        };
    }
}
