using System;
using System.Collections.Generic;

namespace CM.Backend.API
{

	public interface ITypeRegistry
	{
		string ToString(TypeRegistry.BrandPageType brandPageType);
	}

	public class TypeRegistry : ITypeRegistry
	{
		public TypeRegistry()
		{
		}

		public enum BrandPageType
		{
			Cellar,
			InfoPage
		}

		public string ToString(BrandPageType brandPageType)
		{
			var dictionary = new Dictionary<BrandPageType, string>();
			dictionary.Add(BrandPageType.Cellar, "Cellar");
			dictionary.Add(BrandPageType.InfoPage, "InfoPage");

			return dictionary[brandPageType];

		}
	}
}