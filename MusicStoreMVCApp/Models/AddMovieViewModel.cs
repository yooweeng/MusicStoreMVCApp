using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStoreMVCApp.Models
{
    public class AddMovieViewModel
    {
        public Movie Movie { get; set; }
        public List<Genre> Genres { get; set; } 
        public List<int> SelectedGenresId { get; set; }
    }
}