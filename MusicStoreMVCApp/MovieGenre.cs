//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MusicStoreMVCApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class MovieGenre
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int GenreId { get; set; }
    
        public virtual Genre Genre { get; set; }
        public virtual Movie Movie { get; set; }
    }
}