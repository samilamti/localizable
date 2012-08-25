using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Models;

namespace Localizable.Database
{
    public class Context : DbContext
    {
        public Context() : base("ApplicationServices")
        {

        }

        public DbSet<TranslationKey> Keys { get; set; }
        public DbSet<Translation> Values { get; set; }
        public DbSet<Translator> Translators { get; set; }
        public DbSet<DownvotedKey> DownvotedKeys { get; set; }
        public DbSet<Event> Events { get; set; }

        public Translator GetTranslator(IPrincipal user)
        {
            var translator = Translators.FirstOrDefault(t => t.EMail == user.Identity.Name);
            if (translator == null)
            {
                translator = new Translator {EMail = user.Identity.Name};
                Translators.Add(translator);
                SaveChanges();
            }
            return translator;
        }
    }
}