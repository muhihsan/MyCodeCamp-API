using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{campId}/speakers")]
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
        public IActionResult Get(int campId)
        {
            var speakers = _repository.GetSpeakersByCampId(campId);

            return Ok(speakers);
        }
    }
}
