using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Controllers;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Models
{
    public class TalkLinksResolver : IValueResolver<Talk, TalkModel, ICollection<LinkModel>>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TalkLinksResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ICollection<LinkModel> Resolve(Talk source, TalkModel destination, ICollection<LinkModel> destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.URLHELPER];

            return new List<LinkModel>
            {
                new LinkModel
                {
                    Rel = "Self",
                    Href = url.Link("GetTalk", new { campId = source.Speaker.Camp.Id, speakerId = source.Speaker.Id, id = source.Id })
                },
                new LinkModel
                {
                    Rel = "Update",
                    Href = url.Link("UpdateTalk", new { campId = source.Speaker.Camp.Id, speakerId = source.Speaker.Id, id = source.Id }),
                    Verb = "PUT"
                },
                new LinkModel
                {
                    Rel = "Speaker",
                    Href = url.Link("SpeakerGet", new { campId = source.Speaker.Camp.Id, id = source.Speaker.Id })
                }
            };
        }
    }
}
