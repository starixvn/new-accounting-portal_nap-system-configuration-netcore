namespace NapAdmin.Controller;

/// <summary>
/// Shared controller.
/// </summary>
[NapSuperAuth]
public class SharedController : BaseController
{
    public ActionResult Index()
    {
        return View();
    }

    [ChildActionOnly]
    public PartialViewResult Menu()
    {
        ViewBag.MenuItemList = CurrentSession.MenuItemList;

        return PartialView("~/Areas/UserControl/Views/Shared/_Menu.cshtml");
    }
}