using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    /// <summary>
    /// Provides a fluent builder for creating <see href="https://jsonapi.org/">JSON:API</see> uniform resource identifiers (URIs).
    /// </summary>
    public class JsonApiUriBuilder : UriBuilder
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNames = new();

        private readonly Dictionary<string, List<string>> includedFields = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class.
        /// </summary>
        public JsonApiUriBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the specified URI.
        /// </summary>
        /// <param name="uri">A URI string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
        /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid URI.</exception>
        public JsonApiUriBuilder(string uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme and host.
        /// </summary>
        /// <param name="schemeName">An Internet access protocol.</param>
        /// <param name="hostName">A DNS-style domain name or IP address.</param>
        public JsonApiUriBuilder(string schemeName, string hostName)
            : base(schemeName, hostName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme, host and port.
        /// </summary>
        /// <param name="scheme">An Internet access protocol.</param>
        /// <param name="host">A DNS-style domain name or IP address.</param>
        /// <param name="portNumber">An IP port number for the service.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="portNumber" /> is less than -1 or greater than 65,535.</exception>
        public JsonApiUriBuilder(string scheme, string host, int portNumber)
            : base(scheme, host, portNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the scheme, host, port and path.
        /// </summary>
        /// <param name="scheme">An Internet access protocol.</param>
        /// <param name="host">A DNS-style domain name or IP address.</param>
        /// <param name="port">An IP port number for the service.</param>
        /// <param name="pathValue">The path to the Internet resource.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port" /> is less than -1 or greater than 65,535.</exception>
        public JsonApiUriBuilder(string scheme, string host, int port, string pathValue)
            : base(scheme, host, port, pathValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiUriBuilder" /> class with the specified <see cref="Uri" /> instance.
        /// </summary>
        /// <param name="uri">An instance of the <see cref="Uri" /> class.</param>
        /// <exception cref="ArgumentNullException"><paramref name="uri" /> is <see langword="null" />.</exception>
        public JsonApiUriBuilder(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Gets or sets the naming policy used to convert property names for includes, sparse fields, sorting and filtering.
        /// Default value is camel-casing.
        /// </summary>
        /// <returns>A property naming policy, or <see langword="null" /> to leave property names unchanged. </returns>
        public JsonNamingPolicy? NamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        public NameValueCollection GetQueryParameters()
        {
            return QueryParametersCollection.Parse(Query);
        }

        public JsonApiUriBuilder AddQuery(string key, params string[] values)
        {
            var parameters = QueryParametersCollection.Parse(Query);

            if (values.Length == 0)
            {
                parameters.Add(null, key);
            }
            else
            {
                foreach (var value in values)
                {
                    parameters.Add(key, value);
                }
            }

            Query = parameters.ToString();

            return this;
        }

        public JsonApiUriBuilder Include(string relationship)
        {
            return AddQuery("include", ConvertName(relationship));
        }

        public JsonApiUriBuilder OrderBy(string member)
        {
            return AddQuery("sort", ConvertName(member));
        }

        public JsonApiUriBuilder OrderByDescending(string member)
        {
            return AddQuery("sort", $"-{ConvertName(member)}");
        }

        public JsonApiUriBuilder IncludeField(string type, string member)
        {
            return IncludeFields(type, member);
        }

        public JsonApiUriBuilder IncludeFields(string type, params string[] members)
        {
            if (includedFields.TryGetValue(type, out var values))
            {
                includedFields[type] = values = new List<string>();
            }

            foreach (var member in members)
            {
                AddMember(values, NamingPolicy?.ConvertName(member) ?? member);
            }

            return this;
        }

        internal string ConvertName(string name)
        {
            return NamingPolicy?.ConvertName(name) ?? name;
        }

        internal static string GetTypeName(Type type, JsonNamingPolicy namingPolicy, string? name)
        {
            return TypeNames.GetOrAdd(type, x =>
            {
                // 1. Use name passed in first
                if (!string.IsNullOrEmpty(name))
                {
                    TypeNames[x] = name!;

                    return name!;
                }

                // 2. Try and create the object and use the Type property
                if (x.IsResource())
                {
                    var member = x.GetTypeMember();
                    var constructor = x.GetConstructor(Type.EmptyTypes);

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
                            TypeNames[x] = value;

                            return value;
                        }
                    }
                }

                // 3. Use the type name and make a best guess
                var typeName = namingPolicy.ConvertName(x.Name);

                return Pluralizer.Pluralize(typeName);
            });
        }

        private void AddMember(List<string> values, string value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }
    }

    /// <summary>
    /// Provides a fluent builder for creating <see href="https://jsonapi.org/">JSON:API</see> uniform resource identifiers (URIs)
    /// using the specified <typeparamref name="T"/> resource type.
    /// </summary>
    /// <typeparam name="T">The JSON:API resource type.</typeparam>
    public class JsonApiUriBuilder<T> : JsonApiUriBuilder
    {
        private readonly Dictionary<string, List<string>> includedFields = new();

        private readonly Dictionary<string, List<string>> excludedFields = new();

        /// <inheritdoc />
        public JsonApiUriBuilder()
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string uri)
            : base(uri)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string schemeName, string hostName)
            : base(schemeName, hostName)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string scheme, string host, int portNumber)
            : base(scheme, host, portNumber)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(string scheme, string host, int port, string pathValue)
            : base(scheme, host, port, pathValue)
        {
        }

        /// <inheritdoc />
        public JsonApiUriBuilder(Uri uri)
            : base(uri)
        {
        }

        public JsonApiUriBuilder<T> Include<TRelationship>(Expression<Func<T, TRelationship>> expression)
        {
            Include(GetMemberPath(expression));

            return this;
        }

        public JsonApiUriBuilder<T> OrderBy<TMember>(Expression<Func<T, TMember>> expression)
        {
            OrderBy(GetMemberPath(expression));

            return this;
        }

        public JsonApiUriBuilder<T> OrderByDescending<TMember>(Expression<Func<T, TMember>> expression)
        {
            OrderByDescending(GetMemberPath(expression));

            return this;
        }

        public JsonApiUriBuilder<T> IncludeAllFields()
        {
            return this;
        }

        public JsonApiUriBuilder<T> IncludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, includedFields, type);

            return this;
        }

        public JsonApiUriBuilder<T> ExcludeField<TMember>(Expression<Func<T, TMember>> expression, string? type = null)
        {
            AddIncludedField(expression, excludedFields, type);

            return this;
        }

        private void AddIncludedField<TMember>(Expression<Func<T, TMember>> expression, Dictionary<string, List<string>> fields, string? type = null)
        {
            var member = GetFinalMember(expression);

            if (member == null)
            {
                throw new JsonApiException($"Cannot parse expression: {expression}");
            }

            var field = NamingPolicy.ConvertName(GetMemberName(member.Member));
            var typeName = GetTypeName(member.Member.DeclaringType!, NamingPolicy, type);

            if (!fields.TryGetValue(typeName, out var values))
            {
                fields[typeName] = values = new List<string>();
            }

            if (!values.Contains(field))
            {
                values.Add(field);
            }
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

        private string GetMemberPath<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var values = new List<string>();

            while (memberExpression != null)
            {
                var name = GetMemberName(memberExpression.Member);

                values.Add(NamingPolicy?.ConvertName(name) ?? name);

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            values.Reverse();

            return string.Join(".", values);
        }

        private string GetMemberName(MemberInfo member)
        {
            var nameAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();

            return nameAttribute != null
                ? nameAttribute.Name
                : member.Name;
        }
    }
}
