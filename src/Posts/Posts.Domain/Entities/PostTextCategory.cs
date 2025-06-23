using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class PostTextCategory
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public virtual ICollection<PostWithTextCategories> PostWithTextCategories { get; set; }

    }
}
