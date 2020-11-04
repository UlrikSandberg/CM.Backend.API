using System;
using System.Collections.Generic;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class CreateAndAddBrandCellarSection : CommandWithIdResponse
    {
        public Guid BrandId { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public List<Guid> Champagnes { get; private set; }

        public CreateAndAddBrandCellarSection(Guid brandId, string title, string body, List<Guid> champagnes)
        {
            BrandId = brandId;
            Title = title;
            Body = body;
            Champagnes = champagnes;
        }
    }
}