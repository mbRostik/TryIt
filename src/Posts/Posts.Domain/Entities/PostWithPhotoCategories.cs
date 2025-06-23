using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Entities
{
    public class PostWithPhotoCategories
    {
        public int PostId { get; set; }

        public int PostPhotoCategoryId { get; set; }

        public virtual Post Post { get; set; }
        public virtual PostPhotoCategory PostPhotoCategory { get; set; }
    }
}
