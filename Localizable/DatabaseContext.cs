using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;

namespace Localizable
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TranslationKey> Keys { get; set; }
        public DbSet<Translation> Values { get; set; }
        public DbSet<Translator> Translators { get; set; }
    }
}