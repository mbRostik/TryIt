using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class PostPhotoCategory
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public virtual ICollection<PostWithPhotoCategories> PostWithPhotoCategories { get; set; }

    }
}
