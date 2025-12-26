namespace PetStore.Infrastructure.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        // One-to-Many relationship: Each Customer can have multiple Pets
        public virtual ICollection<PetModel> Pets { get; set; } = new List<PetModel>();

        public CustomerModel()
        {
        }

        public CustomerModel(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}

