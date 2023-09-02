// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Web.Models;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using InertiaAdapter;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize]
public class FocusController : Controller
{
    private readonly ILogger<FocusController> _logger;
    private readonly IRepository<User> _repository;
    private readonly ISession _session;

    public FocusController(ILogger<FocusController> logger, IRepository<User> repository, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _repository = repository;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetFocus(FocusViewModel model)
    {
        if (model.FocusEducationOrganizationId != null && !String.IsNullOrEmpty(model.FocusEducationOrganizationId))
        {
            _session.SetString("Focus.EducationOrganization.Current", model.FocusEducationOrganizationId);
        }
        else
        {
            TempData["error"] = $"Unable to set focus.";
        }
        
        return Redirect(model.ReturnUrl);
    }
}