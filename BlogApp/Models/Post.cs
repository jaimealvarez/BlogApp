using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime SubmitDateTime { get; set; }
        public bool Submitted { get; set; }
        public bool Pending { get; set; }
        public bool Approved { get; set; }
        public DateTime ApprovedDateTime { get; set; }
        public string Content { get; set; }
    }
}
