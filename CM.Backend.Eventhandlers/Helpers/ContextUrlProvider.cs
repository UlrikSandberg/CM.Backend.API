using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CM.Backend.Domain.Aggregates.Notification.Events;

namespace CM.Backend.EventHandlers.Helpers
{

    public enum urlRoot
    {
        brand,
        user,
        champagne,
        publicuser
    }

    public enum urlSpecification
    {
        profile,
        cellar,
        page,
        inspecttasting,
        tastingcomment,
        unknown
        
    }
    
    public enum urlBrandSpecification
    {
        profile,
        cellar,
        page
    }
    
    public enum urlUserSpecification
    {
        cellar
    }
    
    public enum urlChampagneSpecification
    {
        inspecttasting,
        tastingcomment
    }
    
    public enum urlPublicUserSpecification
    {
        cellar
    }
    
    public static class ContextUrlProvider
    {
        public static string GetContextUrlForNotification(NotificationCreated notificationEvent)
        {
            if (notificationEvent.InvokedByAction.Equals(Domain.EnumOptions.NotificationAction.BrandNews) ||
                notificationEvent.InvokedByAction.Equals(Domain.EnumOptions.NotificationAction.CommunityUpdate))
            {
                return notificationEvent.ProvidedContextUrl;
            }

            if (notificationEvent.ProvidedContextUrl != null)
            {
                return notificationEvent.ProvidedContextUrl;
            }
            
            StringBuilder contextUrl = new StringBuilder();

            if (notificationEvent.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.UserFollowed))
            {
                var urlRoot = Helpers.urlRoot.publicuser;
                
                contextUrl.Append(urlRoot);

                contextUrl.Append("/");

                contextUrl.Append(notificationEvent.InvokerId.Value);

                contextUrl.Append("/");
            }

            if (notificationEvent.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.TastingCommented))
            {
                var urlRoot = Helpers.urlRoot.champagne;

                contextUrl.Append(urlRoot);

                contextUrl.Append("/");

                contextUrl.Append(notificationEvent);

                contextUrl.Append("/");
            }

            return contextUrl.ToString();
        }

        public static string ResolveContextUrlForCommentLiked(Guid champagneId, Guid tastingId, Guid commentId)
        {
            var contextUrl = new StringBuilder();

            contextUrl.Append(urlRoot.champagne);

            contextUrl.Append("/");

            contextUrl.Append(champagneId);

            contextUrl.Append("/");

            contextUrl.Append(urlChampagneSpecification.inspecttasting);

            contextUrl.Append("/");

            contextUrl.Append(tastingId);

            contextUrl.Append("/");

            contextUrl.Append(urlChampagneSpecification.tastingcomment);

            contextUrl.Append("/");

            contextUrl.Append(commentId);

            return contextUrl.ToString();
        }

        public static string ResolveContextUrlForTastingCreated(Guid champagneId, Guid tastingId)
        {
            return ResolveContextUrlForCommentOnTasting(tastingId, champagneId);
        }
        
        public static string ResolveContextUrlForTastingLiked(Guid tastingId, Guid champagneId)
        {
            return ResolveContextUrlForCommentOnTasting(tastingId, champagneId);
        }
        
        public static string ResolveContextUrlForCommentOnTasting(Guid tastingId, Guid champagneId)
        {
            var contextUrl = new StringBuilder();

            contextUrl.Append(urlRoot.champagne);

            contextUrl.Append("/");

            contextUrl.Append(champagneId);

            contextUrl.Append("/");

            contextUrl.Append(urlChampagneSpecification.inspecttasting);

            contextUrl.Append("/");

            contextUrl.Append(tastingId);

            contextUrl.Append("/");

            return contextUrl.ToString();
        }

        public static Match ConvertContextUrl(string url)
        {
            var pattern = @"^([A-z]*)\/([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]*)\/?([A-z]*)\/([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]*)?";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(url);

            return match;
        }
    }
}