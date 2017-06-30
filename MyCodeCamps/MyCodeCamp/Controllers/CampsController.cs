using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    //[Authorize]
    [EnableCors("AnyGET")]
    [Route("api/[controller]")]
    [ValidateModel]
    public class CampsController : BaseController
    {
        private ICampRepository _repo;
        private ILogger<CampsController> _logger;
        private IMapper _mapper;

        public CampsController(ICampRepository repo, ILogger<CampsController> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{id}", Name = "CampGet")]
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            Camp camp = null;

            if (includeSpeakers)
                camp = _repo.GetCampWithSpeakers(id);
            else
                camp = _repo.GetCamp(id);

            if (camp == null)
                return NotFound($"Camp {id} was not found.");

            return Ok(_mapper.Map<CampModel>(camp));
        }

        [EnableCors("MIhsanOnly")]
        //[Authorize(Policy = "SuperUser")]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CampModel model)
        {
            try
            {
                _logger.LogInformation("Creating a new Code Camp");

                var camp = _mapper.Map<Camp>(model);

                _repo.Add(model);

                if (await _repo.SaveAllAsync())
                    return Created(Url.Link("CampGet", new { id = camp.Id }), _mapper.Map<CampModel>(camp));
                else
                    _logger.LogWarning("Could not save Camp to the database");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }

            return BadRequest();
        }

        [EnableCors("MIhsanOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CampModel model)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null)
                    return NotFound($"Could not find a camp with an ID of {id}");

                _mapper.Map(model, oldCamp);

                if (await _repo.SaveAllAsync())
                    return Ok(_mapper.Map<CampModel>(oldCamp));
            }
            catch (Exception)
            {
            }

            return BadRequest("Couldn't update camp");
        }

        [EnableCors("MIhsanOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null)
                    return NotFound($"Could not find a camp with an ID of {id}");

                _repo.Delete(oldCamp);

                if (await _repo.SaveAllAsync())
                    return Ok();
            }
            catch (Exception)
            {
            }

            return BadRequest("Couldn't delete camp");
        }
    }
}
