using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStoreMVCApp.Models
{
    public class GuestCustomerIndexViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IEnumerable<int> SelectedGenresId { get; set; }
        public IEnumerable<MovieIdGenreIdModel> MovieIdGenreIds { get; set; }
    }
}