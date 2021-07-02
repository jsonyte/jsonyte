using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Jsonyte.Text;

namespace Jsonyte
{
    /*
        TODO:


        include
        fields
        sort
        paging
        filtering


        =========================

        GET /articles/1?include=author,comments.author

        GET /articles?fields[articles]=title,body&fields[people]=name

        GET /people?sort=age
        GET /articles?sort=-created,title

        Paging (needs strategy)
        page[number]
        page[size]
        OR
        page[offset]
        page[limit]
        OR
        page[cursor]

        Filtering (needs strategy)
        filter[name]=something
     */
    public class JsonApiQueryBuilder
    {
        private readonly List<string> includes = new();

        private readonly List<string> sorts = new();

        private readonly Dictionary<string, List<string>> includedFields = new();

        public JsonNamingPolicy NamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        public JsonApiQueryBuilder Include(string relationship)
        {
            AddMember(includes, NamingPolicy.ConvertName(relationship));

            return this;
        }

        public JsonApiQueryBuilder OrderBy(string member)
        {
            AddMember(sorts, NamingPolicy.ConvertName(member));

            return this;
        }

        public JsonApiQueryBuilder OrderByDescending(string member)
        {
            AddMember(sorts, $"-{NamingPolicy.ConvertName(member)}");

            return this;
        }

        public JsonApiQueryBuilder IncludeField(string type, string member)
        {
            return IncludeFields(type, member);
        }

        public JsonApiQueryBuilder IncludeFields(string type, params string[] members)
        {
            if (includedFields.TryGetValue(type, out var values))
            {
                includedFields[type] = values = new List<string>();
            }

            foreach (var member in members)
            {
                AddMember(values, NamingPolicy.ConvertName(member));
            }

            return this;
        }

        private void AddMember(List<string> values, string value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }
    }

    public class JsonApiQueryBuilder<T> : JsonApiQueryBuilder
    {
        private readonly List<string> includes = new();

        private readonly List<string> sorts = new();

        private readonly Dictionary<string, List<string>> includedFields = new();

        private readonly Dictionary<string, List<string>> excludedFields = new();

        private readonly Dictionary<Type, string> types = new();

        public JsonNamingPolicy NamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        public JsonApiQueryBuilder<T> Include<TRelationship>(Expression<Func<T, TRelationship>> expression)
        {
            AddMember(includes, GetMemberName(expression));

            return this;
        }

        public JsonApiQueryBuilder<T> OrderBy<TMember>(Expression<Func<T, TMember>> expression)
        {
            AddMember(sorts, GetMemberName(expression));

            return this;
        }

        public JsonApiQueryBuilder<T> OrderByDescending<TMember>(Expression<Func<T, TMember>> expression)
        {
            AddMember(sorts, $"-{GetMemberName(expression)}");

            return this;
        }

        public JsonApiQueryBuilder<T> IncludeAllFields()
        {
            return this;
        }

        public JsonApiQueryBuilder<T> IncludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, includedFields, type);

            return this;
        }

        public JsonApiQueryBuilder<T> ExcludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, excludedFields, type);

            return this;
        }

        private void AddMember(List<string> values, string value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        private void AddIncludedField<TMember>(Expression<Func<T, TMember>> expression, Dictionary<string, List<string>> fields, string? type = null)
        {
            var member = GetFinalMember(expression);

            if (member == null)
            {
                throw new JsonApiException($"Cannot parse expression: {expression}");
            }

            var field = NamingPolicy.ConvertName(member.Member.Name);
            var typeName = GetTypeName(member.Member.DeclaringType!, type);

            if (!fields.TryGetValue(typeName, out var values))
            {
                fields[typeName] = values = new List<string>();
            }

            if (!values.Contains(field))
            {
                values.Add(field);
            }
        }

        private string GetTypeName(Type type, string? name)
        {
            if (types.TryGetValue(type, out var cached))
            {
                return cached;
            }

            // 1. Use name passed in first
            if (!string.IsNullOrEmpty(name))
            {
                types[type] = name;

                return name;
            }

            // 2. Try and create the object and use the Type property
            if (type.IsResource())
            {
                var member = type.GetTypeMember();
                var constructor = type.GetConstructor(Type.EmptyTypes);

                if (constructor != null)
                {
                    var resource = constructor.Invoke(null);

                    var value = member switch
                    {
                        PropertyInfo property => property.GetValue(resource)?.ToString(),
                        FieldInfo field => field.GetValue(resource)?.ToString(),
                        _ => null
                    };

                    if (!string.IsNullOrEmpty(value))
                    {
                        types[type] = value;

                        return value;
                    }
                }
            }

            // 3. Use the type name and make a best guess
            var typeName = NamingPolicy.ConvertName(type.Name);

            return Pluralizer.Pluralize(typeName);
        }

        private MemberExpression? GetFinalMember<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            while (memberExpression != null)
            {
                if (memberExpression.Expression is not MemberExpression nested)
                {
                    return memberExpression;
                }

                memberExpression = nested;
            }

            return null;
        }

        private string GetMemberName<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var value = new StringBuilder();

            while (memberExpression != null)
            {
                var name = memberExpression.Member.Name;

                if (value.Length > 0)
                {
                    value.Append('.');
                }

                value.Append(NamingPolicy.ConvertName(name));

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            return value.ToString();
        }
    }
}
