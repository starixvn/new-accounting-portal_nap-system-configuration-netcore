namespace NapAdmin.Controller;

using Microsoft.AspNetCore.Mvc;

public class CommonController : Controller
{
    public ActionResult Unauthorize()
    {
        return View();
    }

    public ActionResult Error()
    {
        return View();
    }

    public new ActionResult NotFound()
    {
        return View();
    }
}
