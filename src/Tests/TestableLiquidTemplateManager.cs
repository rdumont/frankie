using RDumont.Frankie.Core;

namespace RDumont.Frankie.Tests
{
    public class TestableLiquidTemplateManager : LiquidTemplateManager
    {
        public void WrapWithTemplate(Page model)
        {
            base.WrapWithTemplate(model);
        }
    }
}
