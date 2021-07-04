using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggerBlog.Models
{
    public class PostTags
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int TagsId { get; set; }
        public Post Post { get; set; }
        public string Tags { get; set; }
    

    }
}
