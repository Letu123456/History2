﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class CategoryHistorical
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CategoryArtifact>? CategoryArtifacts { get; set; } = new List<CategoryArtifact>();
        public ICollection<Quiz>? Quizzes { get; set; } = new List<Quiz>();
        public ICollection<Historical>? Artifacts { get; set; } = new List<Historical>();
    }
}
