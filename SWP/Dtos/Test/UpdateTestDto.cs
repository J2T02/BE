﻿using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Test
{
    public class UpdateTestDto
    {
        public string Result { get; set; }
        public string? Note { get; set; }           
        public string FilePath { get; set; }
        public int Status { get; set; }

        public int TestType { get; set; }
    }
}
