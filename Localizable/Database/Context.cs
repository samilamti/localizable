using System.Data.Entity;
using Models;

namespace Localizable.Database
{
    public class Context : DbContext
    {
        public DbSet<TranslationKey> Keys { get; set; }
        public DbSet<Translation> Values { get; set; }
        public DbSet<Translator> Translators { get; set; }
    }
}