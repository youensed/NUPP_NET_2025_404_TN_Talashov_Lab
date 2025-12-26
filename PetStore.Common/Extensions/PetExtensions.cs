using System.Collections.Generic;

namespace PetStore.Common.Extensions
{
	public static class PetExtensions
	{
		// метод розширення
		public static void ShowAllPets<T>(this IEnumerable<T> pets) where T : PetStore.Common.Models.Pet
		{
			foreach (var pet in pets)
				System.Console.WriteLine($"{pet.Name}, вік {pet.Age}");
		}
	}
}
