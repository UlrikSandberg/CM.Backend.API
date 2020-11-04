using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Helpers;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.UserListModels;
using CM.Backend.Queries.Queries.UserListQueries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class UserListQueryHandler : 
        IQueryHandler<GetStandardTop10Lists, IEnumerable<StandardUserList>>,
        IQueryHandler<GetStandardTop10List, IEnumerable<ChampagneLight>>
    {
        
        private const string CachedBrands = "GetAllBrandsCached";//TODO: All these cached keys should be in a global resource file!!!
        
        private readonly IChampagneRepository _champagneRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IBrandRepository _brandRepository;

        private const string BrutNature = "BrutNature";
        private const string ExtraBrut = "ExtraBrut";
        private const string Brut = "Brut";
        private const string ExtraDry = "ExtraDry";
        private const string Sec = "Sec";
        private const string DemiSec = "DemiSec";
        private const string Doux = "Doux";
        
        private const string Rose = "Rose";
        private const string BlancDeBlanc = "BlancDeBlanc";
        private const string BlancDeNoir = "BlancDeNoir";
        private const string OnIce = "OnIce";
        private const string TradBrut = "TradBrut";
        private const string TradSweet = "TradSweet";
        
        private const string BlancDeBlancKey = "BlancDeBlanc";
        private StandardUserListConfiguration BlancDeBlancConfig = new StandardUserListConfiguration
        {
            IncludedStyles = new List<string> {"BlancDeBlanc"}
        };

        private const string BlancDeNoirKey = "BlancDeNoir";
        private StandardUserListConfiguration BlancDeNoirConfig = new StandardUserListConfiguration
        {
            IncludedStyles = new List<string> {"BlancDeNoir"}
        };

        private const string BrutKey = "Brut";
        private StandardUserListConfiguration BrutConfig = new StandardUserListConfiguration
        {
            IncludedDosages = new List<string> {"Brut"},
            ExcludedStyles = new List<string> {"BlancDeBlanc", "BlancDeNoir", "Rose" , "OnIce", "TradSweet"}
        };

        private const string RoseKey = "Rose";
        private StandardUserListConfiguration RoseConfig = new StandardUserListConfiguration
        {
            IncludedStyles = new List<string> {"Rose"}
        };

        private const string DemiSeckey = "DemiSec";
        private StandardUserListConfiguration DemiSecConfig = new StandardUserListConfiguration
        {
            IncludedDosages = new List<string> {"DemiSec"}
        };

        private Dictionary<string, StandardUserListConfiguration> Top10Configurations =
            new Dictionary<string, StandardUserListConfiguration>();

        public UserListQueryHandler(IChampagneRepository champagneRepository, IMemoryCache memoryCache, IBrandRepository brandRepository)
        {
            _champagneRepository = champagneRepository;
            _memoryCache = memoryCache;
            _brandRepository = brandRepository;
            Top10Configurations.Add(BlancDeBlancKey, BlancDeBlancConfig);
            Top10Configurations.Add(BlancDeNoirKey, BlancDeNoirConfig);
            Top10Configurations.Add(BrutKey, BrutConfig);
            Top10Configurations.Add(RoseKey, RoseConfig);
            Top10Configurations.Add(DemiSeckey, DemiSecConfig);
        }
        
        public async Task<IEnumerable<StandardUserList>> HandleAsync(GetStandardTop10Lists query, CancellationToken ct)
        {
            //Fetch images for each standardTop10List
            //Generate the current 5 different standardUserList
            var blancDeBlancs = new StandardUserList
            {
                Title = "Blanc De Blancs",
                Subtitle = "Top 10 Blanc de Blancs",
                ContentType = "Champagne",
                ConfigurationKey = BlancDeBlancKey,
                ImageId = Guid.Empty,
                IsValidForVintage = false,
                IsValidForNonVintage = true
            };

            var blancDeNoir = new StandardUserList
            {
                Title = "Blanc De Noirs",
                Subtitle = "Top 10 Blanc de Blancs",
                ContentType = "Champagne",
                ConfigurationKey = BlancDeNoirKey,
                ImageId = Guid.Empty,
                IsValidForVintage = false,
                IsValidForNonVintage = false
            };

            var brut = new StandardUserList
            {
                Title = "Brut",
                Subtitle = "Top 10 Brut",
                ContentType = "Champagne",
                ConfigurationKey = BrutKey,
                ImageId = Guid.Empty,
                IsValidForVintage = true,
                IsValidForNonVintage = true
                
            };

            var rose = new StandardUserList
            {
                Title = "Rosé",
                Subtitle = "Top 10 Rosé",
                ContentType = "Champagne",
                ConfigurationKey = RoseKey,
                ImageId = Guid.Empty,
                IsValidForVintage = true,
                IsValidForNonVintage = true
            };

            var demiSec = new StandardUserList
            {
                Title = "Demi-Sec",
                Subtitle = "Top 10 Demi-Sec",
                ContentType = "Champagne",
                ConfigurationKey = DemiSeckey,
                IsValidForVintage = false,
                IsValidForNonVintage = true
            };
            
            return new List<StandardUserList>{blancDeBlancs, brut, rose, demiSec};
        }

        public async Task<IEnumerable<ChampagneLight>> HandleAsync(GetStandardTop10List query, CancellationToken ct)
        {
            //Fetch configuration
            var config = Top10Configurations[query.ConfigurationKey];

            if (config == null)
            {
                //Maybe throw exception here
                return null;
            }
            
            //Prepare configuration for search...
            var includedStyles = new List<string>();
            var includedDosages = new List<string>();

            if (config.IncludedStyles.Count == 0)
            {
                //Include all possible styles
                includedStyles = GetPossibleStyles();

                foreach (var excluded in config.ExcludedStyles)
                {
                    includedStyles.Remove(excluded);
                }
            }
            else
            {
                includedStyles = config.IncludedStyles;
            }

            if (config.IncludedDosages.Count == 0)
            {
                //Include all possible dosages
                includedDosages = GetPossibleDosages();

                foreach (var excluded in config.ExcludedDosages)
                {
                    includedDosages.Remove(excluded);
                }
            }
            else
            {
                includedDosages = config.IncludedDosages;
            }

            //Fetch the top10
            var result = await _champagneRepository.GetTop10(query.FilterByVintage, query.FilterByHighestRating,
                includedStyles, includedDosages, config.ExcludedStyles);
            
            //Fetch - In-memory cache of brand names!
            //Retreive all brands, there is only 50 so it will be okay for now, especially seeing as we are projecting on GetAll. 50 Brands * 3 properties = 20KB at most...
            var brands = await _memoryCache.GetOrCreateAsync(CachedBrands, async entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromDays(5));

                return await _brandRepository.GetAll(c => new BrandSearchProjectionModel
                {
                    Id = c.Id,
                    ImageId = c.LogoImgId,
                    Name = c.Name
                });
            });

            return result.Select(x => MappingResources.MapChampagneToChampagneLight(x, brands.SingleOrDefault(y => y.Id == x.BrandId).Name));
        }

        private List<string> GetPossibleStyles()
        {
            return new List<string> { Rose, BlancDeBlanc, BlancDeNoir, OnIce, TradBrut, TradSweet};
        }

        private List<string> GetPossibleDosages()
        {
            return new List<string> { BrutNature, ExtraBrut, Brut, ExtraDry, Sec, DemiSec, Doux };
        }
    }
}