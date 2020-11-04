using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Model.CommentModels;
using CM.Backend.Queries.Model.TastingModels;
using CM.Backend.Queries.Queries.TastingQueries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class TastingQueryHandler :
        IQueryHandler<GetUserTastingForEdit, EditTastingModel>,
        IQueryHandler<GetChampagneTastings, IEnumerable<TastingModel>>,
        IQueryHandler<GetInspectTasting, InspectTastingModel>,
        IQueryHandler<GetComments, IEnumerable<CommentModel>>,
        IQueryHandler<GetComment, CommentModel>
    {
        private readonly ITastingRepository tastingRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IChampagneRepository champagneRepository;
        private readonly IUserRepository userRepository;
        private readonly ILikeRepository likeRepository;
        private readonly ICommentRepository commentRepository;

        public TastingQueryHandler(ITastingRepository tastingRepository, IBrandRepository brandRepository, IChampagneRepository champagneRepository, IUserRepository userRepository, ILikeRepository likeRepository, ICommentRepository commentRepository)
        {
            this.tastingRepository = tastingRepository;
            this.brandRepository = brandRepository;
            this.champagneRepository = champagneRepository;
            this.userRepository = userRepository;
            this.likeRepository = likeRepository;
            this.commentRepository = commentRepository;
        }
        
        public async Task<EditTastingModel> HandleAsync(GetUserTastingForEdit query, CancellationToken ct)
        {
            //Check if tasting exists
            var result = await tastingRepository.GetTasting(query.UserId, query.ChampagneId);

            //Get champagneImgId
            var champagne = await champagneRepository.GetById(query.ChampagneId);

            //Get champagneCoverImgId
            var brand = await brandRepository.GetById(champagne.BrandId);
            
            //Map all data into new editTastingModel object
            var editTastingModel = new EditTastingModel();

            if (result == null)
            {
                editTastingModel.Id = Guid.Empty;
                editTastingModel.Review = null;
                editTastingModel.Rating = 0.0;

                editTastingModel.IsTastingNull = true;
                
            }
            else
            {
                editTastingModel.Id = result.Id;
                editTastingModel.Review = result.Review;
                editTastingModel.Rating = result.Rating;
                editTastingModel.IsTastingNull = false;
            }

            editTastingModel.BrandId = brand.Id;
            editTastingModel.ChampagneId = query.ChampagneId;
            editTastingModel.BottleName = champagne.BottleName;
            editTastingModel.BrandName = brand.Name;
            editTastingModel.ChampagneBottleImgId = champagne.BottleImgId;
            editTastingModel.ChampagneCoverImgId = brand.BottleCoverImgId;

            return editTastingModel;
        }

        public async Task<IEnumerable<TastingModel>> HandleAsync(GetChampagneTastings query, CancellationToken ct)
        {
            var result = await tastingRepository.GetTastingsPagedByChampagneIdAsync(query.ChampagneId, query.Page,
                query.PageSize, query.OrderByAscendingDate);

  
            return await ConvertTastings(result, query.RequesterId);

        }


        public async Task<IEnumerable<TastingModel>> ConvertTastings(IEnumerable<Tasting> tastings, Guid requesterId)
        {
            var convertedList = new List<TastingModel>();

            foreach (var tasting in tastings)
            {
                var tastingModel = new TastingModel();
                tastingModel.Id = tasting.Id;
                tastingModel.Review = tasting.Review;
                tastingModel.Rating = tasting.Rating;

                tastingModel.ChampagneId = tasting.ChampagneId;
                tastingModel.BrandId = tasting.BrandId;
                tastingModel.TastedOnDate = tasting.TastedOnDate;

                tastingModel.NumberOfComments = await commentRepository.CountCommentsForContextId(tasting.Id);
                tastingModel.NumberOfLikes = await likeRepository.CountLike(tasting.Id);

                var author = await userRepository.GetById(tasting.AuthorId);
                if (author != null)
                {
                    tastingModel.AuthorId = tasting.AuthorId;
                    tastingModel.AuthorName = author.Name;
                    tastingModel.AuthorProfileImgId = author.ImageCustomization.ProfilePictureImgId;
                }
                else
                {
                    tastingModel.AuthorId = Guid.Empty;
                    tastingModel.AuthorName = "Inactive";
                    tastingModel.AuthorProfileImgId = Guid.Empty;
                }

                if (requesterId.Equals(Guid.Empty))
                {
                    tastingModel.IsLikedByRequester = false;
                    tastingModel.IsCommentedByRequester = false;
                }
                else
                {
                    var likeEntity = await likeRepository.GetLikeByKey(new Like.PrimaryKey
                    {
                        LikeById = requesterId,
                        LikeToContextId = tasting.Id
                    });
                    if (likeEntity != null)
                    {
                        tastingModel.IsLikedByRequester = true;
                    }

                    if (await commentRepository.IsCommentedByUser(tasting.Id, requesterId))
                    {
                        tastingModel.IsCommentedByRequester = true;
                    }
                    else
                    {
                        tastingModel.IsCommentedByRequester = false;
                    }
                    
                }
                convertedList.Add(tastingModel);
            }
            
            return convertedList;
        }

        public async Task<InspectTastingModel> HandleAsync(GetInspectTasting query, CancellationToken ct)
        {
            var tasting = await tastingRepository.GetById(query.TastingId);

            if (tasting == null)
            {
                return null;
            }
            var convertedList = await ConvertTastings(new List<Tasting> {tasting}, query.RequesterId);

            var convertedTasting = convertedList.ElementAt(0);

            //Convert into inspectTastingModel
            var inspectTastingModel = new InspectTastingModel();
            inspectTastingModel.Id = convertedTasting.Id;
            inspectTastingModel.Review = convertedTasting.Review;
            inspectTastingModel.Rating = convertedTasting.Rating;
            inspectTastingModel.AuthorId = convertedTasting.AuthorId;
            inspectTastingModel.AuthorName = convertedTasting.AuthorName;
            inspectTastingModel.AuthorProfileImgId = convertedTasting.AuthorProfileImgId;
            inspectTastingModel.TastedOnDate = convertedTasting.TastedOnDate;
            inspectTastingModel.ChampagneId = convertedTasting.ChampagneId;
            inspectTastingModel.BrandId = convertedTasting.BrandId;
            inspectTastingModel.NumberOfComments = convertedTasting.NumberOfComments;
            inspectTastingModel.NumberOfLikes = convertedTasting.NumberOfLikes;
            inspectTastingModel.IsLikedByRequester = convertedTasting.IsLikedByRequester;
            inspectTastingModel.IsCommentedByRequester = convertedTasting.IsCommentedByRequester;
            
            //Get brandName and bottleName
            var brand = await brandRepository.GetById(convertedTasting.BrandId);
            if (brand != null)
            {
                inspectTastingModel.BrandName = brand.Name;
            }
            var champagne = await champagneRepository.GetById(convertedTasting.ChampagneId);
            if (champagne != null)
            {
                inspectTastingModel.BottleName = champagne.BottleName;
            }

            if (query.RequesterId.Equals(Guid.Empty))
            {
                inspectTastingModel.IsBookmarkedByRequester = false;
                inspectTastingModel.IsTastedByRequester = false;
            }
            else
            {
                var user = await userRepository.GetById(query.RequesterId);

                inspectTastingModel.IsBookmarkedByRequester = user.BookmarkedChampagnes.Contains(tasting.ChampagneId);
                inspectTastingModel.IsTastedByRequester = user.TastedChampagnes.Contains(tasting.ChampagneId);
            }

            //Get comments for tasting
            var comments = await commentRepository.GetCommentsForContextIdPagedAsync(query.TastingId, query.Page,
                query.PageSize, query.OrderByAcendingDate);

            inspectTastingModel.Comments = await ConvertComments(comments, query.RequesterId);
            
            return inspectTastingModel;

        }

        public async Task<IEnumerable<CommentModel>> HandleAsync(GetComments query, CancellationToken ct)
        {
            var comments = await commentRepository.GetCommentsForContextIdPagedAsync(query.ContextId, query.Page,
                query.PageSize, query.AcendingOrderByDate);

            return await ConvertComments(comments, query.RequesterId);
           
        }

        private async Task<IEnumerable<CommentModel>> ConvertComments(IEnumerable<Comment> comments, Guid requesterId)
        {
            var convertedList = new List<CommentModel>();

            foreach (var comment in comments)
            {
                var commentModel = new CommentModel();
                commentModel.Id = comment.Id;
                commentModel.ContextId = comment.ContextId;
                commentModel.ContextType = comment.ContextType;
                commentModel.AuthorId = comment.AuthorId;
                commentModel.AuthorName = comment.AuthorName;
                commentModel.AuthorProfileImgId = comment.AuthorProfileImgId;
                commentModel.Date = comment.Date;
                commentModel.Comment = comment.Content;

                commentModel.NumberOfLikes = await likeRepository.CountLike(comment.Id);

                if (requesterId.Equals(Guid.Empty))
                {
                    commentModel.IsLikedByRequester = false;
                }
                else
                {
                    var likeEntity = await likeRepository.GetLikeByKey(new Like.PrimaryKey
                    {
                        LikeById = requesterId,
                        LikeToContextId = comment.Id
                    });

                    if (likeEntity != null)
                    {
                        commentModel.IsLikedByRequester = true;
                    }
                }
                
                convertedList.Add(commentModel);
            }

            return convertedList;
        }

        public async Task<CommentModel> HandleAsync(GetComment query, CancellationToken ct)
        {
            var result = await commentRepository.GetById(query.CommentId);

            var convertedResult = await ConvertComments(new List<Comment>{result}, query.RequesterId);

            return convertedResult.First();

        }
    }
}