using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace ESD.Infrastructure.Constants
{
    public static class ModelBuilderExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            //if (string.IsNullOrEmpty(input)) { return input; }

            //var startUnderscores = Regex.Match(input, @"^_+");
            //return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();

            return input.ToUpper();
        }
        public static void NamesToSnakeCase(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToSnakeCase());
               // entity.Relational().TableName = entity.Relational().TableName.ToSnakeCase();

                // Replace column names            
                foreach (var property in entity.GetProperties())
                {
                    //property.Relational().ColumnName = property.Name.ToSnakeCase();
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    //key.Relational().Name = key.Relational().Name.ToSnakeCase();
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    //key.Relational().Name = key.Relational().Name.ToSnakeCase();
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    // index.Relational().Name = index.Relational().Name.ToSnakeCase();
                    index.SetName(index.GetName().ToSnakeCase());
                }
            }
        }
    }
   
}
