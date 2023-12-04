using Microsoft.AspNetCore.Mvc;

namespace Azure_PV_111.Models.Home.Db
{
    public class ProducerFormModel
    {
        [FromForm(Name = "db-producer")]
        public String Name { get; set; } = null!;

    }
}
