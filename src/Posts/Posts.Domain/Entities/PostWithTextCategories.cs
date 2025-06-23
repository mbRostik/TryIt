using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class PostWithTextCategories
    {
        public int PostId { get; set; }

        public int PostTextCategoryId { get; set; }

        public virtual Post Post { get; set; }

        public virtual PostTextCategory PostTextCategory { get; set; }
    } 
}
