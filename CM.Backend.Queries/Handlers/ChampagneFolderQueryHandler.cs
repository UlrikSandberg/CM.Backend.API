using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
	public class ChampagneFolderQueryHandler : 
	IQueryHandler<GetAllChampagneFolders, IEnumerable<ChampagneFolderQueryModel>>,
	IQueryHandler<GetBrandChampagneFolders, IEnumerable<ChampagneFolderQueryModel>>,
	IQueryHandler<GetChampagneFolder, ChampagneFolderQueryModel>,
	IQueryHandler<GetBrandChampagneFoldersShuffled, IEnumerable<ChampagneFolderQueryModel>>,
	IQueryHandler<GetChampagneFoldersShuffled, IEnumerable<ChampagneFolderQueryModel>>
	{
		private readonly IChampagneFolderRepository folderRepository;
		private readonly IChampagneRepository champagneRepository;
	    private readonly IBrandRepository brandRepository;

		public ChampagneFolderQueryHandler(IChampagneFolderRepository folderRepository, IChampagneRepository champagneRepository, IBrandRepository brandRepository)
        {
            this.champagneRepository = champagneRepository;
			this.folderRepository = folderRepository;
	        this.brandRepository = brandRepository;
        }

		public async Task<IEnumerable<ChampagneFolderQueryModel>> HandleAsync(GetAllChampagneFolders query, CancellationToken ct)
		{
			IEnumerable<ChampagneFolder> folders = null;
			if (query.FolderType != null)
			{
				folders = await folderRepository.GetPaged(query.Page, query.PageSize, query.FolderType);
			}
			else
			{
				folders = await folderRepository.GetPaged(query.Page, query.PageSize);
			}
			
            //List to contain all converted roots thereby only including published champagnes and summing the ratings
			var convertedRoots = new List<ChampagneFolderQueryModel>();

            //Foreach root check if any champagne is unPublished and sum the total rating of each published champagne
			foreach (var folder in folders)
			{
				var convertedRoot = await ConvertChampagneRoot(folder);
				if (convertedRoot != null)
				{
					convertedRoots.Add(convertedRoot);
				}
			}

			return convertedRoots;
		}

		public async Task<IEnumerable<ChampagneFolderQueryModel>> HandleAsync(GetBrandChampagneFolders query, CancellationToken ct)
		{
			var champagneBrandFolders = await folderRepository.GetFoldersByBrandId(query.BrandId);
			var champagneQueryFolders = new List<ChampagneFolderQueryModel>();

			foreach(var folder in champagneBrandFolders)
			{
				var result = await ConvertChampagneRoot(folder, query.IsEmptyFoldersIncluded);
                if(result != null)
				{
					champagneQueryFolders.Add(result);
				}
			}

			return champagneQueryFolders;
		}

		public async Task<ChampagneFolderQueryModel> HandleAsync(GetChampagneFolder query, CancellationToken ct)
		{
			var root = await folderRepository.GetById(query.ChampagneFolderId);
			return await ConvertChampagneRoot(root);
		}

		private async Task<ChampagneFolderQueryModel> ConvertChampagneRoot(ChampagneFolder folder, bool isEmptyFoldersIncluded = false)
		{
			var champagneList = new List<Guid>();
			champagneList.AddRange(folder.ChampagneIds);

			//Fetch all champagnes for respective root
			var champagnes = await champagneRepository.GetChampagnesFromList(champagneList);

			//Hold information about only the published champagnes
			var publishedChampagnes = new List<Champagne>();
			var publishedChampagnesId = new List<Guid>();

			double numberOfTastings = 0;
			double averageRatingSumOfTastings = 0;
			int numberOfBottlesWithTasting = 0;

			//Iterate over each champagne and check if they are published and if so include them, if not 
			foreach (var champagne in champagnes)
			{
				if (champagne.IsPublished)
				{
					//publishedChampagnes.Add(champagne);
					publishedChampagnes.Add(champagne);

					numberOfTastings += champagne.RateCount;
					averageRatingSumOfTastings += champagne.AverageRating;
				}
			}

			//Map from sorted list of champagnes to corresponding sorted list of champagneId
			foreach (var champagne in publishedChampagnes)
			{
				publishedChampagnesId.Add(champagne.Id);
				if (champagne.AverageRating > 0.0)
				{
					numberOfBottlesWithTasting++;
				}
			}

			//Fetch brandName
			var brand = await brandRepository.GetById(folder.AuthorId);

			//If isEmptyFoldersIncluded is false we should return null if 
			if (!isEmptyFoldersIncluded)
			{
				if (publishedChampagnes.Count == 0)
				{
					return null;
				}
			}

			var queryFolder = new ChampagneFolderQueryModel();
			queryFolder.Id = folder.Id;
			queryFolder.FolderName = folder.FolderName;
			queryFolder.AuthorId = folder.AuthorId;
			queryFolder.AuthorName = brand.Name;
			queryFolder.DisplayImageId = folder.DisplayImageId;
			queryFolder.ChampagneIds = publishedChampagnesId.ToArray();
			queryFolder.FolderContentType = folder.ContentType;
			queryFolder.FolderType = folder.FolderType;
			if (averageRatingSumOfTastings == 0 || numberOfTastings == 0)
			{
				queryFolder.AverageRating = 0;
			}
			else
			{
				queryFolder.AverageRating = averageRatingSumOfTastings / numberOfBottlesWithTasting;
			}

			queryFolder.SumOfRating = 0.0;
			queryFolder.NumberOfTasting = numberOfTastings;

			return queryFolder;
		}

		public async Task<IEnumerable<ChampagneFolderQueryModel>> HandleAsync(GetBrandChampagneFoldersShuffled query, CancellationToken ct)
		{
			var champagneBrandFolders = await folderRepository.GetFoldersByBrandId(query.BrandId);
			var champagneQueryFolders = new List<ChampagneFolderQueryModel>();

			foreach(var folder in champagneBrandFolders)
			{
				var result = await ConvertChampagneRoot(folder);
				if(result != null)
				{
					champagneQueryFolders.Add(result);
				}
			}
			
			//Now shuffle the brandRoots and return number corresponding to query
			var shuffledList = new List<ChampagneFolderQueryModel>();

			var randomizer = new Random();
			
			for (int i = 0; i < query.Amount; i++)
			{
				if (champagneQueryFolders.Count() != 0)
				{
					var index = randomizer.Next(0, champagneQueryFolders.Count);
					shuffledList.Add(champagneQueryFolders[index]);
					champagneQueryFolders.RemoveAt(index);
				}	
			}
			
			return shuffledList;
		}

		public async Task<IEnumerable<ChampagneFolderQueryModel>> HandleAsync(GetChampagneFoldersShuffled query, CancellationToken ct)
		{
            //Get the count of champagne root database
			var randomChampagnes = await champagneRepository.GetChampagnesRandom(query.Amount);

			var rootIdList = new List<Guid>();
			
			foreach (var champagne in randomChampagnes)
			{
				Guid editionFolderId = Guid.Empty;
				foreach (var folder in champagne.ChampagneFolderDependencies)
				{
					if (folder.Value.Equals("Editions"))
					{
						editionFolderId = Guid.Parse(folder.Key);
					}
				}

				if (!editionFolderId.Equals(Guid.Empty))
				{
					rootIdList.Add(editionFolderId);
				}
			}
			
			var champagneRoots = await folderRepository.GetFoldersFromListAsync(rootIdList);

			var convertedRoots = new List<ChampagneFolderQueryModel>();

			foreach (var root in champagneRoots)
			{
				convertedRoots.Add(await ConvertChampagneRoot(root));
			}

			return convertedRoots;
		}
	}
}
