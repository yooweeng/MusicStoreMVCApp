﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStoreMVCApp.Models
{
    public class AddMovieViewModel
    {
        public Movie Movie { get; set; }
        public List<GenreSelectedModel> GenreSelectedList { get; set; } 
    }
}