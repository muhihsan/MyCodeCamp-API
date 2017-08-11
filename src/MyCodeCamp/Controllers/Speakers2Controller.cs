using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Models;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{campId}/speakers")]
    [ApiVersion("2.0")]
    public class Speakers2Controller : SpeakersController
    {
        public Speakers2Controller(ICampRepository repository, ILogger<SpeakersController> logger, IMapper mapper)
            : base(repository, logger, mapper)
        {
        }

        public override IActionResult GetWithCount(int campId, bool includeTalks)
        {
            var speakers = includeTalks ? _repository.GetSpeakersWithTalks(campId) : _repository.GetSpeakers(campId);

            return Ok(
                new
                {
                    currentTime = DateTime.UtcNow,
                    count = speakers.Count(),
                    results = _mapper.Map<IEnumerable<Speaker2Model>>(speakers)
                });
        }
    }
}
