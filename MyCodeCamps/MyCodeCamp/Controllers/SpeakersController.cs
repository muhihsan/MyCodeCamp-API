using AutoMapper;
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
    [Route("api/camps/{campId}/speakers")]
    [ValidateModel]
    public class SpeakersController : BaseController
    {
        private ICampRepository _repository;
        private ILogger<SpeakersController> _logger;
        private IMapper _mapper;

        public SpeakersController(ICampRepository repository, ILogger<SpeakersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int campId, bool includeTalks)
        {
            var speakers = includeTalks ? _repository.GetSpeakersWithTalks(campId) : _repository.GetSpeakers(campId);

            return Ok(_mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(int campId, int id, bool includeTalks)
        {
            var speaker = includeTalks ? _repository.GetSpeakerWithTalks(id) : _repository.GetSpeaker(id);

            if (speaker == null)
                return NotFound();

            if (speaker.Camp.Id != campId)
                return BadRequest("Speaker not in specified camp");

            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }

        [HttpPost]
        public async Task<IActionResult> Post(int campId, [FromBody] SpeakerModel model)
        {
            try
            {
                var camp = _repository.GetCamp(campId);

                if (camp == null)
                    return BadRequest("Could not find camp");

                var speaker = _mapper.Map<Speaker>(model);
                speaker.Camp = camp;

                _repository.Add(speaker);

                if (await _repository.SaveAllAsync())
                    return Created(Url.Link("SpeakerGet", new { campId = camp.Id, id = speaker.Id }), _mapper.Map<SpeakerModel>(speaker));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while adding speaker: {ex}");
            }

            return BadRequest("Could not add new speaker");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int campId, int id, [FromBody] SpeakerModel model)
        {
            try
            {
                var speaker = _repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Id != campId)
                    return BadRequest("Speaker and camp do not match");

                _mapper.Map(model, speaker);

                if (await _repository.SaveAllAsync())
                    return Ok(_mapper.Map<SpeakerModel>(speaker));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while updating speaker: {ex}");
            }

            return BadRequest("Could not update speaker");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int campId, int id)
        {
            try
            {
                var speaker = _repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Id != campId)
                    return BadRequest("Speaker and camp do not match");

                _repository.Delete(speaker);

                if (await _repository.SaveAllAsync())
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while deleting speaker: {ex}");
            }

            return BadRequest("Could not delete speaker");
        }
    }
}
