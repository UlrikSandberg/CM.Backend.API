using System.Collections.Generic;

namespace CM.Backend.Queries.Model.UserListModels
{
    public class StandardUserListConfiguration
    {
        // If any styles has been declared --> Then those styles will have affect over excludedStyles thus, excluded styles will be ignored.
        public List<string> IncludedStyles { get; set; } = new List<string>();
        
        //If any dosages has been declared --> Then those dosages will have affect over excludedDosages thus, excluded dosages will be ignored.
        public List<string> IncludedDosages { get; set; } = new List<string>();
        
        //If no includedStyles has declared we will see if there is any specific styles which should not be here. Fx Top10Brut category, we specifically do not want
        //to include blanc de blanc, blanc de Noir nor Rose.
        public List<string> ExcludedStyles { get; set; } = new List<string>();
        
        //If no includedDosages has been declared we will see if there is any specific dosages which should not be here
        public List<string> ExcludedDosages { get; set; } = new List<string>();
    }
}