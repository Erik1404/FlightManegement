﻿using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string Group_Name { get; set; }
        public string Group_Description { get; set;}
    }
}
