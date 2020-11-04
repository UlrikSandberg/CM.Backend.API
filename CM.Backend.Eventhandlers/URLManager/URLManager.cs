using System;
namespace CM.Backend.EventHandlers.URLManager
{

	public interface IURLManager
    {
		string CreateBrandCellarURL(Guid brandId, Guid brandCellarPageId);
		string CreateBrandPageURL(Guid brandId, Guid brandPageId);
    }

    public class URLDelegateManager : IURLManager
    {

		//Root Urls
		private const string brand = "brand";
		private const string profile = "profile";
		private const string publicProfile = "publicProfile";


        private const string brandCellar = "Cellar";
        private const string publicCellar = "publicCellar";
        private const string personalCellar = "personalCellar";
        private const string brandPage = "page";
        
        public URLDelegateManager()
        {
        }
        
		public string CreateBrandCellarURL(Guid brandId, Guid brandCellarPageId)
        {
			return brand + "/" + brandId + "/" + brandCellar + "/" + brandCellarPageId;
        }

		public string CreateBrandPageURL(Guid brandId, Guid brandPageId)
        {
			return brand + "/" + brandId + "/" + brandPage + "/" + brandPageId;
        }
    }
}
