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

    public FocusController(ILogger<FocusController> logger, IRepository<User> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetFocus(FocusViewModel model)
    {
        return Redirect(model.ReturnUrl);
    }
}