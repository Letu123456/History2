﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class RepliCommentDto
    {
        public string Content { get; set; }
        public int? Rating { get; set; }



        public int EventId { get; set; }
        public int CommentId { get; set; }
    }
}
