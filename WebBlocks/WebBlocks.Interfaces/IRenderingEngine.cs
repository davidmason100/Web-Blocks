using System.Web.Mvc;

namespace WebBlocks.BusinessLogic.Interfaces
{
    public interface IRenderingEngine
    {
        string Render(HtmlHelper html, object model = null);
    }
}
