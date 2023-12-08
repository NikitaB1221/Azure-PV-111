namespace Azure_PV_111.Models.Home.Db
{
    public class ProductFormModel
    {
        public Guid producerId { get; set; }
        public String Name { get; set; } = null!;
        public int Year { get; set; }
    }
}
