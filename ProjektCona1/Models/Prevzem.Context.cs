﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjektCona1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ConaPrevzemEntities : DbContext
    {
        public ConaPrevzemEntities()
            : base("name=ConaPrevzemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Podatki> Podatki { get; set; }
        public virtual DbSet<Postaje> Postaje { get; set; }
        public virtual DbSet<Month> Month { get; set; }
    }
}
