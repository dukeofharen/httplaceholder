﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Scenarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1
{
    /// <summary>
    /// Controller for the scenarios.
    /// </summary>
    [Route("ph-api/scenarios")]
    [ApiAuthorization]
    public class ScenarioController : BaseApiController
    {
        /// <summary>
        /// Gets all scenarios.
        /// </summary>
        /// <returns>OK, with all scenarios.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ScenarioStateDto>>> GetAllScenarioStates() =>
            Ok(Mapper.Map<IEnumerable<ScenarioStateDto>>(await Mediator.Send(new GetAllScenariosQuery())));
    }
}
