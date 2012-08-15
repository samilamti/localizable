using System.Data.Entity;
using Models;

namespace Localizable.Database
{
    public class Initializer : IDatabaseInitializer<Context>
    {
        public void InitializeDatabase(Context context)
        {
            if (!context.Database.Exists())
                CreateDatabase(context);
            else if(!context.Database.CompatibleWithModel(false))
            {
                context.Database.Delete();
                CreateDatabase(context);
            }
        }

        private static void CreateDatabase(Context context)
        {
            context.Database.Create();
            CreateIndex<TranslationKey>(context, "Key");
            CreateIndex<Translation>(context, "Language");
            CreateIndex<Translator>(context, "EMail");
        }

        private static void CreateIndex<TTable>(Context context, string column)
        {
            context.Database.ExecuteSqlCommand(string.Format("create nonclustered index IX_{0}_{1} ON [{0}s] ([{1}])", typeof(TTable).Name, column));
        }
    }
}